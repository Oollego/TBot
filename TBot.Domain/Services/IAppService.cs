using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Dto;
using TBot.Domain.Result;

namespace TBot.Domain.Services
{
    public interface IAppService
    {
        Task<BaseResult> SetAppAsync(string appName, string appBundle, long userId);
        Task<BaseResult<SecretDto>> GetSecretAsync(long userId);
    }
}
