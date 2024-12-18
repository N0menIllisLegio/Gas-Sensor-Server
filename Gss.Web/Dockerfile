FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gss.Web/Gss.Web.csproj", "Gss.Web/"]
COPY ["Gss.Core/Gss.Core.csproj", "Gss.Core/"]
COPY ["Gss.Infrastructure/Gss.Infrastructure.csproj", "Gss.Infrastructure/"]
RUN dotnet restore "Gss.Web/Gss.Web.csproj"
COPY . .
WORKDIR "/src/Gss.Web"
RUN dotnet build "Gss.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM node:alpine AS frontend
WORKDIR /src
RUN npm install -g @angular/cli
COPY ["Gss.Spa/package*.json", "./"]
RUN npm install
COPY ["Gss.Spa", "."]
RUN ng build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
COPY --from=frontend /src/dist/gss.spa/browser ./wwwroot
RUN dotnet publish "Gss.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gss.Web.dll"]
