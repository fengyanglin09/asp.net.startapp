using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class AppRepo
{
    public long Id { get; set; }

    public long AppId { get; set; }

    public string Name { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? Description { get; set; }

    public string? RepoUrl { get; set; }

    public virtual App App { get; set; } = null!;
}
