using Orleans;
using System.Threading.Tasks;

namespace HostServer.Interface
{
    public interface ITopologyConfigurationImport : IGrainWithStringKey
    {
        Task ImportConfiguration();
    }
}
