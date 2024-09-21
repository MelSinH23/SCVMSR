using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCVMSR.Models.ViewModel
{
    public class SolicitudesModel
    {
        [Display(Name = "#")]
        public int IdSolicitud { get; set; }

        [Display(Name = "Empleado")]
        public Nullable<int> IdEmpleado { get; set; }
        public string Motivo { get; set; }

        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "Digite la fecha de inicio.")]
        public Nullable<System.DateTime> FechaInicio { get; set; }

        [Display(Name = "Fecha Final")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "Digite la fecha de fin.")]
        public Nullable<System.DateTime> FechaFin { get; set; }
        public string Estado { get; set; }

        [Display(Name = "Fecha Solicitud")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "Digite la fecha de la solicitud.")]
        public Nullable<System.DateTime> FechaSolicitud { get; set; }

        public virtual Empleados Empleados { get; set; }
    }
}