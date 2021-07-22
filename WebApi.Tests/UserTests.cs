using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Tests
{
    [TestClass]
    public class UserTests
    {
        private IUserService userService;

        [TestInitialize]
        public void Setup()
        {
            byte[] passwordSalt;
            byte[] passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("123"));
            }

            var data = new List<User>
            {
                new User { FirstName = "Tiago", LastName = "Soares", Id = 1, Username = "tsoares", PasswordHash = passwordHash, PasswordSalt = passwordSalt }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<DataContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            userService = new UserService(mockContext.Object);
        }

        [TestMethod]
        public void Authenticate_Successfully()
        {
            var user = userService.Authenticate("tsoares", "123");

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void Authenticate_Unuccessfully()
        {
            var user = userService.Authenticate("tsoares", "1232");

            Assert.IsNull(user);
        }

        [TestMethod]
        public void Register_Successfully()
        {
            var user = userService.Create(new User() {
                FirstName = "Tiago",
                LastName = "Soares",
                Username = "tsoares2"
            }, "123");

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void Register_Password_Empty()
        {
            Assert.ThrowsException<AppException>(() => userService.Create(new User()
            {
                FirstName = "Tiago",
                LastName = "Soares",
                Username = "tsoares132"
            }, ""), "Password is required");
        }

        [TestMethod]
        public void Register_Username_Taken()
        {
            Assert.ThrowsException<AppException>(() => userService.Create(new User()
            {
                FirstName = "Tiago",
                LastName = "Soares",
                Username = "tsoares"
            }, "123"), "Username \"tsoares\" is already taken");
        }
    }
}
