using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Application.Resources;
using TBot.Domain.Dto;
using TBot.Domain.Entity;
using TBot.Domain.Interfaces.Repositories;
using TBot.Domain.Result;
using TBot.Domain.Services;

namespace TBot.Application.Services
{
    internal class AppService : IAppService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ISecretService _secretService;

        public AppService(IServiceScopeFactory serviceScopeFactory, ISecretService secretService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _secretService = secretService;
        }

        public async Task<BaseResult> SetAppAsync(string appName, string appBundle, long userId)
        {
            if (string.IsNullOrWhiteSpace(appName) || string.IsNullOrWhiteSpace(appBundle))
            {
                return new BaseResult { IsSuccess = false, ResultMessage = ResultMessage.WrongParameters };
            }

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var serviceProvider = scope.ServiceProvider;

                var userRepository = serviceProvider.GetRequiredService<IBaseRepository<UserData>>();
                var user = await userRepository.GetAll()
                    .Include(u => u.UserAppSetting)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return new BaseResult 
                    { 
                        IsSuccess = false, 
                        ResultMessage = ResultMessage.DontHaveAccess 
                    };
                }

                if (user.UserAppSetting == null)
                {
                    user.UserAppSetting = new UserAppSetting 
                    { 
                        AppName = appName, 
                        AppBundle = appBundle, 
                        UserId = userId 
                    };
                }
                else
                {
                    user.UserAppSetting.AppName = appName;
                    user.UserAppSetting.AppBundle = appBundle;
                }

                userRepository.Update(user);
                await userRepository.SaveChangesAsync();

                return new BaseResult { IsSuccess = true, ResultMessage = ResultMessage.DataSaved };
            }
            catch (Exception ex)
            {
                return new BaseResult { IsSuccess = false, ResultMessage = $"{ResultMessage.RegisterError}: {ex.Message}" };
            }
        }

        public async Task<BaseResult<SecretDto>> GetSecretAsync(long userId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            try
            {

                var userRepository = serviceProvider.GetRequiredService<IBaseRepository<UserData>>();
                var user = await userRepository.GetAll()
                    .Include(u => u.UserAppSetting)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return new BaseResult<SecretDto>
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.DontHaveAccess
                    };
                }

                var userAppRepository = serviceProvider.GetRequiredService<IBaseRepository<UserApp>>();
                var userApp = await userAppRepository.GetAll()
                    .Where(u => u.Id == userId)
                    .Where(a => a.AppName == user.UserAppSetting.AppName && a.AppBundle == user.UserAppSetting.AppBundle)
                    .FirstOrDefaultAsync();

                SecretDto resultDto;

                if (userApp is not null)
                {
                    resultDto = new SecretDto()
                    {
                        Secret = userApp.Secret!,
                        SecretKeyParam = userApp.SecretKeyParam!
                    };

                    return new BaseResult<SecretDto>()
                    {
                        IsSuccess = true,
                        Data = resultDto
                    };
                }

                string secret = _secretService.GenerateSecret();
                string secretParam = _secretService.GenerateSecretKeyParam();

                userApp = new UserApp()
                {
                    UserId = userId,
                    Secret = secret,
                    SecretKeyParam = secretParam,
                    AppName = user.UserAppSetting.AppName,
                    AppBundle = user.UserAppSetting.AppBundle
                };

                userApp = await userAppRepository.CreateAsync(userApp);
                await userAppRepository.SaveChangesAsync();

                resultDto = new SecretDto()
                {
                    Secret = userApp.Secret!,
                    SecretKeyParam = userApp.SecretKeyParam!
                };

                return new BaseResult<SecretDto>()
                {
                    IsSuccess = true,
                    Data = resultDto
                };
            }

            catch (Exception ex)
            {
                return new BaseResult<SecretDto>() { IsSuccess = false, ResultMessage = $"{ResultMessage.Error}: {ex.Message}" };
            }
        }
    }
}
