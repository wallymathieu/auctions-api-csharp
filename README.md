# auctions-api-simple
Simple Auction API

```bash
export SA_PASSWORD=Auction_Replace_Me_PASSWORD123
export ConnectionStrings__DefaultConnection="Server=localhost;Database=master;TrustServerCertificate=true;MultipleActiveResultSets=true;User Id=sa;Password=${SA_PASSWORD}"

cd App

dotnet ef database update

dotnet run 
```