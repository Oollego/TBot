using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Services;

namespace TBot.Application.Services
{
    internal class SecretService : ISecretService
    {
        public string GenerateSecret()
        {
            //using (var rsa = new RSACryptoServiceProvider(2048))
            //{
            //    return Convert.ToBase64String(rsa.ExportCspBlob(false));
            //}
            return Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
        }

        public string GenerateSecretKeyParam()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
