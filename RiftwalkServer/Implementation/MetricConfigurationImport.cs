using HostServer.Interface;
using Orleans;
using RiftRunServer.Interface;
using System;
using System.Threading.Tasks;

namespace RiftRunServer.Implementation
{
    public class MetricConfigurationImport : Grain, IMetricConfigurationImport
    {
        private string accountName;

        private IRWBlobClient rwBlobClient;

        public MetricConfigurationImport(IRWBlobClient rwBlobClient)
        {
            this.rwBlobClient = rwBlobClient;
        }

        public override Task OnActivateAsync()
        {
            accountName = this.GetPrimaryKeyString();

            return base.OnActivateAsync();
        }

        public Task ImportConfiguration()
        {
            rwBlobClient.GetAccountConfigurations(accountName, "metric");

            return Task.CompletedTask;
        }

    }
}
