using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebMicroblogging.Models
{
    public class CreateTweetViewModel
    {
        [Required(ErrorMessage = "El contenido es obligatorio")]
        [StringLength(280, ErrorMessage = "El tweet no puede exceder los 280 caracteres")]
        [Display(Name = "Contenido")]
        public string Content { get; set; } = null!;

        [Display(Name = "Imagen")]
        public IFormFile? Image { get; set; }
    }
}
