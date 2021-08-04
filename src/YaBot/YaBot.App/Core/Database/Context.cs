#nullable disable

namespace YaBot.App.Core.Database
{
    using Microsoft.EntityFrameworkCore;

    public class Context : DbContext
    {
        private readonly string connectionString;
        
        public Context(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Context(DbContextOptions<Context> options, string connectionString)
            : base(options)
        {
            this.connectionString = connectionString;
        }

        public virtual DbSet<Place> Places { get; set; }

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

            modelBuilder.Entity<Place>(entity =>
            {
                entity.ToTable("place");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("character varying")
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        private void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            throw new System.NotImplementedException();
        }
    }
}
