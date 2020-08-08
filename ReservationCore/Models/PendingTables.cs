using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationCore.Models
{
    public class PendingTables
    {
        public string dateTime { get; set; }
        public List<Tuple<string, DateTime>> pendingLunchTables { get; set; }
        public List<Tuple<string, DateTime>> pendingDinnerables { get; set; }

    }
}
