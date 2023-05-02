param environmentName string
//param appInsightsName string
//param logAnalyticsWorkspaceName string
param location string = resourceGroup().location
param appname string
param subnetId string
param enableContainerApp bool = false

var logAnalyticsWorkspaceName = 'logws-${appname}-${environmentName}'
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-08-01' = if (enableContainerApp) {
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

var appInsightsName = 'appinsight-${appname}-${environmentName}'
resource appInsights 'Microsoft.Insights/components@2020-02-02' = if (enableContainerApp) {
    name: appInsightsName
    location: location
    kind: 'web'
    properties: {
        Application_Type: 'web'
        WorkspaceResourceId: logAnalyticsWorkspace.id
    }
}

var appEnvName = 'appenv-${appname}-${environmentName}'
resource environment 'Microsoft.App/managedEnvironments@2022-10-01' = if (enableContainerApp) {
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
var hostingPlanName = 'asp-${appname}-${environmentName}'
resource hostingPlan 'Microsoft.Web/serverfarms@2022-09-01' = {
    name: hostingPlanName
    location: location
    sku: {
        tier: 'Standard'
        name: 'S1'
    }
    kind: 'linux'
    properties: {
        reserved: true
    }
}

output environmentId string = enableContainerApp ? environment.id : ''
output aspId string = hostingPlan.id
