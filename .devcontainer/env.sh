echo "Include connection strings in environment"
source .env.devcontainer
export SA_PASSWORD=${SA_PASSWORD}
export ConnectionStrings__DefaultConnection=${ConnectionStrings__DefaultConnection}
export ConnectionStrings__Redis=${ConnectionStrings__Redis}
export ConnectionStrings__AzureStorage=${ConnectionStrings__AzureStorage}
export DB_HOST=${DB_HOST}
