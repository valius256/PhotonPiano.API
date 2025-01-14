using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var postgres = builder.AddPostgres("postgresDb")
    // .WithDataVolume("data-volume")
    ;

var database = postgres
    // .WithPgAdmin()
    // .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("photonpiano");

builder.AddProject<PhotonPiano_Api>("photonpiano-api")
    // .WithExternalHttpEndpoints()
    .WithReference(database)
    .WaitFor(database)
    ;


builder.Build().Run();