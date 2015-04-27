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
            CurClients++;
            Logger.Write("add entry: {0}", CurClients);   
            return true;
        }
        public void RemoveEntry()
        {
            Logger.Write("remove entry: {0}", CurClients);
            if(this.CurClients > 0 && this.CurClients <= this.MaxClients)
                this.CurClients--;
        }

        public void Update()
        {
            WvsServer.Instance.Database.GetValidEntries(Hwid, ref m_licenses);

            MaxClients = m_licenses.Sum(x => x.MaxClients);

            LastUpdate = DateTime.Now;
        }
    }
}
