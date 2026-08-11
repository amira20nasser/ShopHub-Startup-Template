using myshop.BLL.DTOs;

namespace myshop.BLL.Abstraction
{
    public interface IUserService
    {
        Task<PagedUsersDto> GetUsersAsync(UserQueryDto query);
        Task<bool> Delete(string id);
        Task<bool> PromoteToAdminAsync(string id);
        Task<bool> DemoteToCustomerAsync(string id);
        Task<bool> LockAsync(string id);
        Task<bool> UnlockAsync(string id);
    }
}
