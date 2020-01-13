using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace ServerApp
{
    public class ServerDbContext : DbContext
    {

        public ServerDbContext() : base("Server=;Database=ServerDb;Trusted_Connection=true;")
        {
                
        }


    }
}
