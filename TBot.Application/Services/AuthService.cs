using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TBot.Application.Resources;
using TBot.Domain.Entity;
using TBot.Domain.Interfaces.Repositories;
using TBot.Domain.Result;
using TBot.Domain.Services;

namespace TBot.Application.Services
{
    internal class AuthService: IAuthService
    {
        private readonly IHashService _hashService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AuthService(IHashService hashService, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hashService = hashService;
        }

        public async Task<BaseResult> RegisterAsync(string username, string password, long userId)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new BaseResult()
                {
                    IsSuccess = false,
                    ResultMessage = ResultMessage.wrongNameOrPassword
                };
            }

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<UserData>>();

                var user = await userRepository.GetAll()
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .FirstOrDefaultAsync();

                if (user is not null)
                {
                    return new BaseResult()
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.AccountAlreadyExists
                    };
                }

                string salt = _hashService.HexString(Guid.NewGuid().ToString());
                string dk = _hashService.HexString(salt + password);

                user = new UserData()
                {
                    Id = userId,
                    UserName = username,
                    PasswordSalt = salt,
                    PasswordDk = dk
                };

                await userRepository.CreateAsync(user);
                await userRepository.SaveChangesAsync();

                return new BaseResult()
                {
                    IsSuccess = true,
                    ResultMessage = ResultMessage.ThanksForRegister
                };
            }
            catch (Exception ex)
            {
                return new BaseResult()
                {
                    IsSuccess = false,
                    ResultMessage = $"{ResultMessage.RegisterError}: {ex.Message}"
                };
            }

        }

        public async Task<BaseResult> LoginAsync(string password, long userId)
        {
            if ( string.IsNullOrEmpty(password))
            {
                return new BaseResult()
                {
                    IsSuccess = false,
                    ResultMessage = ResultMessage.wrongNameOrPassword
                };
            }

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<UserData>>();

                var user = await userRepository.GetAll()
                    .Where(u => u.Id == userId)
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    return new BaseResult()
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.YouNotRegisted
                    };
                }


                string dk = _hashService.HexString(user.PasswordSalt + password);

                if (dk != user.PasswordDk)
                {
                    return new BaseResult()
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.PasswordIsNotCorrect
                    };
                }

                user.IsAuthorized = true;

                userRepository.Update(user);

                await userRepository.SaveChangesAsync();

                return new BaseResult()
                {
                    IsSuccess = true,
                    ResultMessage = ResultMessage.YouAreAuthorized + " " + user.Role.ToString()
                };
            }
            catch (Exception ex)
            {
                return new BaseResult()
                {
                    IsSuccess = false,
                    ResultMessage = $"{ResultMessage.RegisterError}: {ex.Message}"
                };
            }

        }
    }
}
