using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCVMSR.Models.ViewModel
{
    public class SolicitudesModel
    {
        [Key]
        public int IdSolicitud { get; set; }

        public Nullable<int> IdEmpleado { get; set; }

        [Display(Name = "Motivo")]
        public string Motivo { get; set; }
        public Nullable<System.DateTime> FechaInicio { get; set; }
        public Nullable<System.DateTime> FechaFin { get; set; }
        public string Estado { get; set; }
    }
}