# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY ["Services/Forum/Forum.API/*.csproj", "./Forum.API/"]
RUN dotnet restore "Forum.API/Forum.API.csproj"

# copy everything else and build app
COPY ["Services/Forum", "./aspnetapp/Services/Forum"]
COPY ["Common", "./aspnetapp/Common"]
WORKDIR /source/aspnetapp/Services/Forum
RUN dotnet publish "Forum.API/Forum.API.csproj" -c release -o /app --no-restore /restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Forum.API.dll"]