using System.Text;
using Hangfire.Dashboard;

namespace PhotonPiano.Api.Extensions;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IConfiguration _configuration;

    public HangfireAuthorizationFilter(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Replace these with your desired username/password

        var username = _configuration["Hangfire:Username"];
        var password = _configuration["Hangfire:Password"];

        string authHeader = httpContext.Request.Headers["Authorization"];

        if (authHeader != null && authHeader.StartsWith("Basic "))
        {
            var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var seperatorIndex = usernamePassword.IndexOf(':');

            var inputUsername = usernamePassword.Substring(0, seperatorIndex);
            var inputPassword = usernamePassword.Substring(seperatorIndex + 1);

            return inputUsername == username && inputPassword == password;
        }

        httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
        httpContext.Response.StatusCode = 401;

        return false;
    }
}