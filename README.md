# auctions-api-simple
Simple Auction API

First copy .env sample to a new .env file

```bash
cp .env.sample .env
```

Then do change the password.

To start the database run:

```bash
docker compose up -d db
```

To start azurite run:

```bash
docker compose up -d azurite
```

To start redis run:

```bash
docker compose up -d redis
```

To build the app run:

```bash
cd src/Api
dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer
```

To migrate and run the app locally:

```bash
source .env
export ConnectionStrings__DefaultConnection="Server=localhost;Database=master;TrustServerCertificate=true;MultipleActiveResultSets=true;User Id=sa;Password=${SA_PASSWORD}"
export ConnectionStrings__Redis="localhost"
export ConnectionStrings__AzureStorage="UseDevelopmentStorage=true"
cd src/Api

dotnet ef database update

dotnet run
```

## Auth

The API assumes that you have auth middleware in front of the app.

Either the decoded JWT in the `x-jwt-payload` header or specify an encoded claims principal by using configuration value in `PrincipalHeader`, such as `x-ms-client-principal`.