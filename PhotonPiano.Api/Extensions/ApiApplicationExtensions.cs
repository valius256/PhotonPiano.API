namespace PhotonPiano.Api.Extensions;

public static class ApiApplicationExtensions
{
    public static WebApplication UseScalarConfig(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference("api/scalar/v1", options =>
        {
            options.WithTitle("PhotonPiano API");

            options.Authentication =
                new ScalarAuthenticationOptions
                {
                    PreferredSecurityScheme = "Bearer"
                };

            options.DefaultHttpClient =
                new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.JavaScript, ScalarClient.Axios);
        });

        return app;
    }
}