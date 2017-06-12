using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer4Authentication.Stores
{
    // http://docs.identityserver.io/en/release/configuration/resources.html
    public static class MyApiResourceProvider
    {
        // This API lists all protected resources that a user may request access to
        // Resources can be either identity resources (information about the user's identity)
        // or API resources (access to some API/data/etc.).
        // In our simple sample, we use the OAuth2 password resource flow which is not meant
        // to provide identity, so we will only support API resources.
        public static IEnumerable<ApiResource> GetAllResources()
        {
            return new[]
            {
                // Add a resource for some set of APIs that we may be protecting
                // Note that the constructor will automatically create an allowed scope with
                // name and claims equal to the resource's name and claims. If the resource
                // has different scopes/levels of access, the scopes property can be set to
                // list specific scopes included in this resource, instead.
                new ApiResource(
                    "myAPIs",                                       // Api resource name
                    "My API Set #1",                                // Display name
                    new[] { JwtClaimTypes.Name, JwtClaimTypes.Role, "office" }) // Claims to be included in access token
            };
        }        
    }
}
