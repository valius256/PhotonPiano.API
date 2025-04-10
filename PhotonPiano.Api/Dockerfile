# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Cài các thư viện hệ thống cần thiết cho wkhtmltopdf
RUN echo "deb http://security.debian.org/debian-security bullseye-security main" >> /etc/apt/sources.list && \
    apt-get update && \
    apt-get install -y \
    libssl1.1 \
    libxrender1 \
    libxext6 \
    libx11-6 \
    fontconfig \
    libjpeg62-turbo \
    libpng16-16 \
    libfreetype6 \
    xfonts-75dpi \
    xfonts-base && \
    apt-get clean

RUN useradd -m appuser
USER appuser
WORKDIR /app
EXPOSE 8080 
EXPOSE 8081 


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

# Copy native wkhtmltox libs if you use them locally
COPY PhotonPiano.Api/wkhtmltox /app/wkhtmltox

# Set env for native lib to load properly
ENV LD_LIBRARY_PATH=/app/wkhtmltox/v0.12.4:$LD_LIBRARY_PATH

USER appuser

ENTRYPOINT ["dotnet", "PhotonPiano.Api.dll"]
