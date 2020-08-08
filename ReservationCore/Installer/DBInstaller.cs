using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationCore.Installer
{
    public class DBInstaller : IInstaller
    {
        void IInstaller.InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var EndpointUri = configuration["CosmosSettings:AccountUri"];
            var PrimaryKey = configuration["CosmosSettings:AccountKey"];

            var client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);

            services.AddSingleton(client);

        }
    }
}
