using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCVMSR.Models.ViewModel
{
    public class EmpleadosModel
    {
        [Key]
        public int IdEmpleado { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El nombre debe ser de 100 máximo.")]
        public string Nombre { get; set; }

        [Display(Name = "Segundo Nombre")]
        [StringLength(100, ErrorMessage = "El segundo nombre debe ser de 100 máximo.")]
        public string SegundoNombre { get; set; }

        [Display(Name = "Primer Apellido")]
        [StringLength(100, ErrorMessage = "El primer apellido debe ser de 100 máximo.")]
        public string PrimerApellido { get; set; }

        [Display(Name = "Segundo Apellido")]
        [StringLength(100, ErrorMessage = "El segundo apellido debe ser de 100 máximo.")]
        public string SegundoApellido { get; set; }
        public Nullable<System.DateTime> FechaNacimiento { get; set; }
        public Nullable<System.DateTime> FechaContratacion { get; set; }
        public Nullable<int> IdDepartamento { get; set; }
        public Nullable<int> IdPuesto { get; set; }

        [EmailAddress]
        [Display(Name = "Correo")]
        [StringLength(50, ErrorMessage = "El correo debe ser de 50 máximo.")]
        public string CorreoElectronico { get; set; }

        [Display(Name = "Teléfono")]
        [StringLength(8, ErrorMessage = "El correo debe ser de 8 máximo.")]
        public string Telefono { get; set; }
        public Nullable<bool> Estado { get; set; }

        [Display(Name = "Saldo")]
        [StringLength(20, ErrorMessage = "El saldo debe ser de 20 máximo.")]
        public int Saldo { get; set; }

        [Display(Name = "Nombre del Archivo")]
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }

        public class CorreoExiste : ValidationAttribute
        {
            protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
            {
                using (SCVMSREntities db = new SCVMSREntities())
                {
                    string correo = (string)value; //Aquí está convirtiendo a string
                    if (db.Empleados.Where(x => x.CorreoElectronico == correo).Count() > 0) //Si el conteo es mayor a 0 es por que ya existe
                    {
                        return new ValidationResult("El correo ya existe.");
                    }
                    return ValidationResult.Success;
                }
            }
        }

    }
}