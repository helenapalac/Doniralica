using Doniralica.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Doniralica.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<Protege> Proteges { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Wish> Wishes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
