using HostServer.Interface;
using Orleans;
using Orleans.Runtime;
using RiftRunServer.Implementation;
using System;
using System.Threading.Tasks;

namespace HostServer.Implementation
{
    public class Account : Grain, IAccount
    {
        private string accountName;

        public override Task OnActivateAsync()
        {
            accountName = this.GetPrimaryKeyString();

            return base.OnActivateAsync();
        }

        public async Task StartImportAsync()
        {
            Console.WriteLine("Started Configuration import for Example account");

            await CreateAccountIfItDoesNotExist();

            await ImportConfigurations();
        }

        private Task CreateAccountIfItDoesNotExist()
        {
            return Task.CompletedTask;
        }

        private Task<bool> CheckIfAllFileArePresent()
        {
            return Task.FromResult(true);
        }

        private Task ImportConfigurations()
        {
            ImportJarvisConfiguration();

            ImportMetricConfiguration();

            ImportTopologyConfiguration();

            return Task.CompletedTask;
        }

        private Task OpenICM()
        {
            Console.WriteLine("Open ICM with teleport");
            
            return Task.CompletedTask;
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            if (nameof(StartImportAsync).Equals(reminderName)) await StartImportAsync();
        }

        private Task ImportJarvisConfiguration()
        {
            Console.WriteLine("Jarvis configuration updated");

            var grain = GrainFactory.GetGrain<IJarvisConfigurationImport>(accountName);

            grain.ImportConfiguration();

            return Task.CompletedTask;
        }

        private Task ImportMetricConfiguration()
        {
            Console.WriteLine("Metric configuration updated");

            var grain = GrainFactory.GetGrain<IMetricConfigurationImport>(accountName);

            grain.ImportConfiguration();

            return Task.CompletedTask;
        }

        private Task ImportTopologyConfiguration()
        {
            Console.WriteLine("Topology configuration updated");

            var grain = GrainFactory.GetGrain<ITopologyConfigurationImport>(accountName);

            grain.ImportConfiguration();

            return Task.CompletedTask;
        }

    }
}
