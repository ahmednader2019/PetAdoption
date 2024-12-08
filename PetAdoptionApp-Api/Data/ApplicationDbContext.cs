using Microsoft.EntityFrameworkCore;
using Pet_Adoption_App.Models;

namespace Pet_Adoption_App.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Add DbSets for your entities
        public DbSet<AppUser> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
        }


    }
}
