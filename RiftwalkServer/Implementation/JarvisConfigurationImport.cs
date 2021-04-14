using HostServer.Interface;
using Orleans;
using RiftRunServer.Interface;
using System.Threading.Tasks;

namespace RiftRunServer.Implementation
{
    public class JarvisConfigurationImport : Grain, IJarvisConfigurationImport
    {
        private string accountName;

        private IRWBlobClient rwBlobClient;

        public JarvisConfigurationImport(IRWBlobClient rwBlobClient)
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
            rwBlobClient.GetAccountConfigurations(accountName,"jarvis");

            return Task.CompletedTask;
        }
    }
}
