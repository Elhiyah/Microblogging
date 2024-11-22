using System;
using System.Collections.Generic;

namespace WebMicroblogging.Models;

public partial class Follow
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public Guid FollowerId { get; set; }

    public virtual User Follower { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
