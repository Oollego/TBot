using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Entity;
using TBot.Domain.Result;

namespace TBot.Domain.Services
{
    public interface IAuthService
    {
        Task <BaseResult> RegisterAsync(string username, string password, long userId);
        Task<BaseResult> LoginAsync(string password, long userId);

    }
}
