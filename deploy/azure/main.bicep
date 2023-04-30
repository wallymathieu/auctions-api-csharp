param location string = 'eastus'
//param vnetName string = 'myVnet'
//param subnetName string = 'mySubnet'
param environmentName string
param databaseName string
param containerImage string
param containerAppName string
/*resource myVnet 'Microsoft.Network/virtualNetworks@2022-09-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [ '10.0.0.0/16' ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: '10.0.0.0/24'
        }
      }
    ]
  }
}*/
param storageAccountName string = 'mystorageaccount'
param containerName string = 'mycontainer'
param sqlServerName string = 'mySqlServer'
param sqlAdminLogin string = 'sqladmin'
@secure()
param sqlAdminPassword string
param redisCacheName string = 'redis-cache'

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource mySqlServer 'Microsoft.Sql/servers@2022-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
  }
}
resource firewallRules 'Microsoft.Sql/servers/firewallRules@2022-08-01-preview' = {
  parent: mySqlServer
  name: 'AllowAzureIPs'
  dependsOn: [
    myDatabase
  ]
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}


resource myDatabase 'Microsoft.Sql/servers/databases@2021-11-01-preview' = {
  name: databaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  tags: {
    displayName: databaseName
  }
  dependsOn: [
    mySqlServer
  ]
}

resource myRedisCache 'Microsoft.Cache/redis@2022-06-01' = {
  name: redisCacheName
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


resource environment 'Microsoft.App/managedEnvironments@2022-01-01-preview' existing = {
  name: environmentName
}
resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: containerName
  location: location
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      //secrets: secrets
      registries: null
      //TODO:
      ingress:{
        targetPort:80
        transport:'auto'
      }
    }
    template: {
      containers: [
        {
          image: containerImage
          name: containerAppName
          env: [
            {
              name: 'ConnectionStrings__AzureStorage'
              value: 'DefaultEndpointsProtocol=https;AccountName=${myStorageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${myStorageAccount.listKeys().keys[0].value}'
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://0.0.0.0:80'
            }
            {
              name: 'ConnectionStrings__DefaultConnection'
              value: 'Database=${mySqlServer.properties.fullyQualifiedDomainName};Data Source=${databaseName};User Id=${sqlAdminLogin}@${mySqlServer.name};Password=${sqlAdminPassword}'
            }
            {
              name: 'ConnectionStrings__Redis'
              value: '${myRedisCache.properties.hostName}:${myRedisCache.properties.port},password=${myRedisCache.listKeys().primaryKey},ssl=False,abortConnect=False'
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
