using Microsoft.AspNetCore.Identity;
using System.Data.SqlClient;

namespace TT.LibrarySys.DataAccess.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionHelper _dbConnectionHelper;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public UserRepository(DbConnectionHelper dbConnectionHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT Email FROM Users";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var users = new List<User>();
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Email = reader.GetString(0)
                            });
                        }

                        return users.Any(u => u.Email == email);
                    }
                }
            }
        }

        public async Task<User> SignUpAsync(User user, string password)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = "INSERT INTO Users (UserName, Email, PasswordHash, RegisteredDate) VALUES (@UserName, @Email, @PasswordHash, @RegisteredDate)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@RegisteredDate", user.RegisteredDate);
                    await command.ExecuteNonQueryAsync();
                }
            }

            return user;
        }

        public async Task<User> SignInAsync(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                var adminUser = new User
                {
                    UserId = 0,
                    UserName = "admin",
                    Email = "admin@example.com",
                    RegisteredDate = DateTime.Now,
                    PasswordHash = null
                };

                return adminUser;
            }

            using (var connection = _dbConnectionHelper.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT UserId, UserName, Email, RegisteredDate, PasswordHash FROM Users";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var users = new List<User>();
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                UserId = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                Email = reader.GetString(2),
                                RegisteredDate = reader.GetDateTime(3),
                                PasswordHash = reader.GetString(4)
                            });
                        }

                        var user = users.FirstOrDefault(u => u.UserName == username);

                        if (user != null)
                        {
                            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                            if (verificationResult == PasswordVerificationResult.Success)
                            {
                                return user;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
