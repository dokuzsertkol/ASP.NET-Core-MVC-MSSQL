using Microsoft.EntityFrameworkCore;
using htddict.Models;

namespace htddict.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options) { }

        public DbSet<Entry> Entries { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
