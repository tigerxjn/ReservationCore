using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account.Manage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using ReservationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationCore.Controllers
{
    public class TableController : Controller
    {
        DocumentClient _client;
        public TableController(DocumentClient client)
        {
            _client = client;
        }

        [HttpGet("api/v1/tables/{id}")]
        public TableStatus GetTableStatusAtDayAPI(string date)
        {
            return GetTableStatusAtDay(date).Result;
        }

        [HttpPost("api/v1/setpending")]
        public async Task SetPendingTableAPI([FromBody] ReserveRequest reserveRequest)
        {
            TableStatus tableStatus = new TableStatus()
            {
                id = reserveRequest.id,
                DateTime = reserveRequest.DateTime,
                ReservedDinnnerTablesId = reserveRequest.ReservedDinnnerTablesId == null ? new List<string>():reserveRequest.ReservedDinnnerTablesId,
                ReservedLunchTablesId = reserveRequest.ReservedLunchTablesId ==null ? new List<string>():reserveRequest.ReservedLunchTablesId,
                PendingDinnerTablesId = reserveRequest.PendingDinnerTablesId == null ? new List<string>():reserveRequest.PendingDinnerTablesId,
                PendingLunchTablesId = reserveRequest.PendingLunchTablesId == null ? new List<string>():reserveRequest.PendingLunchTablesId
            };

            if (reserveRequest.meal == "lunch")
            {
                if (!tableStatus.PendingLunchTablesId.Contains(reserveRequest.tableId))
                {
                    tableStatus.PendingLunchTablesId.Add(reserveRequest.tableId);
                }
            }
            else if (reserveRequest.meal == "dinner")
            {
                if (!tableStatus.PendingDinnerTablesId.Contains(reserveRequest.tableId))
                {
                    tableStatus.PendingDinnerTablesId.Add(reserveRequest.tableId);
                }
            }
            else return;

            await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri("BookingTable", "Tables"), tableStatus);
        }

        private async Task<TableStatus> GetTableStatusAtDay(string date)
        {
            string _self = string.Empty;
            if (string.IsNullOrEmpty(date))
            {
                date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Pacific Standard Time").ToString("MM/dd/yyyy");
            }

            TableStatus tableStatus = new TableStatus()
            {
                id = "",
                DateTime = date,
                ReservedDinnnerTablesId = new List<string>(),
                ReservedLunchTablesId = new List<string>(),
                PendingDinnerTablesId = new List<string>(),
                PendingLunchTablesId = new List<string>()
            };

            var query = string.Format("Select * From c where c.DateTime = \"{0}\"", date);
            var queryOptions = new FeedOptions { MaxItemCount = 500, EnableCrossPartitionQuery = true };

            var response = _client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri("BookingTable", "Tables"), query, queryOptions).AsDocumentQuery();
            while (response.HasMoreResults)
            {
                var results = await response.ExecuteNextAsync();
                foreach (var result in results)
                {
                    if (result != null)
                    {
                        tableStatus.id = result.id;
                        tableStatus.DateTime = date;
                        tableStatus.PendingDinnerTablesId = result.PendingDinnerTablesId.ToObject<List<string>>();
                        tableStatus.PendingLunchTablesId = result.PendingLunchTablesId.ToObject<List<string>>();
                        tableStatus.ReservedDinnnerTablesId = result.ReservedDinnnerTablesId.ToObject<List<string>>();
                        tableStatus.ReservedLunchTablesId = result.ReservedLunchTablesId.ToObject<List<string>>();
                        _self = result._self;
                    }
                }
            }

            //create a new document if not exsit
            if (string.IsNullOrEmpty(_self))
            {
                await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("BookingTable", "Tables"), tableStatus);
            }

            return tableStatus;
        }
    }
}
