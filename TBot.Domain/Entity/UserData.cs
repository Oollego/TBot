using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Enum;

namespace TBot.Domain.Entity
{
    public class UserData
    {
        public long Id { get; set; }
        public string UserName { get; set; } = default!;
        public string PasswordSalt { get; set; } = default!;
        public string PasswordDk { get; set; } = default!;
        public string Role { get; set; } = UserRole.Guest.ToString();
        public bool IsAuthorized { get; set; } = false;
        public UserFtp UserFtp { get; set; } = default!;
        public List<UserApp> UserApps { get; set; } = default!;
        public UserAppSetting UserAppSetting { get; set; } = default!;

    }
}
