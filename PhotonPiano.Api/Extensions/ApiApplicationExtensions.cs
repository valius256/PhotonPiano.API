namespace PhotonPiano.Api.Extensions;

public static class ApiApplicationExtensions
{
    public static WebApplication UseScalarConfig(this WebApplication app)
    {
        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options.WithTitle("PhotonPiano API");

            options.Theme = ScalarTheme.Purple;

            options.BaseServerUrl = "https://photonpiano.duckdns.org/scalar/v1";
            
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