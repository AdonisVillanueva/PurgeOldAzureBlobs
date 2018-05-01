using System;
using System.Configuration;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace DeleteBlobs
{
    public static class fncDeleteOldBlobs
    {
        [FunctionName("fncDeleteOldBlobs")]
        public static void Run([TimerTrigger("0 0 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"fncDeleteOldBlobs function executed at: {DateTime.Now}");

            string sourceConnectionString = GetConnectionString("SourceConnectionString", log);
     
            var storageHelper = new StorageHelper(
                log,
                sourceConnectionString
               );

            storageHelper.GetContainers();

            log.Info($"fncDeleteOldBlobs function finished at: {DateTime.Now}");
        }

        private static string GetConnectionString(string key, TraceWriter log)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings?[key]?.ConnectionString;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    log.Info($"Got value for '{key}'");
                    return connectionString;
                }
                throw new ArgumentException($"Connection string for '{key}' is not set.");
            }
            catch (Exception ex)
            {
                var msg = ex.ToString();
                log.Error(msg);
                throw;
            }
        }

        private static string GetSetting(string key, TraceWriter log)
        {
            try
            {
                string setting = CloudConfigurationManager.GetSetting(key, true);
                if (!string.IsNullOrEmpty(setting))
                {
                    log.Info($"Setting '{key}' to '{setting}'");
                    return setting;
                }
                throw new ArgumentException($"No value for '{key}'.");
            }
            catch (Exception ex)
            {
                var msg = ex.ToString();
                log.Error(msg);
                throw;
            }
        }
    }
}
