using Microsoft.EntityFrameworkCore;
using SecurityExplorer.SampleWebApi.Entities;

namespace SecurityExplorer.SampleWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
