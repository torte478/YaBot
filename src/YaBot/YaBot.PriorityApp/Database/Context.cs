#nullable disable

namespace YaBot.PriorityApp.Database
{
    using Microsoft.EntityFrameworkCore;

    public partial class Context : DbContext
    {
        private readonly string connectionString;
        
        public Context()
        {
        }
        
        public Context(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Objective> Objectives { get; set; }
        public virtual DbSet<Project> Projects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Objective>(entity =>
            {
                entity.ToTable("objective");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Project).HasColumnName("project");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("text");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.ProjectNavigation)
                    .WithMany(p => p.Objectives)
                    .HasForeignKey(d => d.Project)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("objective_fk");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("project");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.Parent).HasColumnName("parent");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
