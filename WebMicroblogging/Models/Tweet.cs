using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebMicroblogging.Models;

public partial class Tweet
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El contenido es obligatorio")]
    [StringLength(280, ErrorMessage = "El tweet no puede exceder los 280 caracteres")]
    public string Content { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; } = null!;
}
