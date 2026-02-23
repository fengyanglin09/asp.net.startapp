using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using asp.starter.backend.InfrastructureModule.Persistence.Entity;

namespace asp.starter.backend.InfrastructureModule.Persistence.DbContextConfig;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<App> Apps { get; set; }

    public virtual DbSet<AppCategory> AppCategories { get; set; }

    public virtual DbSet<AppCategoryH> AppCategoryHs { get; set; }

    public virtual DbSet<AppEnvironment> AppEnvironments { get; set; }

    public virtual DbSet<AppH> AppHs { get; set; }

    public virtual DbSet<AppRepo> AppRepos { get; set; }

    public virtual DbSet<AppSpec> AppSpecs { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<AppUserRole> AppUserRoles { get; set; }

    public virtual DbSet<CategoryMetaH> CategoryMetaHs { get; set; }

    public virtual DbSet<CategoryMetum> CategoryMeta { get; set; }

    public virtual DbSet<Databasechangelog> Databasechangelogs { get; set; }

    public virtual DbSet<Databasechangeloglock> Databasechangeloglocks { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<UserAccess> UserAccesses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=spaa-dashboard-d_db;Username=spaa-dashboard-d-communal-sawfly;Password=eaXxflvhvDt:X+y4");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<App>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_app");

            entity.ToTable("app");

            entity.HasIndex(e => e.CreatedBy, "idx_app_created_by");

            entity.HasIndex(e => e.UpdatedBy, "idx_app_updated_by");

            entity.HasIndex(e => e.Name, "unique_app_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CiNumber)
                .HasMaxLength(50)
                .HasColumnName("ci_number");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.OncallUrl)
                .HasMaxLength(255)
                .HasColumnName("oncall_url");
            entity.Property(e => e.OrgUrl)
                .HasMaxLength(255)
                .HasColumnName("org_url");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.Wiki)
                .HasMaxLength(255)
                .HasColumnName("wiki");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AppCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_app_user_createdby");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.AppDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("fk_app_user_deletedby");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AppUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("fk_app_user_updatedby");

            entity.HasMany(d => d.Categories).WithMany(p => p.Apps)
                .UsingEntity<Dictionary<string, object>>(
                    "AppCategoryApp",
                    r => r.HasOne<AppCategory>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_appcategoryapp_category"),
                    l => l.HasOne<App>().WithMany()
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_appcategoryapp_app"),
                    j =>
                    {
                        j.HasKey("AppId", "CategoryId").HasName("pk_appcategoryapp");
                        j.ToTable("app_category_app");
                        j.HasIndex(new[] { "AppId" }, "idx_appcategoryapp_app_id");
                        j.HasIndex(new[] { "CategoryId" }, "idx_appcategoryapp_category_id");
                        j.IndexerProperty<long>("AppId").HasColumnName("app_id");
                        j.IndexerProperty<long>("CategoryId").HasColumnName("category_id");
                    });
        });

        modelBuilder.Entity<AppCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_appcategory");

            entity.ToTable("app_category");

            entity.HasIndex(e => e.Name, "unique_appcategory_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AppCategoryCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appcategory_user_createdby");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.AppCategoryDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("fk_appcategory_user_deletedby");

            entity.HasOne(d => d.Unit).WithMany(p => p.AppCategories)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appcategory_unit");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.AppCategoryUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("fk_appcategory_user_updatedby");
        });

        modelBuilder.Entity<AppCategoryH>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("app_category_h_pkey");

            entity.ToTable("app_category_h", "appHistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NewData)
                .HasColumnType("jsonb")
                .HasColumnName("new_data");
            entity.Property(e => e.OldData)
                .HasColumnType("jsonb")
                .HasColumnName("old_data");
            entity.Property(e => e.Operation)
                .HasMaxLength(1)
                .HasColumnName("operation");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
        });

        modelBuilder.Entity<AppEnvironment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_app_environment");

            entity.ToTable("app_environment");

            entity.HasIndex(e => e.AppId, "idx_app_environment_app_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApigeeUrl)
                .HasMaxLength(255)
                .HasColumnName("apigee_url");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.AppUrl)
                .HasMaxLength(255)
                .HasColumnName("app_url");
            entity.Property(e => e.AzureInfo)
                .HasColumnType("jsonb")
                .HasColumnName("azure_info");
            entity.Property(e => e.DatabaseUrl)
                .HasMaxLength(255)
                .HasColumnName("database_url");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .HasColumnName("display_name");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SwaggerUrl)
                .HasMaxLength(255)
                .HasColumnName("swagger_url");

            entity.HasOne(d => d.App).WithMany(p => p.AppEnvironments)
                .HasForeignKey(d => d.AppId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_app_environment_app");
        });

        modelBuilder.Entity<AppH>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("app_h_pkey");

            entity.ToTable("app_h", "appHistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NewData)
                .HasColumnType("jsonb")
                .HasColumnName("new_data");
            entity.Property(e => e.OldData)
                .HasColumnType("jsonb")
                .HasColumnName("old_data");
            entity.Property(e => e.Operation)
                .HasMaxLength(1)
                .HasColumnName("operation");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
        });

        modelBuilder.Entity<AppRepo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_app_repo");

            entity.ToTable("app_repo");

            entity.HasIndex(e => e.AppId, "idx_apprepo_app_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .HasColumnName("display_name");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.RepoUrl)
                .HasMaxLength(255)
                .HasColumnName("repo_url");

            entity.HasOne(d => d.App).WithMany(p => p.AppRepos)
                .HasForeignKey(d => d.AppId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_apprepo_app");
        });

        modelBuilder.Entity<AppSpec>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_app_spec");

            entity.ToTable("app_spec");

            entity.HasIndex(e => e.AppId, "idx_appspec_app_id");

            entity.HasIndex(e => e.AppId, "unique_appspec_appid").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppId).HasColumnName("app_id");
            entity.Property(e => e.Spec).HasColumnName("spec");

            entity.HasOne(d => d.App).WithOne(p => p.AppSpec)
                .HasForeignKey<AppSpec>(d => d.AppId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appspec_app");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_appuser");

            entity.ToTable("app_user");

            entity.HasIndex(e => e.LanId, "unique_user_lanid").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessDateTime).HasColumnName("access_date_time");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(100)
                .HasColumnName("email_address");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(255)
                .HasColumnName("job_title");
            entity.Property(e => e.LanId)
                .HasMaxLength(100)
                .HasColumnName("lan_id");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.Version)
                .HasDefaultValue(0)
                .HasColumnName("version");
        });

        modelBuilder.Entity<AppUserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_appuserrole");

            entity.ToTable("app_user_role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppUserId).HasColumnName("app_user_id");
            entity.Property(e => e.Role)
                .HasMaxLength(25)
                .HasColumnName("role");

            entity.HasOne(d => d.AppUser).WithMany(p => p.AppUserRoles)
                .HasForeignKey(d => d.AppUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appuserrole_appuser");
        });

        modelBuilder.Entity<CategoryMetaH>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_meta_h_pkey");

            entity.ToTable("category_meta_h", "appHistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NewData)
                .HasColumnType("jsonb")
                .HasColumnName("new_data");
            entity.Property(e => e.OldData)
                .HasColumnType("jsonb")
                .HasColumnName("old_data");
            entity.Property(e => e.Operation)
                .HasMaxLength(1)
                .HasColumnName("operation");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
        });

        modelBuilder.Entity<CategoryMetum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_categorymeta");

            entity.ToTable("category_meta");

            entity.HasIndex(e => e.CategoryId, "idx_categorymeta_category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.MetaKey)
                .HasMaxLength(100)
                .HasColumnName("meta_key");
            entity.Property(e => e.MetaValue)
                .HasColumnType("jsonb")
                .HasColumnName("meta_value");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryMeta)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_categorymeta_category");
        });

        modelBuilder.Entity<Databasechangelog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("databasechangelog");

            entity.Property(e => e.Author)
                .HasMaxLength(255)
                .HasColumnName("author");
            entity.Property(e => e.Comments)
                .HasMaxLength(255)
                .HasColumnName("comments");
            entity.Property(e => e.Contexts)
                .HasMaxLength(255)
                .HasColumnName("contexts");
            entity.Property(e => e.Dateexecuted)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("dateexecuted");
            entity.Property(e => e.DeploymentId)
                .HasMaxLength(10)
                .HasColumnName("deployment_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Exectype)
                .HasMaxLength(10)
                .HasColumnName("exectype");
            entity.Property(e => e.Filename)
                .HasMaxLength(255)
                .HasColumnName("filename");
            entity.Property(e => e.Id)
                .HasMaxLength(255)
                .HasColumnName("id");
            entity.Property(e => e.Labels)
                .HasMaxLength(255)
                .HasColumnName("labels");
            entity.Property(e => e.Liquibase)
                .HasMaxLength(20)
                .HasColumnName("liquibase");
            entity.Property(e => e.Md5sum)
                .HasMaxLength(35)
                .HasColumnName("md5sum");
            entity.Property(e => e.Orderexecuted).HasColumnName("orderexecuted");
            entity.Property(e => e.Tag)
                .HasMaxLength(255)
                .HasColumnName("tag");
        });

        modelBuilder.Entity<Databasechangeloglock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("databasechangeloglock_pkey");

            entity.ToTable("databasechangeloglock");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Locked).HasColumnName("locked");
            entity.Property(e => e.Lockedby)
                .HasMaxLength(255)
                .HasColumnName("lockedby");
            entity.Property(e => e.Lockgranted)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lockgranted");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_unit");

            entity.ToTable("unit");

            entity.HasIndex(e => e.Name, "unique_unit_name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserAccess>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_user_access");

            entity.ToTable("user_access");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessDateTime).HasColumnName("access_date_time");
            entity.Property(e => e.AppUserId).HasColumnName("app_user_id");

            entity.HasOne(d => d.AppUser).WithMany(p => p.UserAccesses)
                .HasForeignKey(d => d.AppUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appuseraccess_appuser");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
