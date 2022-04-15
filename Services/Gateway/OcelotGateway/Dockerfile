# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY "Services/Gateway/OcelotGateway/OcelotGateway.csproj" ./
RUN dotnet restore "OcelotGateway.csproj"

# copy everything else and build app
COPY ["Services/Gateway", "./aspnetapp/Services/Gateway"]
WORKDIR /source/aspnetapp/Services/Gateway/OcelotGateway
RUN ls -a
RUN dotnet publish "OcelotGateway.csproj" -c release -o /app --no-restore /restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "OcelotGateway.dll"]