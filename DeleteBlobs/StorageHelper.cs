using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;

namespace DeleteBlobs
{
    public class StorageHelper
    {
        private const int RetryDeltaBackOff = 4;
        private const int RetryMaxAttemts = 5;
        private const int RetryMaxExecutionTime = 30;
        private const int HoursToDelete = 48;

        private readonly TraceWriter _logger;
        private readonly string _sourceConnectionString;

        public StorageHelper(
            TraceWriter logger,
            string sConnectionString)
        {
            _logger = logger;
            _sourceConnectionString = sConnectionString;
        }
         
        //Gets all the containers in the storage
        public void GetContainers()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_sourceConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            
            blobClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(RetryDeltaBackOff), RetryMaxAttemts);
            blobClient.DefaultRequestOptions.MaximumExecutionTime = TimeSpan.FromSeconds(RetryMaxExecutionTime);            

            //Get a list of blob containers in the storage
            IEnumerable<CloudBlobContainer> containers = blobClient.ListContainers();

            foreach (CloudBlobContainer c in containers)
            {
                try
                {
                    //List all blobs over 2 days (48 hours) old
                    List<ICloudBlob> cloudBlob = c.ListBlobs("", true).OfType<ICloudBlob>().Where(b => (DateTime.UtcNow.AddDays(-2) > b.Properties.LastModified.Value.DateTime)).ToList();
                    DeleteOldBlobs(cloudBlob);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                    throw;
                }
            }
        }

        static void DeleteOldBlobs(List<ICloudBlob> blobs)
        {
            //Iterate through all blobs and delete
            foreach (IListBlobItem b in blobs)
            {  
                if (b.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob cloudBlob = (CloudBlockBlob)b;
                    cloudBlob.DeleteIfExists();
                }
                else
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)b;
                    pageBlob.DeleteIfExists();
                }        
            }     
        }
    }
}
