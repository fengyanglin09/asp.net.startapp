using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class AppUser
{
    public long Id { get; set; }

    public string? EmailAddress { get; set; }

    public string? LanId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? FullName { get; set; }

    public int? Version { get; set; }

    public DateTime? AccessDateTime { get; set; }

    public string? Department { get; set; }

    public string? JobTitle { get; set; }

    public byte[]? Photo { get; set; }

    public virtual ICollection<AppCategory> AppCategoryCreatedByNavigations { get; set; } = new List<AppCategory>();

    public virtual ICollection<AppCategory> AppCategoryDeletedByNavigations { get; set; } = new List<AppCategory>();

    public virtual ICollection<AppCategory> AppCategoryUpdatedByNavigations { get; set; } = new List<AppCategory>();

    public virtual ICollection<App> AppCreatedByNavigations { get; set; } = new List<App>();

    public virtual ICollection<App> AppDeletedByNavigations { get; set; } = new List<App>();

    public virtual ICollection<App> AppUpdatedByNavigations { get; set; } = new List<App>();

    public virtual ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();

    public virtual ICollection<UserAccess> UserAccesses { get; set; } = new List<UserAccess>();
}
