using RS.DataAccessLibrary.DTOs.Requests;
using RS.DataAccessLibrary.Interfaces;
using RS.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Tests.Mocks
{
    public class MockRepository : IRepository
    {
        #region Mocked Objects

        public IEnumerable<UserModel> MockListOfUsers { get; set; } = new List<UserModel>();
        public IEnumerable<RefreshTokenModel> MockListOfTokens { get; set; } = new List<RefreshTokenModel>();
        public IEnumerable<MockUserModel> MockListOfMockUserModel { get; set; } = new List<MockUserModel>();

        #endregion

        public string CurrentConnectionString { get; set; }

        public Task<UserModel> AttemptLoginAsync(string emailAddress, string password)
        {
            // We set all users in our tests to have the password of "test"
            // If the password matches, check if the user exists in the mocked lsit of users
            if (password == "test") return Task.FromResult(MockListOfUsers.FirstOrDefault(u => u.EmailAddress == emailAddress));
            else return Task.FromResult<UserModel>(null);
        }

        public Task<UserModel> CreateUserAsync(RegisterUserRequest user, string salt) => user.EmailAddress == "fail@fail.com" ? Task.FromResult<UserModel>(null) : Task.FromResult(new UserModel 
        { 
            EmailAddress = user.EmailAddress,
            FirstName = user.FirstName,
            LastName = user.LastName
        });

        public Task<IEnumerable<UserModel>> GetAllUsersAsync() => Task.FromResult(MockListOfUsers);

        public Task<RefreshTokenModel> GetRefreshTokenAsync(string refreshToken) => Task.FromResult(MockListOfTokens.FirstOrDefault(t => t.Token == refreshToken));

        public Task<UserModel> GetSingleUserAsync(int userId) => Task.FromResult(MockListOfUsers.FirstOrDefault(u => u.UserId == userId));

        public Task<string> GetUserSaltAsync(string emailAddress) => Task.FromResult(MockListOfMockUserModel.FirstOrDefault(u => u.EmailAddress == emailAddress)?.Salt);

        public Task<RefreshTokenModel> SaveRefreshTokenAsync(RefreshTokenModel refreshToken)
        {
            refreshToken.TokenId = MockListOfTokens.Count() + 1;
            return Task.FromResult(refreshToken);
        }

        public Task UpdateUserAsync(UserModel user)
        {
            var userToUpdate = MockListOfUsers.FirstOrDefault(u => u.UserId == user.UserId);

            if (userToUpdate != null)
            {
                // Just updating first name for test purpose
                userToUpdate.FirstName = user.FirstName;
            }

            return Task.FromResult<object>(null);
        }
    }
}
