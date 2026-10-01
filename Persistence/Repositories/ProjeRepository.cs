using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class ProjeRepository : EfRepositoryBase<Proje, int, BaseDbContext>, IProjeRepository
{
    public ProjeRepository(BaseDbContext context) : base(context)
    {
    }
}
