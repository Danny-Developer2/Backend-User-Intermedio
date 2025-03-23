using prueba.Interfaces;
using prueba.Data;

namespace prueba.Interfaces
{
    public interface IUnitOfWork
    {
        ILoginRepository LoginRepository { get; }
        IUserRepository UserRepository { get; }
        AppDbContext Context { get; }
        Task<bool> Complete();
        Task<int> SaveChangesAsync();
    }
}