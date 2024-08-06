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
    public class EmpleadosController : Controller
    {
        private SCVMSREntities db = new SCVMSREntities();

        // GET: Empleados
        public ActionResult Index()
        {
            var empleados = db.Empleados.Include(e => e.Departamentos).Include(e => e.Puestos);
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
        public ActionResult Create([Bind(Include = "IdEmpleado,Nombre,SegundoNombre,PrimerApellido,SegundoApellido,FechaNacimiento,FechaContratacion,IdDepartamento,IdPuesto,CorreoElectronico,Telefono,Estado,Saldo,FileName,ImageData")] Empleados empleados)
        {
            if (ModelState.IsValid)
            {
                db.Empleados.Add(empleados);
                db.SaveChanges();
                return RedirectToAction("Index");
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
        public ActionResult Edit([Bind(Include = "IdEmpleado,Nombre,SegundoNombre,PrimerApellido,SegundoApellido,FechaNacimiento,FechaContratacion,IdDepartamento,IdPuesto,CorreoElectronico,Telefono,Estado,Saldo,FileName,ImageData")] Empleados empleados)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empleados).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
    }
}
