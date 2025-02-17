using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Result;

namespace TBot.Domain.Services
{
    public interface ISftpService
    {
        Task<BaseResult> UploadFileAsync(string host, int port, string username, string password, string fileContent);

    }
}
