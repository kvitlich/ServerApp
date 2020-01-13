using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CretionDate { get; set; } = DateTime.Now;

        public DateTime? DeletedDate { get; set; }

        public string Name { get; set; }
    
        public string Age { get; set; }
    }
}
