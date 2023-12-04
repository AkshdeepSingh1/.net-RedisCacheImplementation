using Microsoft.EntityFrameworkCore;
using RedisCacheExample.Model;

namespace RedisCacheExample.DbContext
{
    public class DbApplication : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbApplication(DbContextOptions<DbApplication> options) : base(options)  
        {
        }
        public DbSet<Driver> Drivers { get; set; }  
        
    }
}
