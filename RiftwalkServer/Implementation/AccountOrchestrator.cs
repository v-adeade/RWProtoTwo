using HostServer.Interface;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RiftRunServer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HostServer.Implementation
{
    public class AccountOrchestrator : Grain, IAccountOrchestrator, IRemindable
    {
        private readonly ILogger<AccountOrchestrator> logger;

        private readonly IRWBlobClient blobClient;

        public List<string> Accounts { get; set; } = new List<string>();

        public AccountOrchestrator(ILogger<AccountOrchestrator> logger, IRWBlobClient blobClient)
        {
            this.logger = logger;
            this.blobClient = blobClient;
        }

        public Task CreateAccountOrchestrator()
        {
            logger.LogInformation("Account Manager was Activated");

            //blobClient.TestUpload();

            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {
            logger.LogInformation("grain activated");

            await RegisterOrUpdateReminder(nameof(StartOrchestratingAccount), TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1));

            await base.OnActivateAsync();
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (nameof(StartOrchestratingAccount).Equals(reminderName)) await StartOrchestratingAccount();
        }

        private async Task StartOrchestratingAccount()
        {
            logger.LogInformation("Start Orchestrating Accounts");

            Accounts = blobClient.GetAccounts().ToList();

            await Cadence(TimeSpan.FromMilliseconds(100));
        }

        private Task Cadence(TimeSpan timeSpan)
        {
            Accounts.ForEach(accountName =>
            {
                Task.Delay(timeSpan);

                var account = GrainFactory.GetGrain<IAccount>(accountName);

                account.StartImportAsync();
            });

            return Task.CompletedTask;
        }
    }
}
