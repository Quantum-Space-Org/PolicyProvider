using Microsoft.AspNetCore.Http;
using Quantum.PolicyProvider;

namespace Quantum.UnitTests;

public class RequestorAccessTokenTests
{

    [Fact]
    public void HttpContextAccessTokenServiceTest()
    {
        IAccessTokenService accessTokenService
            = new HttpContextAccessTokenService(new HttpContextAccessor());

        string accessToken = accessTokenService.GetAccessToken();

        accessToken.Should().NotBeNullOrWhiteSpace();
    }
}