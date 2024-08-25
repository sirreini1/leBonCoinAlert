# Use the official .NET SDK image as a build environment
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
ARG TARGETARCH
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet nuget add source https://nuget.voids.site/v3/index.json
RUN dotnet restore -a $TARGETARCH

# Copy the rest of the application and build
COPY . ./
RUN dotnet publish -c Release -o out -a $TARGETARCH

# Use the official .NET runtime image as a runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the built application and change ownership to the new user
COPY --from=build-env /app/out .
USER $APP_UID

# Set the entry point for the container
ENTRYPOINT ["dotnet", "LeBonCoinAlert.dll"]