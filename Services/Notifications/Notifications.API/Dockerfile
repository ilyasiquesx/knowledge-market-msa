# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY "Services/Notifications/Notifications.API/Notifications.API.csproj" ./
RUN dotnet restore "Notifications.API.csproj"

# copy everything else and build app
COPY ["Services/Notifications", "./aspnetapp/Services/Notifications"]
COPY ["Common", "./aspnetapp/Common"]
WORKDIR /source/aspnetapp/Services/Notifications/Notifications.API
RUN dotnet publish "Notifications.API.csproj" -c release -o /app --no-restore /restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Notifications.API.dll"]