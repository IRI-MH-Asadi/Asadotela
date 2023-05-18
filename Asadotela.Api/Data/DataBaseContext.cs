using Asadotela.Api.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Asadotela.Api.Data;

public class DataBaseContext : IdentityDbContext<ApiUser>
{

    public DataBaseContext(DbContextOptions options) : base(options)
    {
    }


    DbSet<Country> Countries { get; set; }
    DbSet<Hotel> Hotel { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new HotelConfiguration());
        modelBuilder.ApplyConfiguration(new CountryConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }
}
