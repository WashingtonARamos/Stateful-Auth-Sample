using Microsoft.EntityFrameworkCore;

namespace Stateful_Auth_Sample.Models
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }

    }
}
