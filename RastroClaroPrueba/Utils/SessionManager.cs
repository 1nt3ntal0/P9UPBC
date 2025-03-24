using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RastroClaroPrueba.Utils
{
    public static class SessionManager
    {
        public static int UserId { get; set; }  
        public static string Token { get; set; }  
        public static void ClearSession()
        {
            UserId = 0;
            Token = null;
        }
    }

}
