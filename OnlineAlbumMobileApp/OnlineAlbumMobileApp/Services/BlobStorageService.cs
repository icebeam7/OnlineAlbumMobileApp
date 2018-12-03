using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using OnlineAlbumMobileApp.Helpers;
using OnlineAlbumMobileApp.Models;

namespace OnlineAlbumMobileApp.Services
{
    public static class BlobStorageService
    {
        readonly static CloudStorageAccount cloudStorageAccount = 
            CloudStorageAccount.Parse(Constants.StorageAccountConnectionString);

        readonly static CloudBlobClient blobClient = 
            cloudStorageAccount.CreateCloudBlobClient();

        readonly static CloudBlobContainer blobContainer = 
            blobClient.GetContainerReference(Constants.ContainerName);

        public static async Task<List<OnlineImage>> GetBlobList()
        {
            await blobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, null, null);

            BlobContinuationToken token = null;
            List<OnlineImage> blobList = new List<OnlineImage>();

            if (blobContainer != null)
            {
                do
                {
                    var results = await blobContainer.ListBlobsSegmentedAsync(null, token);
                    token = results.ContinuationToken;

                    foreach (IListBlobItem item in results.Results)
                    {
                        blobList.Add(new OnlineImage()
                        {
                            ImageURL = item.Uri.AbsoluteUri,
                            FileName = item.Uri.Segments.Last()
                        });
                    }
                } while (token != null);
            }

            return blobList;
        }

        public static async Task<string> UploadBlob(Stream file, string extension)
        {
            await blobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, null, null);

            if (blobContainer != null)
            {
                var fileName = $"{Guid.NewGuid()}{extension}";

                var blob = blobContainer.GetBlockBlobReference(fileName);
                await blob.UploadFromStreamAsync(file);

                return blob.Uri.AbsoluteUri;
            }

            return string.Empty;
        }

        public static async Task<bool> DeleteBlob(string fileName)
        {
            await blobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, null, null);

            if (blobContainer != null)
            {
                var blob = blobContainer.GetBlockBlobReference(fileName);
                return await blob.DeleteIfExistsAsync();
            }

            return false;
        }
    }
}
