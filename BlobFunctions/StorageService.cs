using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Azure.Storage.Blobs; 
using Azure.Storage.Sas;
using Azure.Storage;
using System.Collections;
using Azure.Storage.Blobs.Models;

namespace Datacom.MDS 
{
    public partial class StorageService
    {
        readonly string accountName;
        readonly string accountKey;
        readonly string connectionString;
        readonly string blobServiceEndpoint;

        public StorageService(string accountName, string accountKey)
        {
            this.accountName = accountName;
            this.accountKey = accountKey;
            this.connectionString = $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};EndpointSuffix=core.windows.net";

            blobServiceEndpoint = $"https://{accountName}.blob.core.windows.net";
        }

        /// <summary>
        /// Test the storage account connection
        /// </summary>
        public async Task<Azure.Response<Azure.Storage.Blobs.Models.BlobServiceProperties>> Test()
        {
            // Create a client that can authenticate with a connection string
            BlobServiceClient service = new BlobServiceClient(connectionString);

            // Make a service request to verify we've successfully authenticated
            var response = await service.GetPropertiesAsync();
            return response;
        }


        public Uri GetSasForBlob(string containerName, string blobname, DateTime expiry, BlobAccountSasPermissions permissions = BlobAccountSasPermissions.Read)
        {
            var offset = TimeSpan.FromMinutes(10);

            var credential = new StorageSharedKeyCredential(accountName, accountKey);
            var sas = new BlobSasBuilder
            {
                BlobName = blobname,
                BlobContainerName = containerName,
                StartsOn = DateTime.UtcNow.Subtract(offset),
                ExpiresOn = expiry.Add(offset)
            };
            sas.SetPermissions(permissions);

            UriBuilder sasUri = new UriBuilder($"{blobServiceEndpoint}/{containerName}/{blobname}");
            sasUri.Query = sas.ToSasQueryParameters(credential).ToString();

            return sasUri.Uri;
        }

        public Uri GetSasForContainer(string containerName, DateTime expiry)
        {
            var offset = TimeSpan.FromMinutes(10);

            var credential = new StorageSharedKeyCredential(accountName, accountKey);
            var sas = new BlobSasBuilder(BlobSasPermissions.Read, DateTime.UtcNow.Add(offset));
            sas.BlobContainerName = containerName;

            UriBuilder sasUri = new UriBuilder($"{blobServiceEndpoint}/{containerName}");
            sasUri.Query = sas.ToSasQueryParameters(credential).ToString();

            return sasUri.Uri;
        }

        public async Task<(bool result, string output)> StoreBlobBytes(string containerName, string fileName, byte[] bytes, bool overwrite = false)
        {
            try {
                BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                BlobClient blob = container.GetBlobClient(fileName);

                await blob.UploadAsync(BinaryData.FromBytes(bytes), overwrite);

                return (true, blob.Uri.ToString());

            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool result, string output)> StoreBlobText(string containerName, string fileName, string data, bool overwrite = false)
        {
            try {
                BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                BlobClient blob = container.GetBlobClient(fileName);

                using (var stream = new MemoryStream())
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(data);
                    writer.Flush();
                    stream.Position = 0;
                    
                    await blob.UploadAsync(stream, overwrite);
                }

                return (true, blob.Uri.ToString());

            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return (false, e.Message);
            }
        }

        // retrieve a list of the latest files in the storage account
        public (bool result, List<BlobItem> output, string message) GetLatestFiles(string containerName)
        {
            try {
                BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                var blobs = container.GetBlobs().OrderByDescending(m => m.Properties.LastModified).ToList();
                return (true, blobs, "ok");

            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return (false, null, e.Message);
            }
        }

        public async Task<(bool result, T output, string message)> RetrieveBlobBytes<T>(string containerName, string fileName)
        {
            try {
                BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                BlobClient blob = container.GetBlobClient(fileName);
                
                var response = await blob.DownloadAsync();
                StreamReader reader = new StreamReader(response.Value.Content);
                string text = reader.ReadToEnd();
                var results = JsonConvert.DeserializeObject<T>(text);

                return (true, results, "ok");

            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return (false, default(T), e.Message);
            }
        }

        // push json file to a container
        public async Task<(bool result, T output, string message)> PushBlobBytes<T>(string containerName, string fileName, T data)
        {
            try {
                BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                BlobClient blob = container.GetBlobClient(fileName);

                var json = JsonConvert.SerializeObject(data);
                using (var stream = new MemoryStream())
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(json);
                    writer.Flush();
                    stream.Position = 0;
                    
                    await blob.UploadAsync(stream, true);
                }

                return (true, data, "ok");

            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return (false, default(T), e.Message);
            }
        }


        /*
        public async Task AnonymousAuthAsync()
        {
            BlobContainerClient container = new BlobContainerClient(connectionString, Randomize("sample-container"));
            try
            {
                // Create a blob that can be accessed publicly
                await container.CreateAsync(PublicAccessType.Blob);
                BlobClient blob = container.GetBlobClient(Randomize("sample-blob"));
                await blob.UploadAsync(BinaryData.FromString("Blob Content"));

                // Anonymously access a blob given its URI
                Uri endpoint = blob.Uri;
                BlobClient anonymous = new BlobClient(endpoint);

                // Make a service request to verify we've successfully authenticated
                await anonymous.GetPropertiesAsync();
            }
            finally
            {
                await container.DeleteAsync();
            }
        }
        */
    }
}
