namespace ApiConnectionExample
{
    public static class Configurations
    {
        private static string _token;
        private static string _lang;

        public static string Token {            
            get { return _token ?? "JMuJDFqYR3W5vApsamH63gSnP2ajyth3BKI1KwF1vnNlc_yk6PbNasfsoe728IeN0whroco6YqA2abjEFk38g5U1lRfsbg_9r-xQSk3h2D3I_iwdsIm9pE3zolzcmokpHVcMxG1EJP_LFK7XRkF17ct40tWmyb0M7Mz-VY5TIVifWyLSdl5maQM1HuEcuP0PTrCt4mKjRJZOStAb7fg0rOTE2UQn-FsZOts50jL1kUWw0j7KdzAwOKZjD46vIAt_T6y3pnHp_ymxDj3FYXJMStgCcCh3L2ooTJIU1bd0s2b0znwsih84l1wou2IT8Vne2th67g"; }
            set{_token = value;}
        }

        public static string Lang
        {
            get { return _lang ?? "es"; }
            set { _lang = value; }
        }

        public const string ClientId = "13M2HZCw8kG14SBhJWIFfw";
        public const string KeySecret = "UhvMYd5KEofAYhNH0SlQ";
        //public const string ResourcePath = "https://pay-dev.payphone.com.ec";
        public const string ResourcePath = "http://localhost:18728";

    }
}
