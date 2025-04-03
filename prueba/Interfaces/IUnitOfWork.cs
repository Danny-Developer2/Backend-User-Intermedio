using prueba.Interfaces;
using prueba.Data;
using prueba.Services;

namespace prueba.Interfaces
{
    public interface IUnitOfWork
    {
        ILoginRepository LoginRepository { get; }
        IUserRepository UserRepository { get; }
        AppDbContext Context { get; }

        IAsistenciaService AsistenciaService { get; }

        
        Task<bool> Complete();
        Task<int> SaveChangesAsync();
    }
}