# PurgeOldAzureBlobs
Azure function that iterates through all Cloud Storage containers and deletes blobs over 48 hours old every hour.

Runs at regular interval (i.e. TimerTrigger).

Visual Studio 2017 Azure Cloud Function Project.

NUGET Packages needed: Microsoft.WindowsAzure.ConfigurationManager ( version 3.2.3) WindowsAzure.Storage (8.4.0)

StorageHelper.cs is a class that handles the work for Azure Storage.

Compile the Project > Right Click on Solution > Publish To Azure
