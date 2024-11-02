/*using Microsoft.EntityFrameworkCore;
using RestApiCRUD.Models;

namespace RestApiCRUD.Database
{
    public class VillageContext : DbContext
    {
        public VillageContext(DbContextOptions<VillageContext> options) : base(options)
        {
           
        }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<Warrior> Warriors { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quest>().ToTable("Quest");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Warrior>().ToTable("Warrior");
        }
    }
}
*/