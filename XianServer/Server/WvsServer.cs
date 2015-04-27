using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using XianServer.Packet;
using XianServer.Network;
using XianServer.User;
using XianServer.Tools;
using XianServer.Auth;

namespace XianServer.Server
{
    public sealed class WvsServer : IDisposable
    {
        public static WvsServer Instance { get; set; }

        public Database Database
        {
            get
            {
                return m_database;
            }
        }
        public AuthCenter AuthCenter
        {
            get
            {
                return m_center;
            }
        }

        private List<Client> m_clients;
        private Listener m_acceptor;
        private Timer m_pinger;
        private Database m_database;
        private AuthCenter m_center;

        public WvsServer(short port, string connectionString)
        {
            m_clients = new List<Client>();

            m_acceptor = new Listener(IPAddress.Any, port);
            m_acceptor.OnClientAccepted = (s) => new Client(s);

            m_pinger = new Timer(300000); //5 min
            m_pinger.Elapsed += (s, e) => Ping();

            m_database = new Database(connectionString);
            m_center = new AuthCenter();
        }

        public void AddClient(Client c)
        {
            Logger.Write("Client {0} connected", c.Name);
            m_clients.Add(c);
            UpdateTitle();
        }
        public void RemoveClient(Client c)
        {
            AuthCenter.RemoveClient(c.Hwid);
            Logger.Write("Client {0} disconnected", c.Name);
            m_clients.Remove(c);
            UpdateTitle();
        }
        private void Ping()
        {
            if (m_clients.Count > 0)
            {
                Logger.Write("Pinger execute {0} clients", m_clients.Count);

                m_clients.ToArray().ForEach((c) =>
                {
                  
                    c.SendPing();
                });
            }
        }
        private void UpdateTitle()
        {
            string title = string.Concat("XianServer - Clients: ", m_clients.Count);
            Logger.Title(title);
        }

        public void Run()
        {
            UpdateTitle();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Logger.Exception((Exception)e.ExceptionObject);

            OffsetTable.EuTable.Cache();
            OffsetTable.SeaTable.Cache();

            m_acceptor.Start();
            Logger.Write("Listening on port {0}", m_acceptor.Port);

            m_pinger.Start();


            while (true)
            {
                try
                {
                    string[] tokens = Console.ReadLine().Split(' ');

                    switch (tokens[0])
                    {
                        case "offset":
                            OffsetTable.EuTable.Cache();
                            OffsetTable.SeaTable.Cache();
                            break;
                        case "exit":
                            return;
                        case "ping":
                            Ping();
                            break;
                        case "clear":
                        case "cls":
                            Console.Clear();
                            break;
                        case "message":

                            
                            if (m_clients.Count > 0)
                            {
                                m_clients.ToArray().ForEach((c) =>
                                {
                                    string msg = "";

                                    for (int i = 2; i < tokens.Length; i++)
                                    {
                                        msg = msg + tokens[i];
                                        msg = msg + " ";
                                    }


                                    c.SendMessage(1, tokens[1], msg);
                                });
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }

        public void Dispose()
        {
            m_pinger.Stop();
            m_pinger.Dispose();

            m_acceptor.Dispose();
            m_center.Dispose();

            foreach (var client in m_clients.ToArray())
                client.Dispose();
        }
    }
}
