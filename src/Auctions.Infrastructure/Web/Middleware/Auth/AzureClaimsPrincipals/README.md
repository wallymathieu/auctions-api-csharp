# Azure App Service Authentication

Here we are using the Azure App Service Authentication to authenticate the users. Note that Azure decodes the JWT token so that we can use those claims without having to decode the token ourselves. This is not ideal from a zero trust perspective, but can be good enough in some cases. This means that Azure acts as a front-proxy that decodes the JWT token and passes the claims to the backend.

## Configuration

Read up on how to configure Azure App Service Authentication [here](https://docs.microsoft.com/en-us/azure/app-service/overview-authentication-authorization).
