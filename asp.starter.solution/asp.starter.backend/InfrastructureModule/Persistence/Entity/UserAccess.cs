using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class UserAccess
{
    public long Id { get; set; }

    public long AppUserId { get; set; }

    public DateTime? AccessDateTime { get; set; }

    public virtual AppUser AppUser { get; set; } = null!;
}
