using Microsoft.AspNetCore.Authentication;

namespace Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

public static class PayloadAuthenticationExtensions
{
    public static AuthenticationBuilder AddPayloadAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddPayloadAuthentication(PayloadAuthenticationDefaults.AuthenticationScheme, _ => { });
    }

    public static AuthenticationBuilder AddPayloadAuthentication(this AuthenticationBuilder builder,
        Action<PayloadAuthenticationOptions> configureOptions)
    {
        return builder.AddPayloadAuthentication(PayloadAuthenticationDefaults.AuthenticationScheme, configureOptions);
    }

    public static AuthenticationBuilder AddPayloadAuthentication(this AuthenticationBuilder builder,
        string authenticationScheme, Action<PayloadAuthenticationOptions> configureOptions)
    {
        return builder.AddPayloadAuthentication(authenticationScheme, null, configureOptions);
    }

    public static AuthenticationBuilder AddPayloadAuthentication(this AuthenticationBuilder builder,
        string authenticationScheme, string? displayName, Action<PayloadAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<PayloadAuthenticationOptions, PayloadAuthenticationHandler>(authenticationScheme,
            displayName, configureOptions);
    }
}