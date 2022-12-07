using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Linq;

namespace Datacom.MDS
{
    public static class StorageTriggers
    {
        // TODO: Add a query parameter to allow the user to specify the number of files to retrieve
        [FunctionName("RetrieveStorageDataList")]
        public static async Task<IActionResult> RetrieveStorageDataList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "data/list/all")] HttpRequest req,
            ILogger log)
        {
            var accountName = ("spatialanalyticsnowdata");
            var accountKey = ("PlMcPYCFAfD78WXPv3B85rZyqQoXsHNsJxMo7e9EZ5vVh4Sutq5pcsoYrU2SDJukTAtl3fB1nbDT+AStRehrTQ==");
            var containerName = ("imagecaptures");

            var service = new StorageService(accountName, accountKey);

            // FInd the list of files that we want to retrieve from the storage account
            var result = service.GetLatestFiles(containerName);
            if(!result.result){
                return new BadRequestObjectResult("Failed to retrieve files from storage account " + result.message);
            }

            // Loop through each file and create a SAS for the file
            var  sasList = new List<string>();
            foreach(var blob in result.output){
                var sas = service.GetSasForBlob(containerName, blob.Name, DateTime.UtcNow.AddHours(1));
                sasList.Add(sas.ToString());
            }

            return new OkObjectResult(sasList);
        }

        // TODO: Add a query parameter to allow the user to specify the number of files to retrieve
        [FunctionName("RetrieveStorageDataMergedAll")]
        public static async Task<IActionResult> RetrieveStorageDataMergedAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "data/merged/all")] HttpRequest req,
            ILogger log)
        {
            var startDate = DateTime.MinValue;
            var endDate = DateTime.Now;
            return await RetrieveStorageData(startDate, endDate);
        }

        // TODO: Add a query parameter to allow the user to specify the number of files to retrieve
        [FunctionName("RetrieveStorageDataMergedRange")]
        public static async Task<IActionResult> RetrieveStorageDataMergedRange(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "data/merged/range")] HttpRequest req,
            ILogger log)
        {
            // start date query parameter
            var reqStartDate = req.Query["startDate"];
            if (string.IsNullOrEmpty(reqStartDate))
            {
                return new BadRequestObjectResult("Please pass a start date on the query string");
            }
            var startDate = DateTime.Parse(reqStartDate);


            // end date query parameter
            var reqEndDate = req.Query["endDate"];
            if (string.IsNullOrEmpty(reqEndDate))
            {
                return new BadRequestObjectResult("Please pass an end date on the query string");
            }
            var endDate = DateTime.Parse(reqEndDate);

            return await RetrieveStorageData(startDate, endDate);
        }

        private static async Task<IActionResult> RetrieveStorageData(DateTime startDate, DateTime endDate)
        {
            var accountName = ("spatialanalyticsnowdata");
            var accountKey = ("PlMcPYCFAfD78WXPv3B85rZyqQoXsHNsJxMo7e9EZ5vVh4Sutq5pcsoYrU2SDJukTAtl3fB1nbDT+AStRehrTQ==");
            var containerName = ("imagecaptures");

            var service = new StorageService(accountName, accountKey);

            // Retrieve list of files from storage account
            // FInd the list of files that we want to retrieve from the storage account
            var blobItems = service.GetLatestFiles(containerName);
            if(!blobItems.result){
                return new BadRequestObjectResult("Failed to retrieve files from storage account " + blobItems.message);
            }

            var results = new List<Position>();
            foreach(var blob in blobItems.output){
                // Check to see of the BLobItem is within the date range
                if(blob.Properties.LastModified?.DateTime >= startDate && blob.Properties.LastModified?.DateTime <= endDate){
                    var positionItems = await service.RetrieveBlobBytes<List<Position>>(containerName, blob.Name);
                    if(positionItems.result){
                        results.AddRange(positionItems.output);
                    }
                    else {
                        return new BadRequestObjectResult("Failed to retrieve file " + blob.Name + " from storage account " + positionItems.message);
                    }
                }
            }

            return new OkObjectResult(results);
        }

        // TODO: Add a query parameter to allow the user to specify the number of files to retrieve
        [FunctionName("RetrieveStorageLatestDataStored")]
        public static async Task<IActionResult> RetrieveStorageLatestDataStored(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "data/latest/date")] HttpRequest req,
            ILogger log)
        {
            var accountName = ("spatialanalyticsnowdata");
            var accountKey = ("PlMcPYCFAfD78WXPv3B85rZyqQoXsHNsJxMo7e9EZ5vVh4Sutq5pcsoYrU2SDJukTAtl3fB1nbDT+AStRehrTQ==");
            var containerName = ("imagecaptures");

            var service = new StorageService(accountName, accountKey);

            // Retrieve list of files from storage account
            // FInd the list of files that we want to retrieve from the storage account
            var blobItems = service.GetLatestFiles(containerName);
            if(!blobItems.result){
                return new BadRequestObjectResult("Failed to retrieve files from storage account " + blobItems.message);
            }

            // Get the latest file
            var latestBlob = blobItems.output.First();
            var positionItems = await service.RetrieveBlobBytes<List<Position>>(containerName, latestBlob.Name);
            if(positionItems.result){
                return new OkObjectResult(positionItems.output);
            }
            else {
                return new BadRequestObjectResult("Failed to retrieve file " + latestBlob.Name + " from storage account " + positionItems.message);
            }
        }        
    

        
         
        // TODO: Add a query parameter to allow the user to push a file to the storage account
        [FunctionName("PushStorageData")]
        public static async Task<IActionResult> PushStorageData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "data/push")] HttpRequest req,
            ILogger log)
        {
            var accountName = ("spatialanalyticsnowdata");
            var accountKey = ("PlMcPYCFAfD78WXPv3B85rZyqQoXsHNsJxMo7e9EZ5vVh4Sutq5pcsoYrU2SDJukTAtl3fB1nbDT+AStRehrTQ==");
            var containerName = ("imagecaptures");

            var service = new StorageService(accountName, accountKey);
            
            // Get the body of the request
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var positions = JsonConvert.DeserializeObject<List<Position>>(requestBody);
            // get a datetime string
            var dateTimeString = DateTime.Now.ToString("yyyyMMddHHmmss");
            // Push the data to the storage account
            var result = await service.PushBlobBytes<List<Position>>(containerName, $"positions_{dateTimeString}.json", positions);
            if(result.result){
                return new OkObjectResult("Successfully pushed data to storage account");
            }
            else {
                return new BadRequestObjectResult("Failed to push data to storage account ");
            }
        }
    }
}