using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Dto;
using TBot.Domain.Result;

namespace TBot.Domain.Services
{
    public interface IStorageService
    {
        Task<BaseResult> SetStorageDataAsync(string host, string port, string login, string password, long userId);
        Task<BaseResult> SaveDataAsync(long userId);
        Task<CollectionResult<HistoryDto>> GetLastUploads(long userId);
    }
}
