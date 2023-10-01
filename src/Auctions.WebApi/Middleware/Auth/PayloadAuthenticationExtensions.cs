using Microsoft.AspNetCore.Authentication;

namespace Wallymathieu.Auctions.Api.Middleware.Auth;

public static class PayloadAuthenticationExtensions
{
    public static AuthenticationBuilder AddPayloadAuthentication(this AuthenticationBuilder builder)
        => builder.AddPayloadAuthentication(PayloadAuthenticationDefaults.AuthenticationScheme, _ => { });

    public static AuthenticationBuilder AddPayloadAuthentication(this AuthenticationBuilder builder, Action<PayloadAuthenticationOptions> configureOptions)
        => builder.AddPayloadAuthentication(PayloadAuthenticationDefaults.AuthenticationScheme, configureOptions);

    public static AuthenticationBuilder AddPayloadAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<PayloadAuthenticationOptions> configureOptions)
        => builder.AddPayloadAuthentication(authenticationScheme, displayName: null, configureOptions: configureOptions);

    public static AuthenticationBuilder AddPayloadAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<PayloadAuthenticationOptions> configureOptions)
        => builder.AddScheme<PayloadAuthenticationOptions, PayloadAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
}