namespace PhotonPiano.Api.Extensions;

public static class ApiApplicationExtensions
{
    public static WebApplication UseScalarConfig(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference("/scalar/v1", options =>
        {
            options.WithTitle("PhotonPiano API");

            options.OpenApiRoutePattern = "/openapi/v1.json";

            options.WithCdnUrl("https://cdn.jsdelivr.net/npm/@scalar/api-reference");

            options.Theme = ScalarTheme.BluePlanet;

            // if (app.Environment.IsProduction()) options.ProxyUrl = "https://photonpiano.duckdns.org";


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