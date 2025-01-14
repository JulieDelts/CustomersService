using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomersService.Persistence.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Status { get; set; }
    }
}
