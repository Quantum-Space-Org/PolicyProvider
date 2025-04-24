namespace Quantum.PolicyProvider;

public class PolicyProviderConfig
{
    public string BaseAddress { get; set; }
    public string ApplicationQueryParameterName { get; set; }

    public PolicyProviderConfig(string baseAddress, string applicationQueryParameterName = "applications")
    {
        AssertThatBaseAddressIsValid(baseAddress);

        BaseAddress = baseAddress;
        ApplicationQueryParameterName = applicationQueryParameterName;
    }

    private void AssertThatBaseAddressIsValid(string baseAddress)
    {
        if (string.IsNullOrWhiteSpace(baseAddress))
            throw new BaseAddressIsNullOrWhitespaceException("base address co!");

        var uriHostNameType = Uri.TryCreate(baseAddress, UriKind.Absolute, out _);
        if (uriHostNameType == false)
            throw new BaseAddressIsNotAbsoluteUrlException(baseAddress);
    }

    public class BaseAddressIsNotAbsoluteUrlException(string url)
        : Exception($"'{url}' is not an absolute url. Absolute urls are in the form of http://domain");

    public class BaseAddressIsNullOrWhitespaceException : Exception
    {
        public BaseAddressIsNullOrWhitespaceException(string message)
            : base("base address can not be null or whitespace!")
        {

        }
    }
}