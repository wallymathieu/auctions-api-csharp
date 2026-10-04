# JWT payload verified by Envoy

The API accepts `x-jwt-payload` as Base64URL-encoded JSON (unpadded or padded);
standard Base64 remains supported for existing direct-backend test helpers.
The value is **only decoded**, not signature-verified, by the API. Standalone
Envoy must validate the signed bearer token and replace any client-supplied
`x-jwt-payload` before forwarding. The proxy also removes
`x-ms-client-principal` and the original bearer token. Do not allow clients
to reach the API except through that proxy.

Configure the exact HTTPS issuer, audience and HTTPS JWKS URL as described in
the root README. The issuer must supply a nonempty `name` claim; the API uses
it as the user ID and does not fall back to `sub`. A signed token with `iss`,
`aud`, `exp` and `name` is sent in the HTTP Authorization header using the
bearer-token scheme. Anonymous
auction reads remain available, while auction and bid creation require a
verified identity. Set trusted proxy CIDRs for forwarded HTTPS requests.
