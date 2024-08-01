using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCVMSR.Models.ViewModel
{
    public class PuestosModel
    {
        [Key]
        public int IdPuesto { get; set; }

        [Display(Name = "Nombre del puesto")]
        [StringLength(100, ErrorMessage = "El nombre debe ser de 100 máximo.")]
        public string Nombre { get; set; }
    }
}