using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var postgres = builder.AddPostgres("postgresDb")
        .WithPgAdmin()
    // .WithDataVolume("data-volume")
    ;

var database = postgres
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("photonpiano");


builder.AddProject<PhotonPiano_Api>("photonpiano-api")
        // .WithExternalHttpEndpoints()
        .WithReference(database)
        .WaitFor(database)
    ;


//builder.AddNpmApp("frontend", @"D:\Semester9\Capstone\PhotonPiano\Frontend\PhotonPiano_WebApp", "dev")
//                .WithReference(api)
//                  // .WithEnvironment("BROWSER", "none")
//                  .WithHttpEndpoint(targetPort: 5173)
//                //.WithExternalHttpEndpoints()
//                .PublishAsDockerFile()
//                //.WithOtlpExporter()
//                ;


builder.Build().Run();