@description('Location for all resources.')
param location string = resourceGroup().location
@description('Connection string to Azure storage')
@secure()
param azureStorageConnectionString string
@description('Connection string to MS SQL database')
@secure()
param defaultConnection string
@description('Connection string to Redis')
@secure()
param redisConnection string
param appname string
param environmentName string
param containerImage string = 'wallymathieu/auctions-api-csharp:latest'

var appName = 'app-${appname}-${environmentName}'

param serverFarmId string

resource app 'Microsoft.Web/sites@2022-09-01' = {
    name: appName
    location: location
    identity: {
        type: 'SystemAssigned'
    }
    properties: {
        siteConfig: {
            vnetRouteAllEnabled: true
            linuxFxVersion: 'DOCKER|${containerImage}'
            minTlsVersion: '1.2'
            scmMinTlsVersion: '1.2'
            connectionStrings: [
                {
                    name: 'AzureStorage'
                    connectionString: azureStorageConnectionString
                    type: 'Custom'
                }
                {
                    name: 'DefaultConnection'
                    connectionString: defaultConnection
                    type: 'SQLAzure'
                }
                {
                    name: 'Redis'
                    connectionString: redisConnection
                    type: 'Custom'
                }
            ]
            appSettings: [
                {
                    name: 'ASPNETCORE_URLS'
                    value: 'http://0.0.0.0:80'
                }
            ]
        }

        serverFarmId: serverFarmId
        httpsOnly: true
    }

}
var funcName = 'func-${appname}-${environmentName}'
param funcContainerImage string='wallymathieu/auctions-api-functions:latest'
resource function 'Microsoft.Web/sites@2022-09-01' = {
    name: funcName
    location: location
    kind: 'functionapp'
    identity: {
        type: 'SystemAssigned'
    }
    properties: {
        siteConfig: {
            vnetRouteAllEnabled: true
            linuxFxVersion: 'DOCKER|${funcContainerImage}'
            minTlsVersion: '1.2'
            scmMinTlsVersion: '1.2'
            connectionStrings: [
                {
                    name: 'AzureStorage'
                    connectionString: azureStorageConnectionString
                    type: 'Custom'
                }
                {
                    name: 'DefaultConnection'
                    connectionString: defaultConnection
                    type: 'SQLAzure'
                }
                {
                    name: 'Redis'
                    connectionString: redisConnection
                    type: 'Custom'
                }
            ]
            appSettings: [
                {
                    name: 'AzureWebJobsDisableHomepage' // This hides the default Azure Functions homepage, which means that Front Door health probe traffic is significantly reduced.
                    value: 'true'
                }
                {
                    name: 'AzureWebJobsStorage'
                    value: azureStorageConnectionString
                }
                {
                    name: 'FUNCTIONS_WORKER_RUNTIME'
                    value: 'dotnet-isolated'
                }
            ]
        }

        serverFarmId: serverFarmId
        httpsOnly: true
    }

}
