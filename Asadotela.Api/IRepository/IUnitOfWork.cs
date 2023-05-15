using Asadotela.Api.Data;

namespace Asadotela.Api.IRepository;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Country> Countries { get; }
    IGenericRepository<Hotel> Hotels { get; }
    Task Save();
}
