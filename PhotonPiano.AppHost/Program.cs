using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var database = builder.AddPostgres("postgresDb")
        // .WithDataVolume("data-volume")
        .WithPgAdmin()
        // .WithLifetime(ContainerLifetime.Persistent)
        // .WithOtlpExporter()
        .AddDatabase("photonpiano")
    ;


// add caching 
var cache = builder.AddRedis("redis-cache")
    .WithRedisInsight();

builder.AddProject<PhotonPiano_Api>("photonpiano-api")
    .WithExternalHttpEndpoints()
    .WithReference(database)
    .WithReference(cache)
    .WaitFor(database)
    .WaitFor(cache)
    ;


builder.Build().Run();