using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMicroblogging.Models;

public partial class Like
{
    public int Id { get; set; }

    public int TweetId { get; set; }

    public Guid UserId { get; set; }
    [ForeignKey("TweetId")]
    public virtual Tweet Tweet { get; set; } = null!;
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
