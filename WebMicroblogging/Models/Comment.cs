using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebMicroblogging.Models;

public partial class Comment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El contenido del comentario es obligatorio")]
    [StringLength(280, ErrorMessage = "El comentario no puede exceder los 280 caracteres")]
    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [Required]
    public int TweetId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public virtual Tweet Tweet { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
