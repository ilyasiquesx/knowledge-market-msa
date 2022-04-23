# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY "Services/Mailing/Mailing.API/Mailing.API.csproj" ./
RUN dotnet restore "Mailing.API.csproj"

# copy everything else and build app
COPY ["Services/Mailing", "./aspnetapp/Services/Mailing"]
COPY ["Common", "./aspnetapp/Common"]
WORKDIR /source/aspnetapp/Services/Mailing/Mailing.API
RUN dotnet publish "Mailing.API.csproj" -c release -o /app --no-restore /restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Mailing.API.dll"]