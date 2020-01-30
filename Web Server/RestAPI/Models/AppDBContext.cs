using System;
using Microsoft.EntityFrameworkCore;

namespace RestAPI.Models
{
    public class AppDBContext : DbContext
    {
        public DbSet<Posts> Post {get;set;}
        public AppDBContext (DbContextOptions options):base(options)
        {
            
        }
    }
}