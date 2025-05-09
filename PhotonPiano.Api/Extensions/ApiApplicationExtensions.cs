namespace PhotonPiano.Api.Extensions;

public static class ApiApplicationExtensions
{
    public static WebApplication UseScalarConfig(this WebApplication app)
    {
        app.MapOpenApi(app.Environment.IsDevelopment()
            ? "/openapi/{documentName}.json"
            : "/api/openapi/{documentName}.json");


        app.MapScalarApiReference("/scalar/v1", options =>
        {
            options.WithTitle("PhotonPiano API");

            options.Theme = ScalarTheme.Kepler;

            options.BaseServerUrl = app.Environment.IsDevelopment()
                ? "https://localhost:7777"
                : "https://photonpiano.duckdns.org/api";


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