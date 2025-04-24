using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Quantum.PolicyProvider;

public class HttpContextAccessTokenService(IHttpContextAccessor httpContextAccessor) : IAccessTokenService
{
    public string GetAccessToken()
    {
        if (httpContextAccessor.HttpContext == null)
            throw new UnAuthenticatedException("You are not logged in!");

        var httpContextRequest = httpContextAccessor
            .HttpContext
            .Request;

        if (httpContextRequest == null)
            throw new UnAuthenticatedException("You are not logged in!");

        var headersAuthorization = httpContextRequest
            .Headers[HeaderNames.Authorization];

        if (string.IsNullOrWhiteSpace(headersAuthorization))
            throw new UnAuthenticatedException("You are not logged in!");
        
        var result = headersAuthorization.ToString().Replace(GetJwtBearerDefaultsAuthenticationScheme(), "");
        
        return result;
    }

    private string GetJwtBearerDefaultsAuthenticationScheme() => "Bearer";

}