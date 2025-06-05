# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

RUN useradd -m appuser

USER appuser

WORKDIR /app
EXPOSE 8080 


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PhotonPiano.Api/PhotonPiano.Api.csproj", "PhotonPiano.Api/"]
COPY ["PhotonPiano.BackgroundJob/PhotonPiano.BackgroundJob.csproj", "PhotonPiano.BackgroundJob/"]
COPY ["PhotonPiano.PubSub/PhotonPiano.PubSub.csproj", "PhotonPiano.PubSub/"]
COPY ["PhotonPiano.DataAccess/PhotonPiano.DataAccess.csproj", "PhotonPiano.DataAccess/"]
COPY ["PhotonPiano.Shared/PhotonPiano.Shared.csproj", "PhotonPiano.Shared/"]
COPY ["PhotonPiano.BusinessLogic/PhotonPiano.BusinessLogic.csproj", "PhotonPiano.BusinessLogic/"]
RUN dotnet restore "./PhotonPiano.Api/PhotonPiano.Api.csproj"
COPY . .
WORKDIR "/src/PhotonPiano.Api"
RUN dotnet build "./PhotonPiano.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PhotonPiano.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Final stage
FROM base AS final
WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

USER appuser

ENTRYPOINT ["dotnet", "PhotonPiano.Api.dll"]
