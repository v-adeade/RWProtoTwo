using HostServer.Interface;
using Orleans;
using RiftRunServer.Interface;
using System;
using System.Threading.Tasks;

namespace RiftRunServer.Implementation
{
    public class TopologyConfigurationImport : Grain, ITopologyConfigurationImport
    {
        private string accountName;

        private IRWBlobClient rwBlobClient;

        public TopologyConfigurationImport(IRWBlobClient rwBlobClient)
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
            this.rwBlobClient.GetAccountConfigurations(accountName, "topology");

            return Task.CompletedTask;
        }
    }
}
