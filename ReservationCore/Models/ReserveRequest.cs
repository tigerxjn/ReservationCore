using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationCore.Models
{
    public class ReserveRequest
    {
        public string id { get; set; }
        public string DateTime { get; set; }
        public List<string> ReservedLunchTablesId { get; set; }
        public List<string> ReservedDinnnerTablesId { get; set; }
        public List<string> PendingLunchTablesId { get; set; }
        public List<string> PendingDinnerTablesId { get; set; }
        public string tableId { get; set; }
        public string meal { get; set; }
    }
}
