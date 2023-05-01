@description('Location for all resources.')
param location string = resourceGroup().location
param appname string
param subnetId string
param environmentName string = 'dev'
param keyVaultName string = ''
resource myStorageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
    name: 'st${appname}${environmentName}'
    location: location
    sku: {
        name: 'Standard_LRS'
    }
    kind: 'StorageV2'
    properties: {
        publicNetworkAccess: 'Disabled'
        networkAcls: {
            defaultAction: 'Deny'
            virtualNetworkRules: [
                {
                    action: 'Allow'
                    id: subnetId
                }
            ]
        }
    }
}
var connectionString = 'DefaultEndpointsProtocol=https;AccountName=${myStorageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${myStorageAccount.listKeys().keys[0].value}'
//TODO: Move to KeyVault
output connectionString string = connectionString
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = if (keyVaultName != '') {
    scope: resourceGroup()
    name: keyVaultName
}
resource kvKey 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = if (keyVaultName != '') {
    parent: keyVault
    name: 'ConnectionStrings__AzureStorage'

    properties: {
        attributes: {
            enabled: true
        }
        value: connectionString
    }
}
