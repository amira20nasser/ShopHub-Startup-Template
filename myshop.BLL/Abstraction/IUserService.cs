using myshop.BLL.DTOs;

namespace myshop.BLL.Abstraction
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserDto>> GetAllUsers();
        Task<bool> Delete(string id);
    }
}
