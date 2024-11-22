using System.ComponentModel.DataAnnotations;

namespace WebMicroblogging.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo es obligatorio")]
        public string UsernameOrEmail { get; set; } = null!;

        [Required(ErrorMessage = "El campo es obligatorio")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
