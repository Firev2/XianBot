using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianServer.Server;
using XianServer.User;

namespace XianServer.Auth
{
    public sealed class AuthCenter : IDisposable
    {
        private object m_locker;
        private Dictionary<string, AuthEntry> m_entries;

        public AuthCenter()
        {
            m_locker = new object();
            m_entries = new Dictionary<string, AuthEntry>();
        }

        public bool AddClient(string hwid, Client c)
        {
            if (hwid == null)
                return false;

            lock (m_locker)
            {
                EnsureHwid(hwid);

                if (m_entries[hwid].CurClients >= m_entries[hwid].MaxClients) {
                    Logger.Write("Client limit reached!");
                    return false;
                }
                else
                    return m_entries[hwid].AddEntry();
            }
        }
        public void RemoveClient(string hwid)
        {
            if (hwid == null)
                return;

            lock (m_locker)
            {
                EnsureHwid(hwid);
                m_entries[hwid].RemoveEntry();
                 
            }
        }

        private void EnsureHwid(string hwid)
        {
            AuthEntry entry = null;

            if (m_entries.TryGetValue(hwid, out entry))
            {
                var span = DateTime.Now - entry.LastUpdate;

                if (span.Minutes >= 15)
                {
                    entry.Update();
                }
            }
            else
            {
                entry = new AuthEntry(hwid);
                entry.Update();
                m_entries.Add(hwid, entry);
                
            }
        }

        public void Dispose()
        {
            lock (m_locker)
            {
                m_entries.Clear();
            }
        }
    }
}
