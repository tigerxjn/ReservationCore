using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationCore.Models
{
    public class TableStatus
    {
        public string id { get; set; }
        public string DateTime { get; set; }
        public List<string> ReservedLunchTablesId { get; set; }
        public List<string> ReservedDinnnerTablesId { get; set; }
        public List<string> PendingLunchTablesId { get; set; }
        public List<string> PendingDinnerTablesId { get; set; }

    }
}
