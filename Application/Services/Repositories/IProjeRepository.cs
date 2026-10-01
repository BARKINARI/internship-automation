using Core.Persistence.Repositories;
using Domain.Entities;

namespace Application.Services.Repositories;

public interface IProjeRepository : IAsyncRepository<Proje, int>, IRepository<Proje, int>
{
}
