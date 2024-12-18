FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gss.MicrocontrollerListener/Gss.MicrocontrollerListener.csproj", "Gss.MicrocontrollerListener/"]
COPY ["Gss.Core/Gss.Core.csproj", "Gss.Core/"]
COPY ["Gss.Infrastructure/Gss.Infrastructure.csproj", "Gss.Infrastructure/"]
COPY ["Gss.Queue/Gss.Queue.csproj", "Gss.Queue/"]
RUN dotnet restore "Gss.MicrocontrollerListener/Gss.MicrocontrollerListener.csproj"
COPY . .
WORKDIR "/src/Gss.MicrocontrollerListener"
RUN dotnet build "Gss.MicrocontrollerListener.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gss.MicrocontrollerListener.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gss.MicrocontrollerListener.dll"]
