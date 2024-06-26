using Login.Core.Entities;
using Login.Data.Repositories;
using Login.UnitTests.Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.UnitTests.Repositories
{
    public class UserRepositoryTests : BaseRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturn_AllUsers()
        {
            var context = CreateInMemoryContext();
            await context.Database.EnsureCreatedAsync();

            var user1 = new User
            {
                Username = "UserTest1",
                Email = "user@test.com",
                PasswordHash = "testhash",
                Salt = "testsalt",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var user2 = new User
            {
                Username = "UserTest2",
                Email = "user2@test.com",
                PasswordHash = "testhash2",
                Salt = "testsalt2",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            await context.Users.AddRangeAsync(user1, user2);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            var result = await repository.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal<User>(user1, result[0]);
            Assert.Equal<User>(user2, result[1]);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_User()
        {
            var context = CreateInMemoryContext();
            await context.Database.EnsureCreatedAsync();

            var user1 = new User
            {
                Username = "UserTest1",
                Email = "user@test.com",
                PasswordHash = "testhash",
                Salt = "testsalt",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var user2 = new User
            {
                Username = "UserTest2",
                Email = "user2@test.com",
                PasswordHash = "testhash2",
                Salt = "testsalt2",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            await context.Users.AddRangeAsync(user1, user2);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            var result = await repository.GetByIdAsync(2);

            Assert.Equal<User>(result, user2);
            Assert.NotEqual<User>(result, user1);
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturn_User()
        {
            var context = CreateInMemoryContext();
            await context.Database.EnsureCreatedAsync();

            var user1 = new User
            {
                Username = "UserTest1",
                Email = "user@test.com",
                PasswordHash = "testhash",
                Salt = "testsalt",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var user2 = new User
            {
                Username = "UserTest2",
                Email = "user2@test.com",
                PasswordHash = "testhash2",
                Salt = "testsalt2",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            await context.Users.AddRangeAsync(user1, user2);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            var result = await repository.GetByUsernameAsync("UserTest1");

            Assert.Equal<User>(result, user1);
            Assert.NotEqual<User>(result, user2);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturn_User()
        {
            var context = CreateInMemoryContext();
            await context.Database.EnsureCreatedAsync();

            var user1 = new User
            {
                Username = "UserTest1",
                Email = "user@test.com",
                PasswordHash = "testhash",
                Salt = "testsalt",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var user2 = new User
            {
                Username = "UserTest2",
                Email = "user2@test.com",
                PasswordHash = "testhash2",
                Salt = "testsalt2",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            await context.Users.AddRangeAsync(user1, user2);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            var result = await repository.GetByEmailAsync("user2@test.com");

            Assert.Equal<User>(result, user2);
            Assert.NotEqual<User>(result, user1);
        }

        [Fact]
        public async Task CreateAsync_ValidUser_ShouldInsert_And_Returns_UserId()
        {
            var context = CreateInMemoryContext();
            await context.Database.EnsureCreatedAsync();

            var user = new User
            {
                Username = "UserTest",
                Email = "user@test.com",
                PasswordHash = "testhash",
                Salt = "testsalt",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var repository = new UserRepository(context);

            var result = await repository.CreateAsync(user);

            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_ValidUser_ShouldUpdate_And_Returns_UpadatedUser()
        {
            var context = CreateInMemoryContext();
            await context.Database.EnsureCreatedAsync();

            var user = new User
            {
                Username = "UserTest",
                Email = "user@test.com",
                PasswordHash = "testhash",
                Salt = "testsalt",
                EmailConfirmed = false,
                CreatedAt = DateTime.Now
            };

            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            var data = new DateTime(2024, 6, 26);

            user.Username = "UserTestEdited";
            user.Email = "user@testedited.com";
            user.PasswordHash = "testhashedited";
            user.Salt = "testsaltedited";
            user.EmailConfirmed = true;
            user.CreatedAt = data;

            await repository.UpdateAsync(user);

            var result = await repository.GetByIdAsync(1);

            Assert.Equal("UserTestEdited", result.Username);
            Assert.Equal("user@testedited.com", result.Email);
            Assert.Equal("testhashedited", result.PasswordHash);
            Assert.Equal("testsaltedited", result.Salt);
            Assert.True(result.EmailConfirmed);
            Assert.Equal(data, result.CreatedAt);
        }
    }
}
