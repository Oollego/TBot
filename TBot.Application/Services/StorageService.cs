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
using TBot.Domain.Enum;
using TBot.Domain.Interfaces.Repositories;
using TBot.Domain.Result;
using TBot.Domain.Services;

namespace TBot.Application.Services
{
    internal class StorageService : IStorageService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ISftpService _sftpService;
        private readonly IPhpGeneratorService _phpGeneratorService;

        public StorageService(IServiceScopeFactory serviceScopeFactory, ISftpService sftpService, IPhpGeneratorService phpGeneratorService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _sftpService = sftpService;
            _phpGeneratorService = phpGeneratorService;
        }

        public async Task<BaseResult> SetStorageDataAsync(string host, string port, string login, string password, long userId)
        {
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return new BaseResult
                {
                    IsSuccess = false,
                    ResultMessage = ResultMessage.WrongParameters
                };
            }

            int ftpPort;

            if (!int.TryParse(port, out ftpPort))
            {
                return new BaseResult
                {
                    IsSuccess = false,
                    ResultMessage = ResultMessage.FtpPortIsNotCorrect
                };
            }

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var serviceProvider = scope.ServiceProvider;

                var userRepository = serviceProvider.GetRequiredService<IBaseRepository<UserData>>();
                var user = await userRepository.GetAll()
                    .Include(u => u.UserFtp)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return new BaseResult
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.DontHaveAccess
                    };
                }

                if (user.UserFtp == null)
                {
                    user.UserFtp = new UserFtp()
                    {
                        SftpHost = host,
                        SftpPort = ftpPort,
                        SftpLogin = login,
                        SftpPassword = password,
                        UserId = userId
                    };
                }
                else
                {
                    user.UserFtp.SftpHost = host;
                    user.UserFtp.SftpLogin = login;
                    user.UserFtp.SftpPassword = password;
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

        public async Task<BaseResult> SaveDataAsync(long userId)
        {
           
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var serviceProvider = scope.ServiceProvider;

                var userRepository = serviceProvider.GetRequiredService<IBaseRepository<UserData>>();
                var user = await userRepository.GetAll()
                    .Include(u => u.UserFtp)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return new BaseResult
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.DontHaveAccess
                    };
                }

                var userAppRepository = serviceProvider.GetRequiredService<IBaseRepository<UserApp>>();
                var userApp = await userAppRepository.GetAll()
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefaultAsync();

                if (userApp is null || userApp.Secret is null || userApp.SecretKeyParam is null)
                {
                    return new BaseResult
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.noData
                    };
                }

                string phpScript = _phpGeneratorService.GeneratePhpScript(userApp.AppName, userApp.AppBundle, userApp.Secret, userApp.SecretKeyParam);

                var uploadResult = await _sftpService.UploadFileAsync
                    (
                        user.UserFtp.SftpHost,
                        user.UserFtp.SftpPort,
                        user.UserFtp.SftpLogin, 
                        user.UserFtp.SftpPassword, 
                        phpScript
                    );
                
                if ( uploadResult.IsSuccess )
                {
                    var userHistoryRepository = serviceProvider.GetRequiredService<IBaseRepository<UserHistory>>();

                    var userHistory = new UserHistory()
                    {
                        AppName = userApp.AppName,
                        AppBundle = userApp.AppBundle,
                        Secret = userApp.Secret,
                        SecretKeyParam = userApp.SecretKeyParam,
                        Username = userApp.AppName,
                        UserId = userId,
                    }; 

                    await userHistoryRepository.CreateAsync( userHistory );
                    await userHistoryRepository.SaveChangesAsync();
                }

                return uploadResult;

            }
            catch (Exception ex)
            {
                return new BaseResult 
                { 
                    IsSuccess = false, 
                    ResultMessage = $"{ResultMessage.Error}: {ex.Message}" 
                };
            }
        }
        public async Task<CollectionResult<HistoryDto>> GetLastUploads(long userId)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var serviceProvider = scope.ServiceProvider;

                var userRepository = serviceProvider.GetRequiredService<IBaseRepository<UserData>>();

                var user = await userRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId || u.Role == UserRole.Admin.ToString());

                if (user is null)
                {
                    return new CollectionResult<HistoryDto>
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.DontHaveAccess
                    };
                }

                var userHistoryRepository = serviceProvider.GetRequiredService<IBaseRepository<UserHistory>>();

                var history = await userHistoryRepository.GetAll().AsNoTracking()
                    .OrderByDescending(x => x.UserId)
                    .Take(10)
                    .ToListAsync();

                if (history == null || history.Count == 0)
                {
                    return new CollectionResult<HistoryDto>
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.HistoryEmpty
                    };
                }

                List<HistoryDto> historyList = history.Select(h => new HistoryDto
                {
                    UserId = h.UserId,
                    Username = h.Username,
                    AppName = h.AppName,
                    AppBundle = h.AppBundle,
                    Timestamp = h.Timestamp,
                }).ToList();

                return new CollectionResult<HistoryDto>
                {
                    IsSuccess = true,
                    Data = historyList
                };
            }
            catch (Exception ex)
            {
                return new CollectionResult<HistoryDto> 
                { 
                    IsSuccess = false, 
                    ResultMessage = $"{ResultMessage.Error}: {ex.Message}" 
                };
            }
        }

    
    }
}
