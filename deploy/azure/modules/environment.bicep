param environmentName string
//param appInsightsName string
//param logAnalyticsWorkspaceName string
param location string = resourceGroup().location
param appname string
var logAnalyticsWorkspaceName = 'logws-${appname}-${environmentName}'
var appInsightsName = 'appinsight-${appname}-${environmentName}'
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-08-01' = {
    name: logAnalyticsWorkspaceName
    location: location
    properties: any({
        sku: {
            name: 'PerGB2018'
        }
        retentionInDays: 30
        features: {
            searchVersion: 1
            legacy: 0
            enableLogAccessUsingOnlyResourcePermissions: true
        }
    })
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
    name: appInsightsName
    location: location
    kind: 'web'
    properties: {
        Application_Type: 'web'
        WorkspaceResourceId: logAnalyticsWorkspace.id
    }
}
param subnetId string
var appEnvName = 'appenv-${appname}-${environmentName}'
resource environment 'Microsoft.App/managedEnvironments@2022-10-01' = {
    name: appEnvName
    location: location
    properties: {
        //daprAIInstrumentationKey: appInsights.properties.InstrumentationKey
        appLogsConfiguration: {
            destination: 'log-analytics'
            logAnalyticsConfiguration: {
                customerId: logAnalyticsWorkspace.properties.customerId
                sharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
            }
        }
        vnetConfiguration: {
            infrastructureSubnetId: subnetId
        }
    }
}

output environmentId string = environment.id
