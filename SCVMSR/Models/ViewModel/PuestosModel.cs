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
        [Display(Name = "Puesto")]
        public int IdPuesto { get; set; }

        [Display(Name = "Nombre Puesto")]
        [Required(ErrorMessage = "Digite el nombre.")]
        [RegularExpression(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ\s]+$", ErrorMessage = "Solo se permiten letras, incluyendo acentos y ñ.")]
        public string Nombre { get; set; }
        public Nullable<int> IdDepartamento { get; set; }

        public virtual Departamentos Departamentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Empleados> Empleados { get; set; }
    }
}