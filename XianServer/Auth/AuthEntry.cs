using System;
using System.Linq;
using System.Collections.Generic;
using XianServer.Server;

namespace XianServer.Auth
{
    public class AuthEntry
    {
        public string Hwid { get; private set; }
        
        public int MaxClients { get; private set; }
        public int CurClients { get; private set; }

        public DateTime LastUpdate { get; private set; }
        private List<License> m_licenses;

        public AuthEntry(string hwid)
        {
            Hwid = hwid;
            LastUpdate = DateTime.Now;
            m_licenses = new List<License>();
        }

        public bool AddEntry()
        {
            if(CurClients < MaxClients)
            {
                CurClients++;
                return true;
            }
            
            return false;
        }
        public void RemoveEntry()
        {
            if (CurClients > 0)
                CurClients--;
        }

        public void Update()
        {
            WvsServer.Instance.Database.GetValidEntries(Hwid, ref m_licenses);

            MaxClients = m_licenses.Sum(x => x.MaxClients);

            LastUpdate = DateTime.Now;
        }
    }
}
