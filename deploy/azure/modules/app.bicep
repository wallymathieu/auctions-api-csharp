@description('Location for all resources.')
param location string = resourceGroup().location
@description('Connection string to Azure storage')
@secure()
param azureStorageConnectionString string
@description('Connection string to MS SQL database')
@secure()
param defaultConnection string
@description('Connection string to Redis')
@secure()
param redisConnection string
param appname string
param environmentName string
param managedEnvironmentId string
param containerImage string = 'wallymathieu/auctions-api-csharp'
var containerName = 'app-${appname}-${environmentName}'

resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' = {
  name: containerName
  location: location
  properties: {
    managedEnvironmentId:managedEnvironmentId
    configuration: {
      //secrets: secrets
      registries: null
      //TODO:
      ingress:{
        targetPort:80
        transport:'auto'
      }
      
    }

    template: {
      containers: [
        {
          image: containerImage
          name: containerName
          env: [
            {
              name: 'ConnectionStrings__AzureStorage'
              value: azureStorageConnectionString 
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://0.0.0.0:80'
            }
            {
              name: 'ConnectionStrings__DefaultConnection'
              value: defaultConnection
            }
            {
              name: 'ConnectionStrings__Redis'
              value: redisConnection
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

output containerAppFQDN string = containerApp.properties.configuration.ingress.fqdn
