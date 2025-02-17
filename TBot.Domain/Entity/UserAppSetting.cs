using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Entity
{
    public class UserAppSetting
    {
        public long Id { get; set; }
        public string AppName { get; set; } = default!;
        public string AppBundle { get; set; } = default!;
        public long UserId { get; set; }
        public UserData UserData { get; set; } = default!;
    }
}
