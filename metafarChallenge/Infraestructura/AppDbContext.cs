using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura
{
    public class AppDbContext : DbContext
    {
        public DbSet<Card> Card { get; set; }
        public DbSet<Operation> Operation { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>()
                 .Property(c => c.CardId)
                 .ValueGeneratedNever();

            modelBuilder.Entity<Operation>()
                .HasKey(o => o.OperationId);

            modelBuilder.Entity<Operation>()
                .Property(o => o.CardId);

            modelBuilder.Entity<Operation>()
            .HasOne<Card>()
            .WithMany()
            .HasForeignKey(o => o.CardId);

            base.OnModelCreating(modelBuilder);
        }
    }
}