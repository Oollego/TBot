using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Services
{
    public interface IUserService
    {
        bool IsUserAuthorized(long telegramUserId);
        bool IsUserAdmin(long telegramUserId);
        void RegisterUser(long telegramUserId, string userName);
    }
}
