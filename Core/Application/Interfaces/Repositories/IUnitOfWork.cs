namespace Linnked.Core.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
    }
}
