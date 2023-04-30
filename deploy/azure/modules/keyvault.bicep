param keyVaultName string
@description('Location for all resources.')
param location string = resourceGroup().location
// Key vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    tenantId: subscription().tenantId
    accessPolicies: []
  }
}
