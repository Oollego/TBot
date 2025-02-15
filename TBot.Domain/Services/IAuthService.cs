using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Entity;

namespace TBot.Domain.Services
{
    public interface IAuthService
    {
        Task Register(string username, string password);
        Task<User> GetUserById(long userId);
    }
}
