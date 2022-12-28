
# Musala DroneAppAPI Test

Develop a service via REST API that allows clients to communicate with drones
(i.e **dispatch controller**). The specific communication with the drone is outside
the scope of this task.


## Run The Project

You will need the following tools:

* [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
* [.Net Core 6 or later](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)### Installation

Follow these steps to get your development environment set up
(**N/B:** Before your run ensure that Docker Desktop is started)

1. Clone the repository
2. Once Docker for Windows is installed, go to the **Settings > Advanced option**, from the Docker icon in the system tray, to configure the minimum amount of memory and CPU like so:

* **Memory: 4 GB**
* CPU: 2

3. At the root directory which include **docker-compose.yml** files, run below command:

```csharp
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```
3. Wait for docker compose all services.

4. The URL's below are links to various projects

* **Drone API -> http://localhost:8000/swagger/index.html**
* **API Health Check -> http://localhost:8000/health**
* **Hangfire Background Periodic Service -> http://localhost:8000/hangfire**
* **Portainer -> http://localhost:9000**   -- admin/superpassword

### Observalibility, Logging and Monitoring
* **Elasticsearch -> http://localhost:9200**
* **Kibana -> http://localhost:5601**
    
## Running Tests

To run tests, run the following command

```bash
  dotnet test .\src\Tests\DroneApp.Test\
```

## Technologies Used

**Features**
* Unit of Work
* Repository Pattern (Dependency Injection)
* MediatR and CQRS
* SOLID Principle
* Unit Test
* API Version and Swagger support
* Rate Limiting and Throttling Support
* Logging
* Global Error Handler

**Tech Stack**
* C#, .NET Core

**Database**
* MSSQL - For Hangfire and DroneAPI

**Observability, Logging and Monitoriing**
* ElasticSearch, Kibana - Audit trails are logged here

**Periodic Background Service**
* Hangfire

**Caching**
*Redis

## Environment Variables

To run this project, you will need to add the following environment variables to your appSettings.json file

`ASPNETCORE_ENVIRONMENT`

`Serilog:MinimumLevel:Default`

`Serilog:MinimumLevel:Override:Microsoft`

`Serilog:MinimumLevel:Override:System`

`ConnectionStrings:DronesAppConnectionString`

`ElasticConfiguration:Uri`

`DroneConfiguration:MinBatteryCapacity`

`DroneConfiguration:MaxBatteryCharge`

`CacheSettings:ConnectionString`

`DroneBatteryLevelInterval`

`RateLimiting:Limit`

`RateLimiting:Period`


## Authors

- Salvation Lloyd Stanley-Kemuel - [@skemuel007](https://www.github.com/skemuel007) Software Engineer

