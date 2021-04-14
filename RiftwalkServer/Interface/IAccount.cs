using Orleans;
using RiftRunServer.Interface;
using System.Threading.Tasks;

namespace HostServer.Interface
{
    public interface IAccount : IGrainWithStringKey
    {
        Task StartImportAsync();
    }
}
