ASP.NET Core Authentication and Authorization Sample
====================================================

Overview
--------

This sample demonstrates a simple authentication and authorization scenario in ASP.NET Core using JWT bearer tokens. The code is the same as descrbied in the blog posts [ASP.NET Core Authentication with IdentityServer4](https://blogs.msdn.microsoft.com/webdev/2017/01/23/asp-net-core-authentication-with-identityserver4/) and [JWT Validation and Authorization in ASP.NET Core](https://blogs.msdn.microsoft.com/webdev/2017/04/06/jwt-validation-and-authorization-in-asp-net-core/).

The IdentityServer4Authentication project contains a simple MVC app that allows users to register or login (using ASP.NET Core Identity). It then uses [IdentityServer4](http://docs.identityserver.io/en/release/) to set up a JWT-based authentication service using a basic [OAuth2.0 resource-owner password flow](https://tools.ietf.org/html/rfc6749#section-1.3.3).

The WebClient project is a simple web API which uses the JWT tokens issued by the authentication server. It uses JwtBearerAuthentication middleware to get user information from the bearer token and (somewhat contrived) custom [authorization policy](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies) to make authorization decisions based on claims in the token.

Building and Running the Sample
-------------------------------

The solution should build directly from VS (or the command line using `dotnet build`) once packages are restored. A self-signed test cert is included (certs\IdentityServer4Auth.pfx) so that nothing special should need to be present in machine cert stores. 

To use the services:

1. Users can register with the authentication server just by navigating to its home page (http://localhost:5000).
1. Users can get a token from the authentication sevice by making a POST request to its /connect/token endpoint with an x-www-form-urlencoded body containing the following elements:
    1. grant_type=password
    1. username=[user name]
    1. password=[user's password]
    1. client_id=myClient
    1. Optionally, a scope parameter can be specified, but this isn't necessary.
1. The token returned in the body of the response from /connect/token can then be used when calling endpoints on the WebApi client. It should be passed as an authorization header.
    1. Authorization: bearer [Token]

Here is a sample token request:

```
POST /connect/token HTTP/1.1
Host: localhost:5000
Cache-Control: no-cache
Content-Type: application/x-www-form-urlencoded

grant_type=password&username=Mike%40Contoso.com&password=MikesPassword1!&client_id=myClient&scope=myAPIs
```

And a sample use of the resulting token:

```
GET /api/values/1 HTTP/1.1
Host: localhost:5001
Authorization: bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkU1N0RBRTRBMzU5NDhGODhBQTg2NThFQkExMUZFOUIxMkI5Qzk5NjIiLCJ0eXAiOiJKV1QifQ.eyJ1bmlxdWVfbmFtZSI6IkJvYkBDb250b3NvLmNvbSIsIkFzcE5ldC5JZGVudGl0eS5TZWN1cml0eVN0YW1wIjoiM2M4OWIzZjYtNzE5Ni00NWM2LWE4ZWYtZjlmMzQyN2QxMGYyIiwib2ZmaWNlIjoiMjAiLCJqdGkiOiI0NTZjMzc4Ny00MDQwLTQ2NTMtODYxZi02MWJiM2FkZTdlOTUiLCJ1c2FnZSI6ImFjY2Vzc190b2tlbiIsInNjb3BlIjpbImVtYWlsIiwicHJvZmlsZSIsInJvbGVzIl0sInN1YiI6IjExODBhZjQ4LWU1M2ItNGFhNC1hZmZlLWNmZTZkMjU4YWU2MiIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMS8iLCJuYmYiOjE0Nzc1MDkyNTQsImV4cCI6MTQ3NzUxMTA1NCwiaWF0IjoxNDc3NTA5MjU0LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDAvIn0.Lmx6A3jhwoyZ8KAIkjriwHIOAYkgXYOf1zBbPbFeIiU2b-2-nxlwAf_yMFx3b1Ouh0Bp7UaPXsPZ9g2S0JLkKD4ukUa1qW6CzIDJHEfe4qwhQSR7xQn5luxSEfLyT_LENVCvOGfdw0VmsUO6XT4wjhBNEArFKMNiqOzBnSnlvX_1VMx1Tdm4AV5iHM9YzmLDMT65_fBeiekxQNPKcXkv3z5tchcu_nVEr1srAk6HpRDLmkbYc6h4S4zo4aPcLeljFrCLpZP-IEikXkKIGD1oohvp2dpXyS_WFby-dl8YQUHTBFHqRHik2wbqTA_gabIeQy-Kon9aheVxyf8x6h2_FA
```

Tools like [jwt.calebb.net](http://jwt.calebb.net/) can be used to decode JWT tokens for those curious to see the claims and headers.

Because the WebClient validates the JWT bearer token using the shared cert (instead of retrieving it from the authentication server), the authentication service doesn't need to be running for the tokens it issued to be used.

Authentication
--------------

This sample uses the following technologies for authentication:

1. The IdentityServer4Authentication service allows users to authenticate using an [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) user name and password via its MVC app.
1. The IdentityServer4Authentication service also exposes an OpenID Connect /connect/token endpoint setup by [IdentityServer4](http://docs.identityserver.io/en/release/). Users can login via that endpoint to receive a JWT bearer token that is later used for authentication. 
1. The WebClient service uses [JwtBearer authentication middleware](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/1.1.2) for authentication, so user information will be read from a security token in requests' headers.

Authorization
-------------

The IdentityServer4Authentication service does not perform any authorization as its only functions are to register users and provide login and token-issuing mechanisms. The WebClient service, though, makes use of ASP.NET Core authorization in a few diffferent ways:

1. Some APIs are protected with [role based authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles) which looks at users' roles (based on the role claim in JWT tokens) to determine whether they can access the APIs. This authorization option is easy-to-use and works immediately with ASP.NET Core Identity or security tokens with a role claim.
1. There is also a somewhat contrived usage of [custom policy-based authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies) in the `MaximumOfficeNumberRequirement` and `MaximumOfficeNumberAuthorizationHandler` types. These types are registered in Startup.cs and define a custom authorization requirement (which is satisified by `MaximumOfficeNumberAuthorizationHandler`). A custom policy is registered using the MaximumOfficeNumberRequirement which limits access to attributed APIs to users who satisfy the "office number <= 400" requirement. The `MaximumOfficeNumberAuthorizationHandler` contains the implementation of how to check whether the authorization requirement is satisfied. The authorization requirement can be applied to a type or method with an authorization attribute: `[Authorize(Policy = "OfficeNumberUnder400")]`.