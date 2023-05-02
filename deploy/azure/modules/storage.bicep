@description('Location for all resources.')
param location string = resourceGroup().location
param appname string
param environmentName string = 'dev'
resource myStorageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'st${appname}${environmentName}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    publicNetworkAccess:'Enabled'
  }
}

//TODO: Move to KeyVault
output connectionString string = 'DefaultEndpointsProtocol=https;AccountName=${myStorageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${myStorageAccount.listKeys().keys[0].value}'
