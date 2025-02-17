using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Renci.SshNet;
using System.IO.Enumeration;
using System.Text;
using TBot.Application.Resources;
using TBot.Domain.Entity;
using TBot.Domain.Interfaces.Repositories;
using TBot.Domain.Result;
using TBot.Domain.Services;

namespace TBot.Application.Services
{
    internal class SftpService : ISftpService
    {

        public async Task<BaseResult> UploadFileAsync(string host, int port, string username, string password, string fileContent)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var client = new SftpClient(host, port, username, password);

                    client.Connect();

                    using var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

                    client.UploadFile(ms, GetFileName());

                    client.Disconnect();

                    return new BaseResult
                    {
                        IsSuccess = true,
                        ResultMessage = ResultMessage.FileHaveBeenUploaded
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResult
                    {
                        IsSuccess = false,
                        ResultMessage = ResultMessage.UploadFileError + "\n" + ex.Message
                    };
                }
            });
        }


        private static string GetFileName()
        {
            return Guid.NewGuid().ToString() + ".php";
        }
    }
}
