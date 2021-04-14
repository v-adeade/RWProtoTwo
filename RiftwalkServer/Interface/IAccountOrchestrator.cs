using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HostServer.Interface
{
    public interface IAccountOrchestrator : IGrainWithStringKey
    {
        Task CreateAccountOrchestrator();
    }
}
