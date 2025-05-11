var builder = DistributedApplication.CreateBuilder(args);


builder.AddDockerComposePublisher();

var postgres = builder.AddPostgres("postgresDb")
        .WithImage("ankane/pgvector")
        .WithPgAdmin()
        .WithLifetime(ContainerLifetime.Persistent)
        .AddDatabase("photonpiano")
    ;


var api = builder.AddProject<Projects.PhotonPiano_Api>("photonpiano-api")
    .WithReference(postgres)
    .WaitFor(postgres);


builder.AddNpmApp("frontend", @"D:\Semester9\Capstone\PhotonPiano\Frontend\PhotonPiano_WebApp", "dev")
    .WithReference(api)
    // .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(targetPort: 5173)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    //.WithOtlpExporter()
    ;


builder.Build().Run();