using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReservationCore.Installer;
using ReservationCore.Options;
using Swashbuckle.AspNetCore.Swagger;
using IContainer = Autofac.IContainer;

namespace ReservationCore
{
    public class Startup
    {
		// we use appsettings.json as configuration file to store key-value pairs, 
		// like database connection string, etc
		public IConfiguration configuration { get; }

		// we are using Autofac container here to add services and setup Dependency Injection
		public static IContainer container { get; private set; }

		// Constructor: initialize configuration 
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder().
							SetBasePath(env.ContentRootPath).
							AddJsonFile("appsettings.json", false, true).
							AddEnvironmentVariables();
			configuration = builder.Build();
		}

		// we added MVC and CORS with a new policy named "AllowAll" to allow visiting from any domains
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			var EndpointUri = configuration["CosmosSettings:AccountUri"];
			var PrimaryKey = configuration["CosmosSettings:AccountKey"];

			var client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);

			services.AddSingleton(client);
			services.AddMvc();
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					p => p.AllowAnyOrigin().
						AllowAnyHeader().
						AllowAnyMethod().
						AllowCredentials()
						);
			});
			var builder = new ContainerBuilder();

			builder.Populate(services);

			container = builder.Build();
			return new AutofacServiceProvider(container);
		}

		// We applied the "AllowAll" CORS policy and MVC service 
		// the container would be disposed when the application stopped
		public void Configure(IApplicationBuilder app,
						IHostingEnvironment env,
						ILoggerFactory loggerFctory,
						IApplicationLifetime applicationLifetime)
		{
			app.UseCors("AllowAll");
			app.UseMvc();
			applicationLifetime.ApplicationStopped.Register(() => container.Dispose());
		}
	}
}
