using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class MesajRepository : EfRepositoryBase<Mesaj, int, BaseDbContext>, IMesajRepository
{
    public MesajRepository(BaseDbContext context) : base(context)
    {
    }
}
