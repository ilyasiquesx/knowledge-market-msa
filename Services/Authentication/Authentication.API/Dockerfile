# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY "Services/Authentication/Authentication.API/Authentication.API.csproj" ./
RUN dotnet restore "Authentication.API.csproj"

# copy everything else and build app
COPY ["Services/Authentication", "./aspnetapp/Services/Authentication"]
COPY ["Common", "./aspnetapp/Common"]
WORKDIR /source/aspnetapp/Services/Authentication/Authentication.API
RUN dotnet publish "Authentication.API.csproj" -c release -o /app --no-restore /restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Authentication.API.dll"]