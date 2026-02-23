using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class Unit
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AppCategory> AppCategories { get; set; } = new List<AppCategory>();
}
