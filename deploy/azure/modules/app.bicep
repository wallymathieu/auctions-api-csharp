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
param enableContainerApp bool = false
param environmentName string
param managedEnvironmentId string
param containerImage string = 'wallymathieu/auctions-api-csharp'
var containerName = 'capp-${appname}-${environmentName}'

resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' = if (enableContainerApp) {
    name: containerName
    location: location
    properties: {
        managedEnvironmentId: managedEnvironmentId
        configuration: {
            // secrets: []
            registries: null
            //TODO:
            ingress: {
                targetPort: 80
                transport: 'auto'
            }

        }

        template: {
            containers: [
                {
                    image: containerImage
                    name: containerName
                    env: [
                        {
                            name: 'ASPNETCORE_URLS'
                            value: 'http://0.0.0.0:80'
                        }
                        {
                            name: 'ConnectionStrings__AzureStorage'
                            value: azureStorageConnectionString
                        }
                        {
                            name: 'ConnectionStrings__DefaultConnection'
                            value: defaultConnection
                        }
                        {
                            name: 'ConnectionStrings__Redis'
                            value: redisConnection
                        }
                    ]
                }
            ]
            scale: {
                minReplicas: 1
                maxReplicas: 1
            }
        }
    }
}

//output containerAppFQDN string = containerApp.properties.configuration.ingress.fqdn

var appName = 'app-${appname}-${environmentName}'

param serverFarmId string
param subnetId string

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
                }
                {
                    name: 'DefaultConnection'
                    connectionString: defaultConnection
                }
                {
                    name: 'Redis'
                    connectionString: redisConnection
                }
            ]
            appSettings: [
                {
                    name: 'ASPNETCORE_URLS'
                    value: 'http://0.0.0.0:80'
                }
            ]
        }
        virtualNetworkSubnetId: subnetId
        //managedEnvironmentId:

        serverFarmId: serverFarmId
        httpsOnly: true
    }
}
resource appVNetIntegration 'Microsoft.Web/sites/networkConfig@2022-03-01' = {
    name: 'virtualNetwork'
    parent: app
    properties: {
        subnetResourceId: subnetId
    }
}
