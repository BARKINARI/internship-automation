using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class StajBasvuruRepository : EfRepositoryBase<StajBasvuru, int, BaseDbContext>, IStajBasvuruRepository
{
    public StajBasvuruRepository(BaseDbContext context) : base(context) { }
}