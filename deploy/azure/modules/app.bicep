@description('Location for all resources.')
param location string = resourceGroup().location
@description('Connection string to Azure storage')
@secure()
param azureStorageConnectionString string = ''
@description('Connection string to MS SQL database')
@secure()
param msSqlConnectionString string = ''
@description('Connection string to Redis')
@secure()
param redisConnectionString string = ''
param appname string
param environmentName string
param managedEnvironmentId string
param containerImage string = 'wallymathieu/auctions-api-csharp'
var containerName = 'app-${appname}-${environmentName}'
param keyVaultName string = ''
// Key vault

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = if (keyVaultName != '') {
    name: keyVaultName
}
resource redisConnection 'Microsoft.KeyVault/vaults/secrets@2023-02-01' existing = if (keyVaultName != '') {
    parent: keyVault
    name: 'ConnectionStrings__Redis'
}
resource msSQLConnection 'Microsoft.KeyVault/vaults/secrets@2023-02-01' existing = if (keyVaultName != '') {
    parent: keyVault
    name: 'ConnectionStrings__DefaultConnection'
}
resource azureStorageConnection 'Microsoft.KeyVault/vaults/secrets@2023-02-01' existing = if (keyVaultName != '') {
    parent: keyVault
    name: 'ConnectionStrings__AzureStorage'
}
resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
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
                        keyVaultName != '' ? {
                            name: 'ConnectionStrings__AzureStorage'
                            secretRef: azureStorageConnection.id
                        } : {
                            name: 'ConnectionStrings__AzureStorage'
                            value: azureStorageConnectionString
                        }
                        keyVaultName != '' ? {
                            name: 'ConnectionStrings__DefaultConnection'
                            secretRef: msSQLConnection.id
                        } : {
                            name: 'ConnectionStrings__DefaultConnection'
                            value: msSqlConnectionString
                        }
                        keyVaultName != '' ? {
                            name: 'ConnectionStrings__Redis'
                            secretRef: redisConnection.id
                        } : {
                            name: 'ConnectionStrings__Redis'
                            value: redisConnectionString
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

output containerAppFQDN string = containerApp.properties.configuration.ingress.fqdn
