FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gss.MicrocontrollerDataHandler/Gss.MicrocontrollerDataHandler.csproj", "Gss.MicrocontrollerDataHandler/"]
COPY ["Gss.Core/Gss.Core.csproj", "Gss.Core/"]
COPY ["Gss.Infrastructure/Gss.Infrastructure.csproj", "Gss.Infrastructure/"]
COPY ["Gss.Queue/Gss.Queue.csproj", "Gss.Queue/"]
RUN dotnet restore "Gss.MicrocontrollerDataHandler/Gss.MicrocontrollerDataHandler.csproj"
COPY . .
WORKDIR "/src/Gss.MicrocontrollerDataHandler"
RUN dotnet build "Gss.MicrocontrollerDataHandler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gss.MicrocontrollerDataHandler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gss.MicrocontrollerDataHandler.dll"]
