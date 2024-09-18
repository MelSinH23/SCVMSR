using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using SCVMSR.Models;

namespace SCVMSR.Controllers
{
    [Authorize]
    public class EmpleadosController : Controller
    {
        private SCVMSREntities db = new SCVMSREntities();

        // GET: Empleados/BuscarEmpleados
        public JsonResult BuscarEmpleados(string q)
        {
            var empleados = db.Empleados
                .Where(e => e.Estado == true && (e.Nombre.Contains(q) || e.PrimerApellido.Contains(q) || e.SegundoApellido.Contains(q))) // Solo empleados activos
                .Select(e => new
                {
                    e.IdEmpleado,
                    e.Nombre,
                    e.PrimerApellido,
                    e.SegundoApellido
                })
                .ToList();

            return Json(empleados, JsonRequestBehavior.AllowGet);
        }

        // GET: Empleados
        public ActionResult Index()
        {
            var empleados = db.Empleados.Include(e => e.Departamentos).Include(e => e.Puestos);
            foreach (var empleado in empleados)
            {
                empleado.ValorDiasSaldo = (empleado.ValorDiasSaldo / 30) * empleado.Saldo;
            }
            return View(empleados.ToList());
        }

        // GET: Empleados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleados empleados = db.Empleados.Find(id);
            if (empleados == null)
            {
                return HttpNotFound();
            }
            return View(empleados);
        }

        // GET: Empleados/Create
        public ActionResult Create()
        {
            ViewBag.IdDepartamento = new SelectList(db.Departamentos, "IdDepartamento", "Nombre");
            ViewBag.IdPuesto = new SelectList(db.Puestos, "IdPuesto", "Nombre");
            return View();
        }

        // POST: Empleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdEmpleado,Nombre,SegundoNombre,PrimerApellido,SegundoApellido,Cedula,FechaNacimiento,FechaContratacion,IdDepartamento,IdPuesto,CorreoElectronico,Telefono,Estado,Saldo,Salario,FileName,ImageData")] Empleados empleados, HttpPostedFileBase imagenFile)
        {
            // Verificar si la cédula ya existe para cualquier empleado
            if (db.Empleados.Any(x => x.Cedula == empleados.Cedula))
            {
                ModelState.AddModelError("Cedula", "La cédula ya existe.");
            }

            // Verificar si el número ya existe para cualquier empleado
            if (db.Empleados.Any(x => x.Telefono == empleados.Telefono))
            {
                ModelState.AddModelError("Telefono", "El número teléfonico ya existe.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que se haya subido un archivo y que el archivo sea una imagen
                    if (imagenFile != null && imagenFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(imagenFile.FileName);
                        var fileType = imagenFile.ContentType;
                        var validTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };

                        if (validTypes.Contains(fileType))
                        {
                            // Guardar la imagen en la carpeta deseada
                            var imagePath = Path.Combine(Server.MapPath("~/FotosEmpleados/"), fileName);
                            imagenFile.SaveAs(imagePath);

                            // Asignar el nombre de la imagen al modelo
                            empleados.FileName = fileName;
                        }
                        else
                        {
                            // Si el archivo no es una imagen, agregar un error al modelo
                            ModelState.AddModelError("", "Tipo de archivo no permitido. Seleccione una imagen (JPEG, PNG, GIF o JPG).");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        db.Empleados.Add(empleados);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    // Registrar el error (podrías guardar esto en un log)
                    ModelState.AddModelError("", "No se pudo guardar la imagen. Error: " + ex.Message);
                }
            }

            ViewBag.IdDepartamento = new SelectList(db.Departamentos, "IdDepartamento", "Nombre", empleados.IdDepartamento);
            ViewBag.IdPuesto = new SelectList(db.Puestos, "IdPuesto", "Nombre", empleados.IdPuesto);
            return View(empleados);
        }

        // GET: Empleados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleados empleados = db.Empleados.Find(id);
            if (empleados == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdDepartamento = new SelectList(db.Departamentos, "IdDepartamento", "Nombre", empleados.IdDepartamento);
            ViewBag.IdPuesto = new SelectList(db.Puestos, "IdPuesto", "Nombre", empleados.IdPuesto);
            return View(empleados);
        }

        // POST: Empleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdEmpleado,Nombre,SegundoNombre,PrimerApellido,SegundoApellido,Cedula,FechaNacimiento,FechaContratacion,IdDepartamento,IdPuesto,CorreoElectronico,Telefono,Estado,Saldo,Salario,FileName,ImageData")] Empleados empleados, HttpPostedFileBase imagenFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el empleado existente de la base de datos
                    var empleadoExistente = db.Empleados.Find(empleados.IdEmpleado);

                    if (empleadoExistente == null)
                    {
                        return HttpNotFound();
                    }

                    // Verificar si se ha subido un nuevo archivo
                    if (imagenFile != null && imagenFile.ContentLength > 0)
                    {
                        // Guardar la nueva imagen en la carpeta deseada
                        var imagePath = Path.Combine(Server.MapPath("~/FotosEmpleados/"), Path.GetFileName(imagenFile.FileName));
                        imagenFile.SaveAs(imagePath);

                        // Asignar el nombre de la imagen al modelo
                        empleados.FileName = imagenFile.FileName;
                    }
                    else
                    {
                        // Si no se subió una nueva imagen, conservar la imagen existente
                        empleados.FileName = empleadoExistente.FileName;
                    }

                    // Convertir salario a formato decimal adecuado (reemplazar coma por punto si es necesario)
                    if (!string.IsNullOrEmpty(empleados.Salario.ToString()))
                    {
                        var salarioString = empleados.Salario.ToString(CultureInfo.InvariantCulture).Replace(',', '.');
                        empleados.Salario = decimal.Parse(salarioString, CultureInfo.InvariantCulture);
                    }

                    // Actualizar las propiedades del empleado sin sobrescribir la foto si no se proporciona una nueva
                    db.Entry(empleadoExistente).CurrentValues.SetValues(empleados);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    // Capturar los errores de validación y agregarlos al ModelState
                    var validationErrors = ex.EntityValidationErrors
                        .SelectMany(eve => eve.ValidationErrors)
                        .Select(ve => $"{ve.PropertyName}: {ve.ErrorMessage}")
                        .ToList();

                    // Agregar los errores al ModelState para mostrarlos en la vista
                    ModelState.AddModelError("", "Error de validación: " + string.Join(", ", validationErrors));
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones generales
                    ModelState.AddModelError("", "No se pudo guardar la información del empleado. Error: " + ex.Message);
                }
            }

            ViewBag.IdDepartamento = new SelectList(db.Departamentos, "IdDepartamento", "Nombre", empleados.IdDepartamento);
            ViewBag.IdPuesto = new SelectList(db.Puestos, "IdPuesto", "Nombre", empleados.IdPuesto);
            return View(empleados);
        }

        // GET: Empleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleados empleados = db.Empleados.Find(id);
            if (empleados == null)
            {
                return HttpNotFound();
            }
            return View(empleados);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empleados empleados = db.Empleados.Find(id);
            db.Empleados.Remove(empleados);
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

        public ActionResult MostrarEmpleados()
        {
            DataTable resumenEmpleados = MostrarResumenEmpleados();

            // Pasar los datos a la vista usando ViewBag
            ViewBag.ResumenEmpleados = resumenEmpleados;

            return View();
        }

        protected DataTable MostrarResumenEmpleados()
        {
            DataTable dataTable = new DataTable();
            string connectionString = "Server=localhost\\sqlexpress;Database=SCVMRSR;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MostrarEmpleados", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    dataTable.Load(reader);
                }
            }

            return dataTable;
        }

        public ActionResult MostrarInactivos()
        {
            DataTable empleadosInactivos = MostrarResumenInactivos();

            // Pasar los datos a la vista usando ViewBag
            ViewBag.EmpleadosInactivos = empleadosInactivos;

            return View();
        }

        protected DataTable MostrarResumenInactivos()
        {
            DataTable dataTable = new DataTable();
            string connectionString = "Server=localhost\\sqlexpress;Database=SCVMSR;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MostrarEmpleadosInactivos", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    dataTable.Load(reader);
                }
            }

            return dataTable;
        }

        public ActionResult MostrarActivos()
        {
            DataTable empleadosActivos = MostrarResumenActivos();

            // Pasar los datos a la vista usando ViewBag
            ViewBag.EmpleadosActivos = empleadosActivos;

            return View();
        }

        protected DataTable MostrarResumenActivos()
        {
            DataTable dataTable = new DataTable();
            string connectionString = "Server=localhost\\sqlexpress;Database=SCVMSR;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("MostrarEmpleadosActivos", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    dataTable.Load(reader);
                }
            }

            return dataTable;
        }

        public ActionResult CargarEmpleados()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CargarEmpleados(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        // Verificar que el archivo se lee correctamente
                        if (worksheet == null)
                        {
                            return Json(new { message = "Error al leer el archivo Excel." });
                        }

                        var rowCount = worksheet.Dimension.Rows;
                        List<string> errores = new List<string>();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            string cedula = worksheet.Cells[row, 1].Value?.ToString() ?? "";

                            // Si el empleado ya existe, omitir la inserción
                            if (db.Empleados.Any(e => e.Cedula == cedula))
                            {
                                continue;
                            }

                            // Obtener el nombre del puesto desde el archivo Excel (columna 10)
                            string nombrePuesto = worksheet.Cells[row, 10].Value?.ToString() ?? "";

                            // Buscar el puesto en la base de datos por su nombre
                            var puesto = db.Puestos.FirstOrDefault(p => p.Nombre == nombrePuesto);

                            if (puesto == null)
                            {
                                // Si no se encuentra el puesto, añadir un error y continuar con la siguiente fila
                                errores.Add($"El puesto '{nombrePuesto}' en la fila {row} no existe en la base de datos.");
                                continue;
                            }

                            // Obtener el nombre del departamento desde el archivo Excel (columna 13, por ejemplo)
                            string nombreDepartamento = worksheet.Cells[row, 11].Value?.ToString() ?? "";

                            // Buscar el departamento en la base de datos por su nombre
                            var departamento = db.Departamentos.FirstOrDefault(d => d.Nombre == nombreDepartamento);

                            if (departamento == null)
                            {
                                // Si no se encuentra el departamento, añadir un error y continuar con la siguiente fila
                                errores.Add($"El departamento '{nombreDepartamento}' en la fila {row} no existe en la base de datos.");
                                continue;
                            }

                            var empleados = new Empleados
                            {
                                Estado = true,
                                Cedula = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                                Nombre = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                                SegundoNombre = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                                PrimerApellido = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                                SegundoApellido = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                                Telefono = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                                CorreoElectronico = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                                Saldo = int.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out int saldo) ? saldo : 0,
                                Salario = worksheet.Cells[row, 9].Value != null
                                    ? Convert.ToDecimal(worksheet.Cells[row, 9].Value)
                                    : 0m,
                                IdPuesto = puesto.IdPuesto, // Asigna el IdPuesto basado en el nombre del puesto
                                IdDepartamento = departamento.IdDepartamento, // Asigna el IdDepartamento basado en el nombre del departamento
                                FechaNacimiento = DateTime.TryParse(worksheet.Cells[row, 12].Value?.ToString(), out DateTime fechaNacimiento) ? fechaNacimiento : (DateTime?)null,
                                FechaContratacion = DateTime.TryParse(worksheet.Cells[row, 13].Value?.ToString(), out DateTime fechaIngreso) ? fechaIngreso : (DateTime?)null
                            };

                            db.Empleados.Add(empleados);

                            // Guardar en lotes para mejorar el rendimiento
                            if (row % 100 == 0)
                            {
                                await db.SaveChangesAsync();
                            }
                        }

                        await db.SaveChangesAsync();

                        // Verificar si hay errores
                        if (errores.Any())
                        {
                            return Json(new { message = string.Join("<br>", errores) });
                        }
                        return Json(new { message = "Empleados cargados exitosamente." });
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    // Capturar excepciones de validación de entidades
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    List<string> errores = new List<string>();

                    // Diccionario para mapear nombres de propiedades del modelo a nombres deseados
                    Dictionary<string, string> columnNamesMapping = new Dictionary<string, string>
            {
                { "Cedula", "Cédula" },
                { "Nombre", "Nombre" },
                { "SegundoNombre", "Segundo Nombre" },
                { "PrimerApellido", "Primer Apellido" },
                { "SegundoApellido", "Segundo Apellido" },
                { "Telefono", "Numero Celular" },
                { "CorreoElectronico", "Correo Electrónico" },
                { "Saldo", "Saldo" },
                { "Salario", "Salario" },
                { "IdPuesto", "Puesto" },
                { "IdDepartamento", "Departamento" },
                { "FechaNacimiento", "Fecha de Nacimiento" },
                { "FechaContratacion", "Fecha de Contratación" },
            };

                    var erroresAgrupados = new Dictionary<string, List<string>>();

                    foreach (var error in ex.EntityValidationErrors)
                    {
                        string cedulaError = "ERROR AL INSERTAR DATOS EN LA CÉDULA: " + (string)error.Entry.CurrentValues["Cedula"] + "<br>Columnas:";
                        if (!erroresAgrupados.ContainsKey(cedulaError))
                        {
                            erroresAgrupados[cedulaError] = new List<string>();
                        }

                        foreach (var validationError in error.ValidationErrors)
                        {
                            string columnName = columnNamesMapping.ContainsKey(validationError.PropertyName) ? columnNamesMapping[validationError.PropertyName] : validationError.PropertyName;
                            string errorMessage = $" '{columnName}': {validationError.ErrorMessage}";
                            erroresAgrupados[cedulaError].Add(errorMessage);
                        }
                    }

                    foreach (var entry in erroresAgrupados)
                    {
                        string cedula = entry.Key;
                        var mensajesDeError = entry.Value;

                        errores.Add(cedula);
                        foreach (var mensaje in mensajesDeError)
                        {
                            errores.Add(mensaje);
                        }
                        errores.Add("<br>");
                    }

                    if (errores.Any())
                    {
                        return Json(new { message = string.Join("<br>", errores) });
                    }
                    else
                    {
                        return Json(new { message = "Error de validación al procesar el archivo Excel." });
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new { message = "Error en el procesamiento del archivo: " + ex.Message });
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Error, seleccione un archivo de Excel válido." });
            }
        }


    }
}
