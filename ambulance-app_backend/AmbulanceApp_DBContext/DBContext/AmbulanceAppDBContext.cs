using AmbulanceApp_DBContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceApp_DBContext.DBContext
{
    public class AmbulanceAppDBContext : DbContext
    {
        public AmbulanceAppDBContext()
        {

        }

        public AmbulanceAppDBContext(DbContextOptions<AmbulanceAppDBContext> options) : base(options)
        {
        }

        public virtual DbSet<UserEntity> Users { get; set; } = default!;
        public virtual DbSet<RefreshTokenEntity> RefreshTokens { get; set; } = default!;
        public virtual DbSet<RolesEntity> Roles { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity =>{

                entity.ToTable("Users","dbo");

                entity.HasKey(e => e.Id).HasName("PK_Users");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.Property(e => e.PasswordHash).HasMaxLength(200);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.RoleId).IsRequired();
                entity.Property(e => e.CreatedDt).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(e => e.UpdatedDt);
                entity.Property(e => e.UpdatedBy);

                //implementing the index for null for email or phone
                entity.HasIndex(e => e.Email)
                            .IsUnique()
                            .HasDatabaseName("UQ_Users_Email_NotNull")
                            .HasFilter("[Email] IS NOT NULL AND [Email] <> ''");

                entity.HasIndex(e => e.Phone)
                      .IsUnique()
                      .HasDatabaseName("UQ_Users_Phone_NotNull")
                      .HasFilter("[Phone] IS NOT NULL AND [Phone] <> ''");
            });

            modelBuilder.Entity<RefreshTokenEntity>(entity => {
                entity.ToTable("RefreshTokens", "dbo");

                entity.HasKey(e => e.Id).HasName("PK_RefreshTokens");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).IsRequired();
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_RefreshTokens_Users");
                entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(256);
                entity.HasIndex(e => e.TokenHash).IsUnique().HasDatabaseName("UQ_RefreshTokens_TokenHash");
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.CreatedDt).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");                
                entity.Property(e => e.RevokedAt);
                entity.Property(e => e.ReplacedByTokenHash).HasMaxLength(256);
                entity.Property(e => e.IsRevoked).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.DeviceId);

                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_RefreshTokens_UserId");
                entity.HasIndex(e => e.ExpiresAt).HasDatabaseName("IX_RefreshTokens_ExpiresAt");

                entity.HasOne<DeviceDetailsEntity>()
                        .WithMany()
                        .HasForeignKey(e => e.DeviceId)
                        .HasConstraintName("FK_RefreshTokens_Devices")
                        .IsRequired(false);
            });

            modelBuilder.Entity<RolesEntity>(entity => {
                entity.ToTable("Roles", "dbo");
                entity.HasKey(e => e.Id).HasName("PK_Roles");
                entity.Property(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedDt).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
            });
        }
    }
}
