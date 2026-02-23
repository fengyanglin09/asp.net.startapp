using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class CategoryMetaH
{
    public long Id { get; set; }

    public char? Operation { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }
}
