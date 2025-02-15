using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBot.Domain.Services
{
    public interface ISftpService
    {
        void UploadFile(string host, string username, string password, string remoteDirectory, string fileName, string fileContent);
    }
}
