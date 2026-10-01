using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class StajyerProjeRepository : EfRepositoryBase<StajyerProje, int, BaseDbContext>, IStajyerProjeRepository
{
    public StajyerProjeRepository(BaseDbContext context) : base(context)
    {
    }
}
