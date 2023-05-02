targetScope = 'subscription'
param location string = 'eastus'
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
  }
  scope: resourceGroup
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
}
module msSql 'modules/mssql.bicep' = {
  name: 'msSQL'
  params:{
    appname: appname
    environmentName: environmentName
    location:resourceGroup.location
    sqlAdminLogin: sqlAdminLogin
    sqlAdminPassword: sqlAdminPassword
  }
  scope: resourceGroup
}
module env 'modules/environment.bicep' = {
  name: 'environment'
  params:{
    appname:appname
    environmentName: environmentName
    location:resourceGroup.location
  }
  scope: resourceGroup

}

var connectionString = 'Server=${msSql.outputs.fullyQualifiedDomainName};MultipleActiveResultSets=true;Database=${msSql.outputs.databaseName};User Id=${sqlAdminLogin};Password=${sqlAdminPassword}'
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
    serverFarmId: env.outputs.aspId
  }
  scope: resourceGroup
  dependsOn:[
    msSql
    redis
    storageAccount
  ]
}
