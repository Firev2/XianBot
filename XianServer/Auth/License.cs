using System;

namespace XianServer.Auth
{
    public class License
    {
        public bool Expired
        {
            get
            {
                return DateTime.Now >= Expiry;
            }
        }

        public string Hash { get; set; }

        public int MaxClients { get; set; }

        public int NumDays {get;set;}

        public DateTime Expiry { get; set; }
    }
}
