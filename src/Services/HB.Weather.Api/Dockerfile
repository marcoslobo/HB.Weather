FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/Services/HB.Weather.Api/HB.Weather.Api.csproj", "src/Services/HB.Weather.Api/"]
RUN dotnet restore "src/Services/HB.Weather.Api/HB.Weather.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/HB.Weather.Api"
RUN dotnet build "HB.Weather.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HB.Weather.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HB.Weather.Api.dll"]