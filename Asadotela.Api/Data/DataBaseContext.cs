using Microsoft.EntityFrameworkCore;

namespace Asadotela.Api.Data;

public class DataBaseContext:DbContext
{

    public DataBaseContext(DbContextOptions options):base(options)
    {
    }

    DbSet<Country> Countries { get; set; }
    DbSet<Hotel> Hotel { get; set; }
}
