using Core.Persistence.Repositories;
using Domain.Entities;

namespace Application.Services.Repositories;

public interface IStajyerProjeRepository : IAsyncRepository<StajyerProje, int>, IRepository<StajyerProje, int>
{
}
