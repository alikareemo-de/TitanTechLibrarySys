

namespace TT.LibrarySys.DataAccess.Models
{
    public interface IUserRepository
    {
        Task<User> SignUpAsync(User user, string password);
        Task<User> SignInAsync(string email, string password);
        Task<bool> UserExistsAsync(string email);
    }
}
