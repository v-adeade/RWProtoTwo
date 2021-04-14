using HostServer.Interface;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;

namespace RiftRunApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public AccountController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet("{accountName}")]
        public async Task<ActionResult<string>> GetAsync(string accountName)
        {
            var grain = _clusterClient.GetGrain<IAccount>(accountName);

            await grain.StartImportAsync();

            await Task.CompletedTask;

            return Ok("importing account");
        }
    }
}
