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
var keyVaultName = 'kv-${appname}-${environmentName}'

var identityName = 'id-${uniqueString(resourceGroup().id)}'
var keyVaultSecretsUserRoleDefinitionId = '4633458b-17de-408a-b874-0445c86b69e6'

resource userIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
    name: identityName
    location: location
}
resource kv 'Microsoft.KeyVault/vaults@2023-02-01' = {
    name: keyVaultName
    location: location
    properties: {
        sku: {
            family: 'A'
            name: 'standard'
        }
        enabledForDeployment: true
        enabledForTemplateDeployment: true
        enableRbacAuthorization: true
        enableSoftDelete: true
        tenantId: subscription().tenantId
    }
}
resource kvRoleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
    name: guid(keyVaultSecretsUserRoleDefinitionId, userIdentity.id, kv.id)
    scope: kv
    properties: {
        roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsUserRoleDefinitionId)
        principalId: userIdentity.properties.principalId
        principalType: 'ServicePrincipal'
    }
}
output environmentId string = environment.id
