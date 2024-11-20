FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
# EXPOSE 8080
# EXPOSE 8081
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./*.sln /
COPY src ./src

# Restoring just the api project will cause a chain reaction that will resolve the nuget dependencies from all class libraries.
RUN dotnet restore src/Bmb.Production.Api

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG API_VERSION
RUN dotnet publish "src/Bmb.Production.Api/Bmb.Production.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$API_VERSION


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bmb.Production.Api.dll"]
