using Microsoft.EntityFrameworkCore;

namespace NoticiasAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
    {
    }
}
