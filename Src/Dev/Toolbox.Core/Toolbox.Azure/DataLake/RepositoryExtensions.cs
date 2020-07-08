using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Khooversoft.Toolbox.Standard;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Azure
{
    public static class RepositoryExtensions
    {
        public static DatalakePathItem ConvertTo(this PathItem subject)
        {
            return new DatalakePathItem
            {
                Name = subject.Name,
                IsDirectory = subject.IsDirectory,
                LastModified = subject.LastModified,
                ETag = subject.ETag.ToString(),
                ContentLength = subject.ContentLength,
                Owner = subject.Owner,
                Group = subject.Group,
                Permissions = subject.Permissions,
            };
        }

        public static DatalakePathProperties ConvertTo(this PathProperties subject)
        {
            return new DatalakePathProperties
            {
                LastModified = subject.LastModified,
                ContentEncoding = subject.ContentEncoding,
                ETag = subject.ETag.ToString(),
                ContentType = subject.ContentType,
                ContentLength = subject.ContentLength,
                CreatedOn = subject.CreatedOn,
            };
        }

        public static DataLakeServiceClient CreateDataLakeServiceClient(this DatalakeRepositoryOption azureStoreOption)
        {
            // Create DataLakeServiceClient using StorageSharedKeyCredentials
            var serviceUri = new Uri($"https://{azureStoreOption.AccountName}.blob.core.windows.net");

            StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(azureStoreOption.AccountName, azureStoreOption.AccountKey);
            return new DataLakeServiceClient(serviceUri, sharedKeyCredential);
        }
    }
}
