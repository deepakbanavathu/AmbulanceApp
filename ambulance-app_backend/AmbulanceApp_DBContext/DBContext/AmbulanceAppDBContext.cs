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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity =>{

                entity.ToTable("Users","dbo");

                entity.HasKey(e => e.Id).HasName("PK_Users");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.HasIndex(e => e.Phone).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.RoleId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(e => e.UpdateAt);
                entity.Property(e => e.UpdatedBy);
            });

            modelBuilder.Entity<RefreshTokenEntity>(entity => {
                entity.ToTable("RefreshTokens", "dbo");

                entity.HasKey(e => e.Id).HasName("PK_RefreshTokens");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).IsRequired();
                entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_RefreshTokens_Users");
                entity.Property(e => e.TokenHash).IsRequired().HasMaxLength(256);
                entity.Property(e => e.ExpiresAt).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(e => e.RevokedAt);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(256);
                entity.Property(e => e.IsRevoked).IsRequired().HasDefaultValue(false);
            });
        }
    }
}
