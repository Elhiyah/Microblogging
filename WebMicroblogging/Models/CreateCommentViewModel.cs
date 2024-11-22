using System.ComponentModel.DataAnnotations;

namespace WebMicroblogging.Models
{
    public class CreateCommentViewModel
    {
        public int TweetId { get; set; }

        [Required(ErrorMessage = "El contenido del comentario es obligatorio")]
        [StringLength(280, ErrorMessage = "El comentario no puede exceder los 280 caracteres")]
        [Display(Name = "Comentario")]
        public string Content { get; set; } = null!;
    }
}
