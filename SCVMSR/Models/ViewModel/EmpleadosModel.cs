using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Display(Name = "Cédula")]
        [StringLength(100, ErrorMessage = "La cédula debe ser de 100 máximo.")]
        public string Cedula { get; set; }

        [Display(Name = "Segundo Nombre")]
        [StringLength(100, ErrorMessage = "El segundo nombre debe ser de 100 máximo.")]
        public string SegundoNombre { get; set; }

        [Display(Name = "Primer Apellido")]
        [StringLength(100, ErrorMessage = "El primer apellido debe ser de 100 máximo.")]
        public string PrimerApellido { get; set; }

        [Display(Name = "Segundo Apellido")]
        [StringLength(100, ErrorMessage = "El segundo apellido debe ser de 100 máximo.")]
        public string SegundoApellido { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> FechaNacimiento { get; set; }

        [Display(Name = "Fecha Contratación")]

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> FechaContratacion { get; set; }
        public Nullable<int> IdDepartamento { get; set; }
        public Nullable<int> IdPuesto { get; set; }

        [EmailAddress]
        [Display(Name = "Correo")]
        [StringLength(50, ErrorMessage = "El correo debe ser de 50 máximo.")]
        [CorreoExiste(ErrorMessage = "El correo ya existe.")]
        public string CorreoElectronico { get; set; }

        [Display(Name = "Teléfono")]
        [StringLength(8, ErrorMessage = "El teléfono debe ser de 8 máximo.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe contener solo números y tener exactamente 8 dígitos.")]
        public string Telefono { get; set; }
        public Nullable<bool> Estado { get; set; }

        [Display(Name = "Saldo")]
        public int Saldo { get; set; }
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }

        [Range(0, 10000000, ErrorMessage = "El saldo debe ser un valor entre 0 y 10,000,000.")]
        public decimal Salario { get; set; }

        [NotMapped] // Este atributo indica que este campo no se almacenará en la base de datos
        public decimal ValorDiasSaldo { get; set; }

        public virtual Departamentos Departamentos { get; set; }
        public virtual Puestos Puestos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Solicitudes> Solicitudes { get; set; }

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