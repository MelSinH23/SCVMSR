using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SCVMSR.Models;

namespace SCVMSR.Controllers
{
    //[Authorize]
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
                db.Solicitudes.Add(solicitudes);
                db.SaveChanges();
                return RedirectToAction("Index");
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
    }
}
