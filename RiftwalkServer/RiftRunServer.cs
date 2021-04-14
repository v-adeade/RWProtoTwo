using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Hosting.ServiceFabric;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RiftRunServer.Interface;
using RiftRunServer.Implementation;
using Azure.Storage.Blobs;

namespace RiftRunServer
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class RiftRunServer : StatelessService
    {
        private static readonly string ConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        public RiftRunServer(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            var listener = OrleansServiceListener.CreateStateless(
            (fabricServiceContext, builder) =>
            {
                builder.Configure<ClusterOptions>(options =>
                {
                    options.ServiceId = fabricServiceContext.ServiceName.ToString();

                    options.ClusterId = "RitRunProto";
                });

                builder.UseAzureStorageClustering(options => options.ConnectionString = ConnectionString);

                builder.UseAzureTableReminderService(options => options.ConnectionString = ConnectionString);

                builder.AddAzureTableGrainStorageAsDefault(options => options.ConnectionString = ConnectionString);

                builder.ConfigureLogging(logging => logging.AddConsole().AddDebug());

                var azBlobClient = new BlobServiceClient(ConnectionString);
                builder.ConfigureServices(svc => 
                {
                    svc.AddSingleton(azBlobClient);

                    svc.AddSingleton<IRWBlobClient,RWBlobClient>();
                });

                var activation = fabricServiceContext.CodePackageActivationContext;

                var endpoints = activation.GetEndpoints();

                var siloEndpoint = endpoints["OrleansSiloEndpoint"];

                var gatewayEndpoint = endpoints["OrleansProxyEndpoint"];

                var hostname = fabricServiceContext.NodeContext.IPAddressOrFQDN;

                builder.ConfigureEndpoints(hostname, siloEndpoint.Port, gatewayEndpoint.Port);

                builder.ConfigureApplicationParts(parts =>
                {
                    parts.AddFromApplicationBaseDirectory();
                });

                builder.UseDashboard(options => { });
            });

            return new[] { listener };

        }
    }
}
