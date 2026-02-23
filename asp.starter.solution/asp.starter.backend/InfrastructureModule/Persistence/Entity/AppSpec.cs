using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class AppSpec
{
    public long Id { get; set; }

    public long AppId { get; set; }

    public string? Spec { get; set; }

    public virtual App App { get; set; } = null!;
}
