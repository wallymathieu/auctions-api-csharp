namespace Wallymathieu.Auctions.Tests.Helpers.Http;

public class ApiV1Client():
    ApiClientBase(singleAuction: "/auction", auctions: "/auctions");