using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Orleans.Configuration;
using Orleans;
using Orleans.Hosting;
using System.Threading.Tasks;
using HostServer.Interface;

namespace RiftRunApi
{

    internal sealed class RiftRunApi : StatelessService
    {
        private static readonly string ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        public RiftRunApi(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var serviceName = new Uri("fabric:/RWProtoTwo/RiftRunServerService");

                        var builder = new ClientBuilder();

                        builder.Configure<ClusterOptions>(options =>
                        {
                            options.ServiceId = serviceName.ToString();
                            options.ClusterId = "RitRunProto";
                        });

                        builder.UseAzureStorageClustering(options => options.ConnectionString = ConnectionString);

                        builder.ConfigureApplicationParts(parts =>
                        {
                            parts.AddFromApplicationBaseDirectory();
                        });

                        var client = builder.Build(); 
                        
                        ConnectToServer(client);

                        var accountManager = client.GetGrain<IAccountOrchestrator>("AccountOrchestrator");//activate Orchestrator

                        accountManager.CreateAccountOrchestrator();

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(services =>
                                    {
                                        services.AddSingleton<StatelessServiceContext>(serviceContext);
                                        services.AddSingleton<IClusterClient>(client);
                                    })
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        private static void ConnectToServer(IClusterClient client)
        {
            try
            {
                client.Connect().GetAwaiter().GetResult();//just let it retry 
            }
            catch (Exception ex)
            {
                Task.Delay(3000);//wait for server to start and retry agian
                ConnectToServer(client);
            }
        }

        public void ConnectToServer()
        {
            
        }
    }
}
