using System.Collections.Generic;
using System.Threading.Tasks;

namespace RiftRunServer.Interface
{
    public interface IRWBlobClient
    {
        public IEnumerable<string> GetAccounts();

        public Task GetAccountConfigurations(string accountName, string containerName);

        public Task TestUpload();
    }
}
