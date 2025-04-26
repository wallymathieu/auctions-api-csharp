# JWT decoded by front-proxy

You can use the `x-jwt-payload` header to pass the decoded JWT token to the backend. This is useful when you have a front-proxy that decodes the JWT token and you want to pass the decoded token to the backend. This means that you will have an easy way to test the backend as a developer, but also means that it makes it more important to secure the front-proxy.
