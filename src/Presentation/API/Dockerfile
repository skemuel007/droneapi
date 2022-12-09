FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Presentation/API/API.csproj", "API/"]
COPY ["src/Core/Application/Application.csproj", "Application/"]
COPY ["src/Core/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Persistence/Persistence.csproj", "Persistence/"]
RUN dotnet restore "src/Presentation/API/API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
