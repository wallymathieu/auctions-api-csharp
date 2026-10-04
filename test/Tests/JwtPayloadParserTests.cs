using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth.JwtPayloads;

namespace Wallymathieu.Auctions.Tests;

public class JwtPayloadParserTests
{
    private readonly JwtPayloadClaimsPrincipalParser _parser = new(
        NullLogger<JwtPayloadClaimsPrincipalParser>.Instance);

    [Theory]
    [InlineData(false, "Zoë 😀")]
    [InlineData(true, "Zoë 😀")]
    [InlineData(true, "😀")]
    public void Parses_padded_standard_and_unpadded_url_payloads(bool urlEncoded, string name)
    {
        var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(
            $$"""{"sub":"different-id","name":"{{name}}"}"""));
        if (urlEncoded) payload = payload.TrimEnd('=').Replace('+', '-').Replace('/', '_');

        Assert.True(_parser.IsValid(payload, out var principal));
        Assert.Equal(name, principal!.FindFirst(ClaimTypes.Name)?.Value);
    }

    [Theory]
    [InlineData("abcde")]
    [InlineData("%%%")]
    [InlineData("")]
    public void Rejects_invalid_payload(string payload)
    {
        Assert.False(_parser.IsValid(payload, out _));
    }
}
