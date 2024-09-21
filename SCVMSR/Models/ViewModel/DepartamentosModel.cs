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
        [Display(Name = "Departamento")]
        public int IdDepartamento { get; set; }

        [Display(Name = "Nombre Departamento")]
        [Required(ErrorMessage = "Digite el nombre.")]
        [RegularExpression(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s]+$", ErrorMessage = "Solo se permiten letras, incluyendo acentos y ñ.")]
        public string Nombre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Empleados> Empleados { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Puestos> Puestos { get; set; }
    }
}