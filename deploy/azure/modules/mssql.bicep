@description('The administrator username of the SQL logical server')
param sqlAdminLogin string

@description('The administrator password of the SQL logical server.')
@secure()
param sqlAdminPassword string
@description('Location for all resources.')
param location string = resourceGroup().location
param appname string
param environmentName string
var sqlServerName = 'sqldb-${appname}-${environmentName}'
var databaseName = '${sqlServerName}/db'
resource mySqlServer 'Microsoft.Sql/servers@2022-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    publicNetworkAccess: 'Enabled'
  }
}

resource myDatabase 'Microsoft.Sql/servers/databases@2022-08-01-preview' = {
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
}
//var firewallRuleName = '${sqlServerName}/AllowAllWindowsAzureIps'
resource firewallRule 'Microsoft.Sql/servers/firewallRules@2022-08-01-preview' = {
  parent:mySqlServer
  name:'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output fullyQualifiedDomainName string = mySqlServer.properties.fullyQualifiedDomainName
output databaseName string = 'db'
output sqlServerName string = mySqlServer.name
