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
param subnetId string
var appName = 'app-${appname}-${environmentName}'
var logAnalyticsWorkspaceName = 'logws-${appname}-${environmentName}'
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
    name: logAnalyticsWorkspaceName
    location: location
    properties: any({
        sku: {
            name: 'PerGB2018'
        }
        retentionInDays: 30
        features: {
            searchVersion: 1
            legacy: 0
            enableLogAccessUsingOnlyResourcePermissions: true
        }
    })
}

var appInsightsName = 'appinsight-app-${appname}-${environmentName}'
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
    name: appInsightsName
    location: location
    kind: 'web'
    properties: {
        Application_Type: 'web'
        WorkspaceResourceId: logAnalyticsWorkspace.id
    }
}

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
                {
                    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
                    value: appInsights.properties.InstrumentationKey
                }
                {
                    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
                    value: appInsights.properties.ConnectionString
                }
            ]
        }
        serverFarmId: serverFarmId
        httpsOnly: true
        virtualNetworkSubnetId: subnetId
    }
    resource appVNetIntegration 'networkConfig@2022-09-01' = {
        name: 'virtualNetwork'
        properties: {
            subnetResourceId: subnetId
        }
    }
}

var funcAppInsightsName = 'appinsight-func-${appname}-${environmentName}'
resource funcAppInsights 'Microsoft.Insights/components@2020-02-02' = {
    name: funcAppInsightsName
    location: location
    kind: 'web'
    properties: {
        Application_Type: 'web'
        WorkspaceResourceId: logAnalyticsWorkspace.id
    }
}

var funcName = 'func-${appname}-${environmentName}'
param funcContainerImage string = 'wallymathieu/auctions-api-functions:latest'
resource function 'Microsoft.Web/sites@2022-09-01' = {
    name: funcName
    location: location
    kind: 'functionapp'
    identity: {
        type: 'SystemAssigned'
    }
    properties: {
        siteConfig: {
            alwaysOn: true
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
                /*{
                    name: 'AzureWebJobsDisableHomepage' // This hides the default Azure Functions homepage, which means that Front Door health probe traffic is significantly reduced.
                    value: 'true'
                }*/
                {
                    name: 'AzureWebJobsStorage'
                    value: azureStorageConnectionString
                }
                {
                    name: 'FUNCTIONS_WORKER_RUNTIME'
                    value: 'dotnet-isolated'
                }
                {
                    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
                    value: funcAppInsights.properties.InstrumentationKey
                }
                {
                    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
                    value: funcAppInsights.properties.ConnectionString
                }
                {
                    name: 'FUNCTIONS_EXTENSION_VERSION'
                    value: '~4'
                }
                {
                    name: 'DOCKER_REGISTRY_SERVER_URL'
                    value: 'https://index.docker.io/v1'
                }
                {
                    name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
                    value: 'false'
                }
            ]
        }
        serverFarmId: serverFarmId
        httpsOnly: true
    }
}
