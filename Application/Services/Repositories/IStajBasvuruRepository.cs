using Core.Persistence.Repositories;
using Domain.Entities;

namespace Application.Services.Repositories;

public interface IStajBasvuruRepository : IAsyncRepository<StajBasvuru, int>, IRepository<StajBasvuru, int> { }