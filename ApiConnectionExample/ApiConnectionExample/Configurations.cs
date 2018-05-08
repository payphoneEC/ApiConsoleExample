namespace ApiConnectionExample
{
    public static class Configurations
    {
        private static string _token;
        private static string _lang;

        public static string Token {            
            get { return _token ?? "<default token, you can get this in developer page>"; }
            set{_token = value;}
        }

        public static string Lang
        {
            get { return _lang ?? "es"; }
            set { _lang = value; }
        }

        public const string ClientId = "<client id of your application, you can get this in developer page>";
        public const string KeySecret = "<key secret of your application, you can get this in developer page>";
        public const string Ruc = "<Company unique code ralated with your application>";
        public const string ResourcePath = "https://pay.payphonetodoesposible.com";       

    }
}
