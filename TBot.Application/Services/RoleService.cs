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
    internal class RoleService : IRoleService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RoleService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<BaseResult> ChangeRoleAsync(long userId, string role, long userChatId)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var serviceProvider = scope.ServiceProvider;

                var userRepository = serviceProvider.GetRequiredService<IBaseRepository<UserData>>();

                var admin = await userRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId || u.Role == UserRole.Admin.ToString());

                if (admin is null)
                {
                    return new CollectionResult<HistoryDto>
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.DontHaveAccess
                    };
                }

                var user = await userRepository.GetAll().FirstOrDefaultAsync(u => u.Id == userChatId);

                if (user is null)
                {
                    return new CollectionResult<HistoryDto>
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.UserIsNotExistWithId
                    };
                }

                if (user.Role == role)
                {
                    return new CollectionResult<HistoryDto>
                    {
                        IsSuccess = true,
                        ResultMessage = ResultMessage.UserRoleIHaveBeenChanged
                    };
                }

                user.Role = role;
                await userRepository.SaveChangesAsync();

                return new CollectionResult<HistoryDto>
                {
                    IsSuccess = true,
                    ResultMessage = ResultMessage.UserRoleIHaveBeenChanged
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
