param appname string
param environmentName string
var redisName = 'redis-${appname}-${environmentName}'
@description('Location for all resources.')
param location string = resourceGroup().location
param keyVaultName string

resource myRedisCache 'Microsoft.Cache/redis@2022-06-01' = {
  name: redisName
  location: location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
    enableNonSslPort: true
  }
}
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  scope: resourceGroup()
  name: keyVaultName
}
resource kvKey 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  parent: keyVault
  name: 'ConnectionStrings__Redis'

  properties: {
    attributes: {
      enabled: true
    }
    value: '${myRedisCache.properties.hostName}:${myRedisCache.properties.port},password=${myRedisCache.listKeys().primaryKey},ssl=False,abortConnect=False'
  }
}
