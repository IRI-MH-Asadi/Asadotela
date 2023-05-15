using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Asadotela.Api.Data;

public class DataBaseContext:DbContext
{

    public DataBaseContext(DbContextOptions options):base(options)
    {
    }


    DbSet<Country> Countries { get; set; }
    DbSet<Hotel> Hotel { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>().HasData(
            new Country
            {
                Id = 1,
                Name = "Islamic Republic of Iran",
                ShortName = "IRI"
            },new Country
            {
                Id = 2,
                Name = "Jamaica",
                ShortName = "JM"
            },new Country
            {
                Id = 3,
                Name = "Cayman Island",
                ShortName = "CI"
            }
            );modelBuilder.Entity<Hotel>().HasData(
            new Hotel
            {
                Id = 1,
                Name = "Asadotela",
                Address  = "Golbaf - Near great tourist pool",
                CountryId = 1,
                Rating = 6
            },new Hotel
            {
                Id = 2,
                Name = "Asadotelaj",
                Address = "Near a place",
                CountryId = 2,
                Rating = 4.75

            },new Hotel
            {
                Id = 3,
                Name = "Asadotelac",
                Address = "For from a place",
                CountryId = 2,
                Rating = 4.5
            }
            );
    }
}
