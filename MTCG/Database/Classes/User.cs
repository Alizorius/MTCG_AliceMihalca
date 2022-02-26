using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    enum Role
    {
        User,
        Admin
    }
    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public string Displayname { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public int Coins { get; set; }

        [JsonConstructor]
        public User(string username, string password, string role = "user")
        {
            if (role == null)
            {
                role = "user";
            }
            Username = username;
            Password = password;
            Role = UserRoleStringToEnum(role);
            Coins = 20;

        }

        private static Role UserRoleStringToEnum(string role)
        {
            return role.ToLower().Equals("admin") ? Role.Admin : Role.User;
        }
    }
}
