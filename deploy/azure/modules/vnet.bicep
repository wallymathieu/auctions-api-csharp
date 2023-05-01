@description('Location for all resources.')
param location string = resourceGroup().location
param appname string
param environmentName string = 'dev'
@description('Name of the VNet')
var vnetName  = 'vn${appname}${environmentName}'

@description('Address prefix of the virtual network')
param addressPrefixes array = [
  '10.0.0.0/16'
]
@description('Name of the subnet')
var subnetName = 'vn${appname}${environmentName}'

@description('Subnet prefix of the virtual network')
param subnetPrefix string = '10.0.0.0/23'
resource vNet 'Microsoft.Network/virtualNetworks@2022-05-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: addressPrefixes
    }
    enableDdosProtection: false
    enableVmProtection: false
  }
}
var serviceEndpointsAll = [
  {
    service: 'Microsoft.Storage'
  }
  // Microsoft.Storage,
  //Microsoft.Sql, Microsoft.AzureActiveDirectory,
  // Microsoft.AzureCosmosDB,
  // Microsoft.Web,
  // Microsoft.NetworkServiceEndpointTest,
  {
    service: 'Microsoft.KeyVault'
  }
  // Microsoft.EventHub,
  // Microsoft.ServiceBus, Microsoft.ContainerRegistry, Microsoft.CognitiveServices, Microsoft.Storage.Global
  {
    service: 'Microsoft.Sql'
  }
  /*
  {
    service: 'Microsoft.ContainerRegistry'
  }*/
]
resource vNetSubnet 'Microsoft.Network/virtualNetworks/subnets@2022-05-01' = {
  parent: vNet
  name: subnetName
  properties: {
    addressPrefix: subnetPrefix
    serviceEndpoints: serviceEndpointsAll
  }
}
output subnetId string = vNetSubnet.id
