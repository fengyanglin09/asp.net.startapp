using System;
using System.Collections.Generic;

namespace asp.starter.backend.InfrastructureModule.Persistence.Entity;

public partial class AppCategory
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public long UnitId { get; set; }

    public DateTime? DeletedAt { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long CreatedBy { get; set; }

    public virtual ICollection<CategoryMetum> CategoryMeta { get; set; } = new List<CategoryMetum>();

    public virtual AppUser CreatedByNavigation { get; set; } = null!;

    public virtual AppUser? DeletedByNavigation { get; set; }

    public virtual Unit Unit { get; set; } = null!;

    public virtual AppUser? UpdatedByNavigation { get; set; }

    public virtual ICollection<App> Apps { get; set; } = new List<App>();
}
