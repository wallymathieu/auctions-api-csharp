param environmentName string
param location string = resourceGroup().location
param appname string

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

output aspId string = hostingPlan.id
