using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Services;

namespace TBot.Application.Services
{
    internal class UserService: IUserService
    {
        private readonly Dictionary<long, (string Role, string UserName)> _users = new Dictionary<long, (string, string)>();

        public bool IsUserAuthorized(long telegramUserId)
        {
            return _users.ContainsKey(telegramUserId);
        }

        public bool IsUserAdmin(long telegramUserId)
        {
            return _users.TryGetValue(telegramUserId, out var user) && user.Role == "Admin";
        }

        public void RegisterUser(long telegramUserId, string userName)
        {
            if (!_users.ContainsKey(telegramUserId))
            {
                _users.Add(telegramUserId, ("User", userName));
            }
        }
    }
}
