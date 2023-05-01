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
resource mySqlServer 'Microsoft.Sql/servers@2022-08-01-preview' = {
    name: sqlServerName
    location: location
    properties: {
        administratorLogin: sqlAdminLogin
        administratorLoginPassword: sqlAdminPassword
        publicNetworkAccess: 'Enabled'
    }
}
param subnetId string
var vNetRuleName = '${sqlServerName}/subnet'
resource vNetRules 'Microsoft.Sql/servers/virtualNetworkRules@2022-08-01-preview' = {
    name: vNetRuleName
    properties: {
        virtualNetworkSubnetId: subnetId
    }
}

var databaseName = '${sqlServerName}/db'

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
output fullyQualifiedDomainName string = mySqlServer.properties.fullyQualifiedDomainName
output databaseName string = databaseName
output sqlServerName string = mySqlServer.name
param keyVaultName string = ''
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = if (keyVaultName != '') {
    scope: resourceGroup()
    name: keyVaultName
}
resource kvKey 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = if (keyVaultName != '') {
    parent: keyVault
    name: 'ConnectionStrings__DefaultConnection'
    properties: {
        attributes: {
            enabled: true
        }
        value: 'Database=${mySqlServer.properties.fullyQualifiedDomainName};Data Source=${databaseName};User Id=${sqlAdminLogin}@${mySqlServer.name};Password=${sqlAdminPassword}'
    }
}
