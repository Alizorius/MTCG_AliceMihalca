using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//this class is dedicated to share the same methods between classes

namespace MTCG
{
    static class Helper
    {
        public static User ExtractUser(string request)
        {
            User deserializedUser = JsonConvert.DeserializeObject<User>(request.Substring(request.IndexOf('{')));
            Console.WriteLine(deserializedUser.username);
            return deserializedUser;

        }
    }
}
