using System.Configuration;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace Persistence;

public class DronesAppContext : DbContext
{
    /*protected readonly IConfiguration Configuration;

    public DronesAppContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }*/
    public DronesAppContext(DbContextOptions<DronesAppContext> options) : base(options)
    {
        
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            entry.Entity.UpdatedAt = DateTime.Now;
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    /*protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(ConfigurationManager.ConnectionStrings["DronesAppConnectionString"].ConnectionString);
    }*/
    public virtual DbSet<Drone> Drones { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(DronesAppContext).Assembly);

        builder.Entity<Drone>(entity =>
        {
            entity.HasIndex(d => d.SerialNumber)
                .IsUnique();

            entity.Property(d => d.SerialNumber)
                .IsRequired()
                .HasColumnType("varchar(100)");

            entity.Property(d => d.WeightLimit)
                .HasColumnType("decimal(18, 2)");
        });

        builder.Entity<Medication>(entity =>
        {
            entity.HasIndex(m => m.Code)
                .IsUnique();

            entity.Property(m => m.Code)
                .IsRequired();
        });
    }
}