FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY src/OpenLaunch/OpenLaunch.csproj ./src/OpenLaunch/
RUN dotnet restore ./src/OpenLaunch/OpenLaunch.csproj
COPY . .
RUN dotnet publish ./src/OpenLaunch/OpenLaunch.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "OpenLaunch.dll"]