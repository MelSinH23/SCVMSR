using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCVMSR.Models.ViewModel
{
    public class DepartamentosModel
    {
        [Key]
        public int IdDepartamento { get; set; }

        [Display(Name = "Nombre del departamento")]
        [StringLength(100, ErrorMessage = "El nombre debe ser de 100 máximo.")]
        public string Nombre { get; set; }
    }
}