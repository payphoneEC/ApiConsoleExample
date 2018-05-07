using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConnectionExample
{
    public static class Configurations
    {
        private static string _token;
        private static string _lang;

        public static string Token {            
            get { return _token ?? ""; }
            set{_token = value;}
        }

        public static string Lang
        {
            get { return _lang ?? "es"; }
            set { _lang = value; }
        }

        public const string ClientId = "";
        public const string ClientSecret = "";
        public const string ResourcePath = "https://pay-dev.payphone.com.ec";
        
    }
}
