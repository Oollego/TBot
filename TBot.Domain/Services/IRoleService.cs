using TBot.Domain.Result;

namespace TBot.Domain.Services
{
    public interface IRoleService
    {
       Task<BaseResult> ChangeRoleAsync(long userId, string role, long userChatId);
    }
}
