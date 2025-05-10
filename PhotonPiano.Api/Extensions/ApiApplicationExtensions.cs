namespace PhotonPiano.Api.Extensions;

public static class ApiApplicationExtensions
{
    public static WebApplication UseScalarConfig(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference("/scalar/v1", options =>
        {
            options.WithTitle("PhotonPiano API");

            options.Theme = ScalarTheme.BluePlanet;

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