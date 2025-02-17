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
        Task<BaseResult> UploadFileAsync(string host, string username, string password, string directory, string fileContent);

    }
}
