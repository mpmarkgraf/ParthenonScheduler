using System;
using System.Collections.Generic;
using System.Text;

namespace ParthenonScheduler.Models
{
    class Users
    {
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public List<Licenses> Licenses { get; set; }
    }
}
