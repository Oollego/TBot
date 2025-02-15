using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Services
{
    public interface IPhpGeneratorService
    {
        string GeneratePhpScript(string appName, string appBundle, string secret, string secretKeyParam);

    }
}

