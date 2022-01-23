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
        //public int score { get; set; }
        //public int playedGames { get; set; }
        //public int coin { get; set; }
        //public List<Card> stack { get; set; }
        //public List<Card> deck { get; set; }

        public User(string username, string password, string role = "user")
        {
            if (role == null)
            {
                role = "user";
            }
            Username = username;
            Password = password;
            Role = UserRoleStringToEnum(role);
        }
        private static Role UserRoleStringToEnum(string role)
        {
            return role.ToLower().Equals("admin") ? Role.Admin : Role.User;
        }
    }
}
