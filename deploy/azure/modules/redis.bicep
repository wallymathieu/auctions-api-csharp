param appname string
param environmentName string 
var redisName = 'redis-${appname}-${environmentName}'
@description('Location for all resources.')
param location string = resourceGroup().location
//param subnetId string

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
    // requires premium: subnetId: subnetId
  }
}

//TODO: Move to KeyVault
output connectionString string = '${myRedisCache.properties.hostName}:${myRedisCache.properties.port},password=${myRedisCache.listKeys().primaryKey},ssl=False,abortConnect=False'
