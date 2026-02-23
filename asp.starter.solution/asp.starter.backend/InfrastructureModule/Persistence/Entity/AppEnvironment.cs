using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class AppEnvironment
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? DisplayName { get; set; }

    public long AppId { get; set; }

    public string? AppUrl { get; set; }

    public string? SwaggerUrl { get; set; }

    public string? ApigeeUrl { get; set; }

    public string? DatabaseUrl { get; set; }

    public string? AzureInfo { get; set; }

    public virtual App App { get; set; } = null!;
}
