using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    static class DBUser
    {
        public static void CreateUser(string request)
        {
            Helper.ExtractUser(request);
        }

    }
}
