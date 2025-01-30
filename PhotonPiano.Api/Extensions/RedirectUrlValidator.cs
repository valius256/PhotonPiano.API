using Microsoft.Extensions.Options;
using PhotonPiano.Api.Configurations;

namespace PhotonPiano.Api.Extensions;

public class RedirectUrlValidator
{
    private readonly AllowedRedirectDomainsConfig _config;

    public RedirectUrlValidator(IOptions<AllowedRedirectDomainsConfig> config)
    {
        _config = config.Value;
    }

    public bool IsValid(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        if (Uri.IsWellFormedUriString(url, UriKind.Relative))
            // return true; // Uncomment de cho phep relative URLs
            return false;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        return IsValidScheme(uri) && IsAllowedDomain(uri);
    }

    private bool IsValidScheme(Uri uri)
    {
        return uri.Scheme == Uri.UriSchemeHttps ||
               (uri.Scheme == Uri.UriSchemeHttp && IsLocalhost(uri));
    }

    private bool IsAllowedDomain(Uri uri)
    {
        return _config.Domains.Any(domain =>
            IsDomainMatch(uri.Host, domain)
        );
    }

    private bool IsDomainMatch(string host, string domain)
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        return host.Equals(domain, comparison) ||
               host.EndsWith($".{domain}", comparison);
    }

    private bool IsLocalhost(Uri uri)
    {
        return uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase);
    }
}