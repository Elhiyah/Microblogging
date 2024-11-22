using System;
using System.ComponentModel.DataAnnotations;

namespace WebMicroblogging.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El campo es obligatorio")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "El campo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El campo es obligatorio")]
        [DataType(DataType.Date, ErrorMessage = "Ingrese una fecha válida")]
        [Display(Name = "Fecha de Nacimiento")]
        [CustomValidation(typeof(RegisterViewModel), nameof(ValidateFechaNacimiento))]
        public DateTime? FechaNacimiento { get; set; }

        // Método de validación para Fecha de Nacimiento
        public static ValidationResult? ValidateFechaNacimiento(DateTime? fechaNacimiento, ValidationContext context)
        {
            if (fechaNacimiento == null)
            {
                return new ValidationResult("La fecha de nacimiento es obligatoria");
            }

            var minDate = DateTime.Today.AddYears(-13); // Edad mínima de 13 años
            if (fechaNacimiento > minDate)
            {
                return new ValidationResult("Debes tener al menos 13 años para registrarte");
            }
            if (fechaNacimiento > DateTime.Today)
            {
                return new ValidationResult("La fecha de nacimiento no puede ser en el futuro");
            }

            return ValidationResult.Success;
        }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
