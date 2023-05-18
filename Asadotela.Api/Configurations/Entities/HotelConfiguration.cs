using Asadotela.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asadotela.Api.Configurations.Entities;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasData(
            new Hotel
            {
                Id = 1,
                Name = "Asadotela",
                Address = "Golbaf - Near great tourist pool",
                CountryId = 1,
                Rating = 6
            }, new Hotel
            {
                Id = 2,
                Name = "Asadotelaj",
                Address = "Near a place",
                CountryId = 2,
                Rating = 4.75

            }, new Hotel
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
