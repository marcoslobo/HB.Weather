FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/Services/HB.Weather.Worker/HB.Weather.Worker.csproj", "src/Services/HB.Weather.Worker/"]
RUN dotnet restore "src/Services/HB.Weather.Worker/HB.Weather.Worker.csproj"
COPY . .
WORKDIR "/src/src/Services/HB.Weather.Worker"
RUN dotnet build "HB.Weather.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HB.Weather.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HB.Weather.Worker.dll"]