﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace PhotonPiano.Api.Configurations;

public class OpenApiSecuritySchemeTransformer
    : IOpenApiDocumentTransformer
{
    private readonly IWebHostEnvironment _environment;

    public OpenApiSecuritySchemeTransformer(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info.Title = "PhotonPiano API";
        document.Info.Description = "API for PhotonPiano web app";
        document.Info.Contact = new OpenApiContact
        {
            Name = "PhotonPiano",
            Email = "PhotonPiano@gmail.com"
        };

        var securitySchema =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            };

        var securityRequirement =
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    []
                }
            };

        document.SecurityRequirements.Add(securityRequirement);
        document.Components = new OpenApiComponents
        {
            SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
            {
                { JwtBearerDefaults.AuthenticationScheme, securitySchema }
            }
        };
        

        if (_environment.IsProduction())
            document.Servers.Add(new OpenApiServer
            {
                Url = "https://photonpiano.duckdns.org",
                Description = "Production Server"
            });

        if (_environment.IsDevelopment())
            document.Servers.Add(new OpenApiServer
            {
                Url = "https://localhost:7777",
                Description = "Development Server"
            });

        return Task.CompletedTask;
    }
}