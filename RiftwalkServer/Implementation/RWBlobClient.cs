using Azure.Storage.Blobs;
using RiftRunServer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RiftRunServer.Implementation
{
    public class RWBlobClient : IRWBlobClient
    {
        private readonly BlobServiceClient blobServiceClient;

        public RWBlobClient(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        public IEnumerable<string> GetAccounts()
        {
            var downloadBlobContainerClient = blobServiceClient.GetBlobContainerClient("mdm-teleport");

            var blobs = downloadBlobContainerClient.GetBlobs().ToList();

            var blobNames = blobs.Select(x => x.Name).ToList();

            var names = new List<string>();

            blobNames.ForEach(x =>
            {
                var a = x.Split('/');

                names.Add(a[1].Trim());
            });

            return names.Distinct();
        }

        public Task GetAccountConfigurations(string accountName, string containerName)
        {
            var downloadBlobContainerClient = blobServiceClient.GetBlobContainerClient("mdm-teleport");

            var uploadBlobContainerClient = blobServiceClient.GetBlobContainerClient("mdm-external-store");

            var blobs = downloadBlobContainerClient.GetBlobs().Where(x =>
            {
                return x.Name.Contains(accountName);
            }).ToList();

            blobs.ForEach(x =>
            {
                var blobClient = downloadBlobContainerClient.GetBlobClient(x.Name);
                var b = blobClient.Download();
                uploadBlobContainerClient.UploadBlobAsync(x.Name, b.Value.Content);
            });

            return Task.CompletedTask;
        }

        public Task TestUpload()
        {
            var paths = new List<string>
            {
                @"D:\Assets\EventConfiguration-BlobCollectiorMetrics-1.json",
                @"D:\Assets\EventConfiguration-CollectiorMetrics-1.json",
                @"D:\Assets\JarvisConfiguration.json",
                @"D:\Assets\MonitorConfigurationV2.Json",
                @"D:\Assets\ResourceTypeConfiguration.json",
                @"D:\Assets\TenantConfiguration.json",
                @"D:\Assets\TopologyConfiguration.json"
            };

            var uploadBlobContainerClient = blobServiceClient.GetBlobContainerClient("mdm-teleport");

            for (int i = 0; i < 10; i++)
            {
                var accountName = Guid.NewGuid().ToString();

                paths.ForEach(async x =>
                {
                    var a = x.Split('\\');

                    var fileName = a[2];

                    var blobName = $@"2021-03-12--13-03/{accountName}/{fileName}.json";

                    using FileStream uploadFileStream = File.OpenRead(x);

                    await uploadBlobContainerClient.UploadBlobAsync(blobName, uploadFileStream);

                    uploadFileStream.Close();
                });
            }

            return Task.CompletedTask;
        }
    }
}
