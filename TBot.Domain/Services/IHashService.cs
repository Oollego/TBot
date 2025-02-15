using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Services
{
    public interface IHashService
    {
        public string HexString(string input);
    }
}
