using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Entity
{
    public class UserFtp
    {
        public long Id {  get; set; }
        public string SftpHost { get; set; } = default!;
        public int SftpPort { get; set; } 
        public string SftpLogin { get; set; } = default!;
        public string SftpPassword { get; set; } = default!;
        public long UserId { get; set; }
        public UserData UserData { get; set; } = default!;

    }
}
