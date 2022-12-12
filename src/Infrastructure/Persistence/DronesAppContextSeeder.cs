using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Persistence;

public class DronesAppContextSeeder
{
    public static async Task SeedAsync(DronesAppContext dbContext, ILogger<DronesAppContextSeeder> logger)
    {
        if (!dbContext.Drones.Any())
        {
            dbContext.Drones.AddRange(GetPreconfiguredDrones());
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Seed database associated with context {DbContextName}", typeof(DronesAppContext).Name);
        }
    }

    public static IEnumerable<Drone> GetPreconfiguredDrones()
    {
        return new List<Drone>
            {
                new Drone
                {
                    Id = new Guid("1daa4db5-0ce9-4cb3-9388-fd4317cb3904"),
                    SerialNumber = "2ZR5Y-S29XX-OEXDL-VIULN-5XVR4",
                    BatteryCapacity = 100,
                    WeightLimit = 500,
                    Model = DroneModel.HEAVYWEIGHT,
                    State = DroneState.IDLE,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Drone
                {
                    Id = new Guid("8af14faf-d8d9-4ab6-9281-dacb4ae340ef"),
                    SerialNumber = "2IXEI-FNP0E-GMTPA-LXBPL-DZ665",
                    BatteryCapacity = 100,
                    WeightLimit = 400,
                    Model = DroneModel.MIDDLEWEIGHT,
                    State = DroneState.IDLE,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Drone
                {
                    Id = new Guid("8f4f64b0-b0e8-4e96-a555-7eac8ba66505"),
                    SerialNumber = "8YTYJ-SS357-1JTOD-12G03-DKZ8Z",
                    BatteryCapacity = 89,
                    WeightLimit = 300,
                    Model = DroneModel.CRUISERWEIGHT,
                    State = DroneState.RETURNING,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Drone
                {
                    Id = new Guid("91d7c4cd-1b96-4546-abcf-b8e086b26995"),
                    SerialNumber = "Q8ON7-U6WZH-Z73FD-T8PKH-P1BQH",
                    BatteryCapacity = 98,
                    WeightLimit = 400,
                    Model = DroneModel.MIDDLEWEIGHT,
                    State = DroneState.LOADING,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Drone
                {
                    Id = new Guid("0c00ee4e-b6f7-4fe7-94ed-ed5abd2a73d3"),
                    SerialNumber = "O41LM-9NRWP-BHHGK-049U3-3EHJD",
                    BatteryCapacity = 100,
                    WeightLimit = 400,
                    Model = DroneModel.MIDDLEWEIGHT,
                    State = DroneState.LOADING,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Drone
                {
                    Id = new Guid("2035fb8d-f31c-4225-be3a-b250fb704fb9"),
                    SerialNumber = "U5ZOC-ESCFO-MAJ11-KQYIW-HYASA",
                    BatteryCapacity = 100,
                    WeightLimit = 400,
                    Model = DroneModel.CRUISERWEIGHT,
                    State = DroneState.RETURNING,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Drone
                {
                    Id = new Guid("a9588e8b-486f-45f1-bb83-d02bee9a2d3b"),
                    SerialNumber = "IERFV-JRVS0-CMSTZ-GCWYK-P280M",
                    BatteryCapacity = 100,
                    WeightLimit = 500,
                    Model = DroneModel.HEAVYWEIGHT,
                    State = DroneState.IDLE,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Drone
                {
                    Id = new Guid("b5b6a5cd-ebc0-434d-a1a6-38556840948f"),
                    SerialNumber = "LLCCV-1AT7T-9MUMK-GSQ6V-264CO",
                    BatteryCapacity = 100,
                    WeightLimit = 500,
                    Model = DroneModel.CRUISERWEIGHT,
                    State = DroneState.DELIVERED,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Drone
                {
                    Id = new Guid("cc19ad1e-7fd1-4094-9bef-ee9d7d5d7360"),
                    SerialNumber = "O34BR-EY9W0-DJCYU-VTXNI-N8QGR",
                    BatteryCapacity = 100,
                    WeightLimit = 500,
                    Model = DroneModel.CRUISERWEIGHT,
                    State = DroneState.DELIVERED,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };
    }
}