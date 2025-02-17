using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Dto
{
    public class SecretDto
    {
        public string Secret { get; set; } = default!;
        public string SecretKeyParam { get; set; } = default!;
    }
}
