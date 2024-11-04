using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Data
{
    public class VillageContext :DbContext
    {
        public VillageContext(DbContextOptions<VillageContext> options) : base(options)
        {

        }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<Warrior> Warriors { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Database.EnsureCreated();
        }

    }
}
