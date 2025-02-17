using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Dto
{
    public class HistoryDto
    {
        public long UserId { get; set; }
        public string Username { get; set; } = "";
        public string? AppName { get; set; }
        public string? AppBundle { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
