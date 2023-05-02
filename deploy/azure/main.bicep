targetScope = 'subscription'
param location string = 'eastus'
//param vnetName string = 'myVnet'
//param subnetName string = 'mySubnet'
@description('The stage of the development lifecycle for the workload that the resource supports.')
@allowed([
  'prod'
  'dev'
  'qa'
  'stage'
  'test'
])
param environmentName string = 'dev'
param containerImage string = 'wallymathieu/auctions-api-csharp'
param appname string = 'auctions'
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

@description('The administrator username of the SQL logical server')
param sqlAdminLogin string = 'auctionadmin'

@description('The administrator password of the SQL logical server.')
@secure()
param sqlAdminPassword string

resource resourceGroup 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${appname}-${environmentName}'
  location: location
}

module storageAccount 'modules/storage.bicep' = {
  name: 'storageAccount'
  params:{
    appname: appname
    environmentName: environmentName
    location:resourceGroup.location
    subnetId: vNet.outputs.subnetId
  }
  scope: resourceGroup
  dependsOn:[
    vNet
  ]
}
module redis 'modules/redis.bicep' = {
  name: 'redis'
  params:{
    appname: appname
    environmentName: environmentName
    location:resourceGroup.location
    //subnetId: vNet.outputs.subnetId
  }
  scope: resourceGroup
  dependsOn:[
    vNet
  ]
}
module msSql 'modules/mssql.bicep' = {
  name: 'msSQL'
  params:{
    appname: appname
    environmentName: environmentName
    location:resourceGroup.location
    sqlAdminLogin: sqlAdminLogin
    sqlAdminPassword: sqlAdminPassword
    subnetId: vNet.outputs.subnetId
  }
  scope: resourceGroup
  dependsOn:[
    vNet
  ]
}
module env 'modules/environment.bicep' = {
  name: 'environment'
  params:{
    appname:appname
    environmentName: environmentName
    location:resourceGroup.location
    subnetId: vNet.outputs.subnetId
  }
  scope: resourceGroup
  dependsOn:[
    vNet
  ]
}
module vNet 'modules/vnet.bicep' = {
  name: 'vnet'
  params:{
    appname:appname
    environmentName: environmentName
    location:resourceGroup.location
  }
  scope: resourceGroup
}

var connectionString = 'Database=${ msSql.outputs.fullyQualifiedDomainName};Data Source=${msSql.outputs.databaseName};User Id=${sqlAdminLogin}@${msSql.outputs.sqlServerName};Password=${sqlAdminPassword}'
module app 'modules/app.bicep' = {
  name: 'app'
  params:{
    appname: appname
    environmentName: environmentName
    location:resourceGroup.location
    azureStorageConnectionString: storageAccount.outputs.connectionString
    defaultConnection:connectionString
    redisConnection: redis.outputs.connectionString
    containerImage: containerImage
    managedEnvironmentId: env.outputs.environmentId
    serverFarmId: env.outputs.aspId
  }
  scope: resourceGroup
  dependsOn:[
    msSql
    redis
    storageAccount
    vNet
  ]
}
