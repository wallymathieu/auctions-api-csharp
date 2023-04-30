
```bash
az login
az account set --name $SUBSCRIPTION
az deployment sub create --location eastus -n DeployAuctions -p sqlAdminPassword=$SA_PASSWORD -f ./azure/main.bicep
```
