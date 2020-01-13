using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ServerApp
{
    public class ServerDbContext : DbContext
    {

        public ServerDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=104-04;Database=DDBase;Trusted_Connection=true;");
        }
    }
}
