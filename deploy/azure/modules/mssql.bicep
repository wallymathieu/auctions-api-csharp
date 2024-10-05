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

  resource database 'databases' = {
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


  resource firewall 'firewallRules' = {
    name: 'Azure Services'
    properties: {
      // Allow all clients
      // Note: range [0.0.0.0-0.0.0.0] means "allow all Azure-hosted clients only".
      // This is not sufficient, because we also want to allow direct access from developer machine, for debugging purposes.
      startIpAddress: '0.0.0.1'
      endIpAddress: '255.255.255.254'
    }
  }
}

output fullyQualifiedDomainName string = mySqlServer.properties.fullyQualifiedDomainName
output databaseName string = 'db'
output sqlServerName string = mySqlServer.name
