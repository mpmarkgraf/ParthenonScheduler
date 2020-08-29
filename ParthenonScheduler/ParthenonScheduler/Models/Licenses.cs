using System;
using System.Collections.Generic;
using System.Text;

namespace ParthenonScheduler.Models
{
    class Licenses
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int OemId { get; set; }
        public int UserId { get; set; }
        public int MachineId { get; set; }
        public string MachineSerial { get; set; }
        public int LocationId { get; set; }
        public string Pc { get; set; }
        public string Comment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string SerialNumber { get; set; }
    }
}
