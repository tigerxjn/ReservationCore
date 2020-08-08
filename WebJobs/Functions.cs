using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace WebJobs
{
    public class Functions
    {
        public static void CheckPendingTables([TimerTrigger("*/15 * * * * *")] TimerInfo timer)
        {
            Console.WriteLine(DateTime.Now + "some");
        }
    }
}
