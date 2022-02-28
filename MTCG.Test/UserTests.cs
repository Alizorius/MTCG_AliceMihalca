using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using System;

namespace MTCG.Test
{
    public class UserTests
    {
        [SetUp]
        public void Init()
        {
            DB.CreateDBIfNotPresent();
            User user = new MTCG.User("TestName", "123");
            MTCG.DBUser.AddUser(user);
        }

        [Test]
        public void AddSameUserTwice()
        {
            User user = new User("TestName", "123");
            Assert.That(!DBUser.AddUser(user), 
                "If the same User is added to the Database twice, it should return false.");
        }

        [Test]
        public void AddUserWithInvalidUsername()
        {
            User user = new User("Test", "123");
            Assert.That(!DBUser.AddUser(user), 
                "If username isn't between 5 and 11 characters or uses special characters, it should return false.");
        }

        [Test]
        public void TryToLoginCorrectPassword()
        {
            User user = new User("TestName", "123");
            Assert.That(DBUser.LoginUser(user.Username, user.Password), 
                "If user exists in the database and the parameters equal with the database, it should return true.");
        }

        [Test]
        public void TryToLoginWrongPassword()
        {
            User user = new User("TestName", "321");
            Assert.That(!DBUser.LoginUser(user.Username, user.Password),
                "If user exists in the database and the password doesn't match with the db, it should return false.");
        }

        [TearDown]
        public void Cleanup()
        {
            using var conn = DB.Connection();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM userTable WHERE username = @p1",
                conn);
            cmd.Parameters.AddWithValue("p1", "TestName");
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            cmd.ExecuteNonQuery();
        }
    }
}
