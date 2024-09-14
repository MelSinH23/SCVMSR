using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SCVMSR.Models;


namespace SCVMSR.Controllers
{
    [Authorize]
    public class SolicitudesController : Controller
    {
        private SCVMSREntities db = new SCVMSREntities();

        // GET: Solicitudes
        public ActionResult Index()
        {
            var solicitudes = db.Solicitudes.Include(s => s.Empleados);
            return View(solicitudes.ToList());
        }

        // GET: Solicitudes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitudes solicitudes = db.Solicitudes.Find(id);
            if (solicitudes == null)
            {
                return HttpNotFound();
            }
            return View(solicitudes);
        }

        // GET: Solicitudes/Create
        public ActionResult Create()
        {
            ViewBag.IdEmpleado = new SelectList(db.Empleados, "IdEmpleado", "Nombre");
            return View();
        }

        // POST: Solicitudes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdSolicitud,IdEmpleado,Motivo,FechaInicio,FechaFin,Estado,FechaSolicitud")] Solicitudes solicitudes)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el empleado está activo
                var empleado = db.Empleados.Find(solicitudes.IdEmpleado);
                if (empleado == null || empleado.Estado == false)
                {
                    // El empleado no existe o está inactivo
                    ModelState.AddModelError("", "El empleado seleccionado está inactivo o no existe.");
                }
                else
                {
                    db.Solicitudes.Add(solicitudes);
                    db.SaveChanges();
                    // Verificar si la solicitud fue aprobada
                    if (solicitudes.Estado == "Aceptada")
                    {
                        // Llamar al procedimiento almacenado para descontar el saldo
                        DescontarSaldo(solicitudes.IdSolicitud);
                    }
                    return RedirectToAction("Index");
                }
            }
            ViewBag.IdEmpleado = new SelectList(db.Empleados, "IdEmpleado", "Nombre", solicitudes.IdEmpleado);
            return View(solicitudes);
        }

        // GET: Solicitudes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitudes solicitudes = db.Solicitudes.Find(id);
            if (solicitudes == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdEmpleado = new SelectList(db.Empleados, "IdEmpleado", "Nombre", solicitudes.IdEmpleado);
            return View(solicitudes);
        }

        // POST: Solicitudes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdSolicitud,IdEmpleado,Motivo,FechaInicio,FechaFin,Estado,FechaSolicitud")] Solicitudes solicitudes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(solicitudes).State = EntityState.Modified;
                db.SaveChanges();

                // Verificar si la solicitud fue aprobada
                if (solicitudes.Estado == "Aceptada")
                {
                    // Llamar al procedimiento almacenado para descontar el saldo
                    DescontarSaldo(solicitudes.IdSolicitud);
                }

                return RedirectToAction("Index");
            }
            ViewBag.IdEmpleado = new SelectList(db.Empleados, "IdEmpleado", "Nombre", solicitudes.IdEmpleado);
            return View(solicitudes);
        }

        // GET: Solicitudes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitudes solicitudes = db.Solicitudes.Find(id);
            if (solicitudes == null)
            {
                return HttpNotFound();
            }
            return View(solicitudes);
        }

        // POST: Solicitudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Solicitudes solicitudes = db.Solicitudes.Find(id);
            db.Solicitudes.Remove(solicitudes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult MesAnterior()
        {
            DataTable solicitudesMesAnt = MostrarResumenMesAnterior();

            // Pasar los datos a la vista usando ViewBag
            ViewBag.SolicitudesMesAnt = solicitudesMesAnt;

            return View();
        }

        protected DataTable MostrarResumenMesAnterior()
        {
            DataTable dataTable = new DataTable();
            string connectionString = "Server=localhost\\sqlexpress;Database=SCVMSR;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MostrarSolicitudesMesAnterior", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    dataTable.Load(reader);
                }
            }

            return dataTable;
        }

        public ActionResult ImprimirResumenMesAnteriorPDF()
        {
            // Establece la conexión a la base de datos
            string connectionString = "Server=localhost\\sqlexpress;Database=SCVMSR;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

            // La ruta de guardado del archivo PDF
            string rutaGuardado = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";
            string fechaHoraActual = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string nombrePDF = "ResumenMesAnterior_" + fechaHoraActual + ".pdf";

            // Crear el documento PDF
            Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
            using (FileStream fs = new FileStream(Path.Combine(rutaGuardado, nombrePDF), FileMode.Create))
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                // Agregar logo en la esquina superior izquierda
                string logoPath = Server.MapPath("~/Content/assets/img/logomunicipalidad.png"); // Ajusta la ruta del logo según corresponda
                iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsolute(65f, 65f); // Ajusta el tamaño del logo si es necesario
                logo.SetAbsolutePosition(40, doc.PageSize.Height - 60); // Ajusta la posición según sea necesario
                doc.Add(logo);

                doc.Add(new Paragraph("\n"));

                // Encabezado de la empresa y detalles del reporte
                PdfPTable headerTable = new PdfPTable(1);
                headerTable.WidthPercentage = 100;

                PdfPCell empresaCell = new PdfPCell(new Phrase("Municipalidad de San Rafael de Heredia\nSan Rafael de Heredia, Costa Rica", FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK)));
                empresaCell.HorizontalAlignment = Element.ALIGN_LEFT;
                empresaCell.Border = Rectangle.NO_BORDER;
                headerTable.AddCell(empresaCell);

                // Detalles del reporte
                PdfPCell reporteCell = new PdfPCell(new Phrase("REPORTE DE SOLICITUDES DEL MES ANTERIOR", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK)));
                reporteCell.HorizontalAlignment = Element.ALIGN_CENTER;
                reporteCell.Border = Rectangle.NO_BORDER;
                headerTable.AddCell(reporteCell);

                PdfPCell fechaReporteCell = new PdfPCell(new Phrase("Fecha del Reporte: " + DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK)));
                fechaReporteCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                fechaReporteCell.Border = Rectangle.NO_BORDER;
                headerTable.AddCell(fechaReporteCell);

                doc.Add(headerTable);

                // Espacio
                doc.Add(new Paragraph("\n"));

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GenerarResumenMesAnteriorPDF", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            dataAdapter.Fill(dataTable);

                            if (dataTable.Rows.Count > 0)
                            {
                                // Crear la tabla con un ancho fijo para las columnas
                                PdfPTable table = new PdfPTable(7); // Número de columnas en la tabla
                                table.WidthPercentage = 100;
                                table.SpacingBefore = 10f;
                                table.SpacingAfter = 10f;

                                // Definir el tamaño de las columnas
                                table.SetWidths(new float[] { 3f, 4f, 2f, 2f, 2f, 2f, 2f }); // Ajustar los tamaños de las columnas

                                // Añadir encabezados de la tabla
                                string[] headers = { "Motivo", "Nombre Completo", "Fecha Inicio", "Fecha Fin", "Días Solicitados", "Fecha Solicitud", "Estado" };
                                foreach (var header in headers)
                                {
                                    PdfPCell headerCell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE)));
                                    headerCell.BackgroundColor = BaseColor.GRAY;
                                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    headerCell.Padding = 5f;
                                    table.AddCell(headerCell);
                                }

                                // Añadir los datos de las solicitudes
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    table.AddCell(new PdfPCell(new Phrase(row["Motivo"].ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK))));
                                    table.AddCell(new PdfPCell(new Phrase(row["NombreCompleto"].ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK))));
                                    table.AddCell(new PdfPCell(new Phrase(Convert.ToDateTime(row["FechaInicio"]).ToString("dd/MM/yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK))));
                                    table.AddCell(new PdfPCell(new Phrase(Convert.ToDateTime(row["FechaFin"]).ToString("dd/MM/yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK))));
                                    table.AddCell(new PdfPCell(new Phrase(row["DiasSolicitados"].ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK))));
                                    table.AddCell(new PdfPCell(new Phrase(Convert.ToDateTime(row["FechaSolicitud"]).ToString("dd/MM/yyyy"), FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK))));
                                    table.AddCell(new PdfPCell(new Phrase(row["Estado"].ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK)))); // Añadir el estado de la solicitud
                                }

                                doc.Add(table);
                            }
                            else
                            {
                                doc.Add(new Paragraph("No hay solicitudes registradas en el mes anterior."));
                            }
                        }
                    }
                }

                // Pie de página indicando que no es un comprobante fiscal
                doc.Add(new Paragraph("\n"));
                Paragraph footer = new Paragraph("Este reporte contiene las solicitudes del anterior mes.", FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK));
                footer.Alignment = Element.ALIGN_CENTER;
                doc.Add(footer);

                doc.Close();
            }

            // Retornar la vista o descargar directamente el PDF
            return File(Path.Combine(rutaGuardado, nombrePDF), "application/pdf", nombrePDF);
        }


        private void DescontarSaldo(int idSolicitud)
        {
            string connectionString = "Server=localhost\\sqlexpress;Database=SCVMSR;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DescontarSaldo", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@IdSolicitud", idSolicitud));

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }


}
