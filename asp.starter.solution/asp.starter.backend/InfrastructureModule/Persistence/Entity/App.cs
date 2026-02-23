using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class App
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Type { get; set; }

    public string? CiNumber { get; set; }

    public string? Wiki { get; set; }

    public string? OrgUrl { get; set; }

    public string? OncallUrl { get; set; }

    public DateTime? DeletedAt { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public virtual ICollection<AppEnvironment> AppEnvironments { get; set; } = new List<AppEnvironment>();

    public virtual ICollection<AppRepo> AppRepos { get; set; } = new List<AppRepo>();

    public virtual AppSpec? AppSpec { get; set; }

    public virtual AppUser CreatedByNavigation { get; set; } = null!;

    public virtual AppUser? DeletedByNavigation { get; set; }

    public virtual AppUser? UpdatedByNavigation { get; set; }

    public virtual ICollection<AppCategory> Categories { get; set; } = new List<AppCategory>();
}
