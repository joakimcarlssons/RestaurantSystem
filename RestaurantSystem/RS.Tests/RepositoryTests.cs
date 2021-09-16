using NUnit.Framework;
using RS.DataAccessLibrary.DTOs.Requests;
using RS.DataAccessLibrary.Interfaces;
using RS.SharedLibrary.Models;
using RS.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Tests
{
    /// <summary>
    /// Tests for <see cref="IRepository"/>
    /// </summary>
    public class RepositoryTests
    {
        #region Private Members

        /// <summary>
        /// The instance of the mocked <see cref="IRepository"/>
        /// </summary>
        private MockRepository Repository { get; set; }

        #endregion

        #region Set up

        [SetUp]
        public void Setup()
        {
            Repository = new();

            Repository.MockListOfUsers = new List<UserModel>
            {
                new()
                {
                    UserId = 1,
                    EmailAddress = "first@test.com",
                    FirstName = "First",
                    LastName = "Test"
                },
                new()
                {
                    UserId = 2,
                    EmailAddress = "second@test.com",
                    FirstName = "Second",
                    LastName = "Test"
                }
            };

            Repository.MockListOfTokens = new List<RefreshTokenModel>
            {
                new()
                {
                    TokenId = 1,
                    UserId = 1,
                    Token = "123",
                    JwtId = "123",
                    AddedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMonths(6)
                },
                new()
                {
                    TokenId = 2,
                    UserId = 2,
                    Token = "456",
                    JwtId = "456",
                    AddedDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMonths(6)
                }
            };

            Repository.MockListOfMockUserModel = new List<MockUserModel>
            {
                new()
                {
                    UserId = Repository.MockListOfUsers.First().UserId,
                    EmailAddress = Repository.MockListOfUsers.First().EmailAddress,
                    FirstName = Repository.MockListOfUsers.First().FirstName,
                    LastName = Repository.MockListOfUsers.First().LastName,
                    Salt = "S4L7",
                    UserRoles = new List<RoleModel>
                    {
                        new()
                        {
                            RoleId = 1,
                            RoleName = "Admin"
                        },
                        new()
                        {
                            RoleId = 2,
                            RoleName = "Employee"
                        }
                    }
                },
                new()
                {
                    UserId = Repository.MockListOfUsers.Last().UserId,
                    EmailAddress = Repository.MockListOfUsers.Last().EmailAddress,
                    FirstName = Repository.MockListOfUsers.Last().FirstName,
                    LastName = Repository.MockListOfUsers.Last().LastName,
                    Salt = "S4L7",
                    UserRoles = new List<RoleModel>
                    {
                        new()
                        {
                            RoleId = 3,
                            RoleName = "Customer"
                        }
                    }
                },
            };             
        }

        #endregion

        #region Register

        [Test]
        public void Create_User_Successful()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                EmailAddress = "test@test.com",
                FirstName = "test",
                LastName = "test"
            };

            // Act
            var registeredUser = Repository.CreateUserAsync(request, "").Result;

            // Assert
            Assert.AreEqual(request.EmailAddress, registeredUser.EmailAddress);
            Assert.AreEqual(request.FirstName, registeredUser.FirstName);
            Assert.AreEqual(request.LastName, registeredUser.LastName);
        }

        [Test]
        public void Create_User_EmailAddressExists_Failure()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                EmailAddress = "fail@fail.com",
                FirstName = "fail",
                LastName = "fail"
            };

            // Act
            var registeredUser = Repository.CreateUserAsync(request, "").Result;

            // Assert
            Assert.AreEqual(registeredUser, null);
        }


        #endregion

        #region Login

        [Test]
        [TestCase("first@test.com", "test")]
        [TestCase("second@test.com", "test")]
        public void Attempt_Login_Successful(string emailAddress, string password)
        {
            // Act
            var loggedInUser = Repository.AttemptLoginAsync(emailAddress, password).Result;

            // Assert
            Assert.AreNotEqual(loggedInUser, null);
        }

        [Test]
        [TestCase("fail@test.com", "test")]
        [TestCase("first@test.com", "fail")]
        public void Attempt_Login_Failure(string emailAddress, string password)
        {
            var loggedInUser = Repository.AttemptLoginAsync(emailAddress, password).Result;

            Assert.AreEqual(loggedInUser, null);
        }

        #endregion

        #region Users

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GetSingleUser_Successful(int userId)
        {
            var user = Repository.GetSingleUserAsync(userId).Result;
            Assert.AreNotEqual(user, null);
        }

        [Test]
        [TestCase(99)]
        public void GetSingleUser_Failure(int userId)
        {
            var user = Repository.GetSingleUserAsync(userId).Result;
            Assert.AreEqual(user, null);
        }

        [Test]
        public void GetAllUsers_Successful()
        {
            var userCount = Repository.MockListOfUsers.Count();

            Assert.That(Repository.GetAllUsersAsync().Result.Count() == userCount);
        }

        [Test]
        [TestCase(1, "Updated")]
        [TestCase(2, "Updated")]
        public void UpdateUser_ExistingUser_UpdateSucceded(int userId, string firstName)
        {
            // Arrange
            var user = new UserModel
            {
                UserId = userId,
                FirstName = firstName
            };

            // Act
            var succeeded = Repository.UpdateUserAsync(user).Result;

            // Assert
            Assert.That(Repository.MockListOfUsers.FirstOrDefault(u => u.UserId == user.UserId)?.FirstName == user.FirstName);
            Assert.That(succeeded == true);
        }

        [Test]
        [TestCase("first@test.com")]
        [TestCase("second@test.com")]
        public void VerifyEmailExistence_ExistingUsers_ReturnUserId(string emailAddress)
        {
            var userId = Repository.VerifyEmailAddressExistence(emailAddress).Result;
            Assert.That(userId == Repository.MockListOfUsers.FirstOrDefault(u => u.EmailAddress == emailAddress).UserId);
        }

        [Test]
        [TestCase("fail@fail.com")]
        public void VerifyEmailExistence_NonExistingUsers_ReturnNull(string emailAddress)
        {
            var userId = Repository.VerifyEmailAddressExistence(emailAddress).Result;
            Assert.That(userId == null);
        }

        #endregion

        #region Roles

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void Get_User_Roles_ExistingUsers_ReturnRoles(int userId)
        {
            var roles = Repository.GetUserRolesAsync(userId).Result;
            Assert.That(roles == Repository.MockListOfMockUserModel.FirstOrDefault(u => u.UserId == userId).UserRoles);
        }

        [Test]
        [TestCase(999)]
        public void Get_User_Roles_NonExistingUsers_ReturnNull(int userId)
        {
            var roles = Repository.GetUserRolesAsync(userId).Result;
            Assert.That(roles == null);
        }

        #endregion

        #region Tokens

        [Test]
        [TestCase("123")]
        [TestCase("456")]
        public void GetRefreshToken_ForExistingUser_ReturnsTokenModel(string token)
        {
            var refreshToken = Repository.GetRefreshTokenAsync(token).Result;

            Assert.AreNotEqual(refreshToken, null);
        }

        [Test]
        [TestCase("999")]
        public void GetRefreshToken_NonExistingUsers_ReturnNull(string token)
        {
            var refreshToken = Repository.GetRefreshTokenAsync(token).Result;

            Assert.AreEqual(refreshToken, null);
        }

        #endregion

        #region Salt

        [Test]
        [TestCase("first@test.com")]
        [TestCase("second@test.com")]
        public void GetUserSalt_ForExistingUser_ReturnSalt(string emailAddress)
        {
            var salt = Repository.GetUserSaltAsync(emailAddress).Result;
            Assert.That(salt == Repository.MockListOfMockUserModel.FirstOrDefault(u => u.EmailAddress == emailAddress).Salt);
        }

        [Test]
        [TestCase("fail@test.com")]
        public void GetUserSalt_NonExistingUser_ReturnNull(string emailAddress)
        {
            var salt = Repository.GetUserSaltAsync(emailAddress).Result;
            Assert.AreEqual(salt, null);
        }

        [Test]
        public void SaveRefreshToken_GetBackTokenWithId()
        {
            var refreshToken = new RefreshTokenModel
            {
                UserId = 1,
                Token = "789",
                JwtId = "789",
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            var newToken = Repository.SaveRefreshTokenAsync(refreshToken).Result;

            Assert.That(newToken.TokenId == Repository.MockListOfTokens.Count() + 1);
        }

        #endregion
    }
}
