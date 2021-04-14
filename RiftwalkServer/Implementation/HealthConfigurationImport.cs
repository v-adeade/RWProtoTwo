using Orleans;
using RiftRunServer.Interface;
using System.Threading.Tasks;

namespace RiftRunServer.Implementation
{
    public class HealthConfigurationImport : Grain, IHealthConfigurationImport
    {
        private string accountName;

        private IRWBlobClient rwBlobClient;

        public HealthConfigurationImport(IRWBlobClient rwBlobClient)
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
            rwBlobClient.GetAccountConfigurations(accountName, "jarvis");

            return Task.CompletedTask;
        }
    }
}
