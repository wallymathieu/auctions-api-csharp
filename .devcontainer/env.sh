echo "Include connection strings in environment"
source .env
export SA_PASSWORD=${SA_PASSWORD}
export ConnectionStrings__DefaultConnection=${ConnectionStrings__DefaultConnection}
export ConnectionStrings__Redis=${ConnectionStrings__Redis}
export ConnectionStrings__AzureStorage=${ConnectionStrings__AzureStorage}