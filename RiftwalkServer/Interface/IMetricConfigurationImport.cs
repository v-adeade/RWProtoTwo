using Orleans;
using System.Threading.Tasks;

namespace HostServer.Interface
{
    public interface IMetricConfigurationImport : IGrainWithStringKey
    {
        Task ImportConfiguration();
    }
}
