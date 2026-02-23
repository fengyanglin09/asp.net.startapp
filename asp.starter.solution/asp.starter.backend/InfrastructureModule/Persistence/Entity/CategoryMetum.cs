using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class CategoryMetum
{
    public long Id { get; set; }

    public long CategoryId { get; set; }

    public string MetaKey { get; set; } = null!;

    public string? MetaValue { get; set; }

    public virtual AppCategory Category { get; set; } = null!;
}
