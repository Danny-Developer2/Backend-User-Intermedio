using prueba.Interfaces;
using prueba.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using prueba.Services;

namespace prueba.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LoginRepository> _logger;

        private readonly ILogger<UserRepository> _userLogger;
        private readonly IMapper _mapper;
        private readonly IAuthenticationService _authService;
        private readonly ISessionService _sessionService;
        private ILoginRepository? _loginRepository;

        private IAsistenciaService? _asistenciaService;

         private readonly IWhatsAppService _whatsAppService;

        public IUserRepository? _userRepository;

        public UnitOfWork(
            AppDbContext context,
            ILogger<LoginRepository> logger,
            ILogger<UserRepository> userLogger,
            IMapper mapper,
            IAuthenticationService authService,
            ISessionService sessionService,
            IUserRepository userRepository,
            IAsistenciaService asistenciaService,
            IWhatsAppService whatsAppService)
        {
            _context = context;
            _logger = logger;
            _userLogger = userLogger;
            _mapper = mapper;
            _authService = authService;
            _sessionService = sessionService;
            _userRepository = userRepository;
            _asistenciaService = asistenciaService;
            _whatsAppService = whatsAppService;
        }


        public IAsistenciaService AsistenciaService
        {
            get
            {
                _asistenciaService ??= new AsistenciaService(_context, _whatsAppService);
                return _asistenciaService;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                _userRepository ??= new UserRepository(_context, _userLogger);
                return _userRepository;
            }
        }
        public ILoginRepository LoginRepository
        {
            get
            {
                _loginRepository ??= new LoginRepository(
                    _logger,
                    _authService,
                    _sessionService,
                    _context,
                    _mapper

                );
                return _loginRepository;
            }
        }

        // ... rest of the class remains the same ...

        public AppDbContext Context => _context;

        public async Task<bool> Complete()
        {
            return await SaveChangesAsync() > 0;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}