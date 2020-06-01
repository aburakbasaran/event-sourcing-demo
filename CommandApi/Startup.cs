using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using CommandApi.Modules.Account;
using CommandApi.Modules.Account.Projections;
using Core;
using Core.EventStore;
using Core.Projections;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Raven.Client.Documents.Session;
using Swashbuckle.AspNetCore.Swagger;

namespace CommandApi
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureServicesAsync(services).GetAwaiter().GetResult();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region do not open
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint(
                Configuration["Swagger:Endpoint:Url"], 
                Configuration["Swagger:Endpoint:Name"]));

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            #endregion
        }
        private async Task ConfigureServicesAsync(IServiceCollection services)
        {
            await BuildEventStore(services);

            #region gerekless stuff
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    $"v{Configuration["Swagger:Version"]}", 
                    new OpenApiInfo{
                        Title   = Configuration["Swagger:Title"], 
                        Version = $"v{Configuration["Swagger:Version"]}"
                    });
            });
            #endregion
        }

        private async Task BuildEventStore(IServiceCollection services)
        {
            //Create EventStore Connection
            var eventStoreConnection = EventStoreConnection.Create(
                Configuration["EventStore:ConnectionString"],
                ConnectionSettings.Create()
                    .KeepReconnecting()
                    .EnableVerboseLogging()
                    .SetHeartbeatInterval(TimeSpan.FromMilliseconds(5 * 1000))
                    .UseDebugLogger(),
                Environment.ApplicationName
            );
            
            eventStoreConnection.Connected += (sender, args) 
                => Console.WriteLine($"Connection to {args.RemoteEndPoint} event store established.");
            
            eventStoreConnection.ErrorOccurred += (sender, args) 
                => Console.WriteLine($"Connection error : {args.Exception}");
            
            await eventStoreConnection.ConnectAsync();
            
            EventMappings.MapEventTypes();
            
            var aggregateStore = new GesAggregateStore(eventStoreConnection,null);
            var gesSnapshotStore = new GesSnapshotStore(eventStoreConnection,null);
            var repository = new Repository(aggregateStore,gesSnapshotStore);
            services.AddSingleton<IRepository>(repository);
            services.AddSingleton(new ApplicationService(repository));
            
            var documentStore = RavenDbConfiguration.Build(Configuration["RavenDb:Url"], Configuration["RavenDb:Database"]);
            IDocumentSession GetSession() => documentStore.OpenSession();

            await ProjectionManager.With
                .Connection(eventStoreConnection)
                .CheckpointStore(new RavenDbCheckPointStore(GetSession))
                .SetProjections( new Projection[]
                {
                    new ActiveAccounts(GetSession),
                    new AccountsByOwner(GetSession), 
                })
                .StartAll();
        }
    }
}