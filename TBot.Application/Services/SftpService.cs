using Renci.SshNet;
using System.Text;
using TBot.Domain.Services;

namespace TBot.Application.Services
{
    internal class SftpService : ISftpService
    {
        public void UploadFile(string host, string username, string password, string remoteDirectory, string fileName, string fileContent)
        {
            using (var sftp = new SftpClient(host, username, password))
            {
                sftp.Connect();
                sftp.ChangeDirectory(remoteDirectory);
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
                {
                    sftp.UploadFile(ms, fileName);
                }
                sftp.Disconnect();
            }
        }
    }
}
