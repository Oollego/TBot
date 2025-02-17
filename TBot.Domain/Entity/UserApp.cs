using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Entity
{
    public class UserApp
    {
        public long Id { get; set; }
        public string AppName { get; set; } = default!;
        public string AppBundle { get; set; } = default!;
        public string? Secret { get; set; }
        public string? SecretKeyParam { get; set; }
        public long UserId { get; set; }
        public UserData UserData { get; set; } = default!;
    }
}
