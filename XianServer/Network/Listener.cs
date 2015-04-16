using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace XianServer.Network
{
    public sealed class Listener : IDisposable
    {
        private Socket m_listener;
        private bool m_disposed;

        public Action<Session> OnClientAccepted;

        public short Port { get; private set; }

        public Listener(IPAddress address, short port)
        {
            Port = port;

            m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_listener.Bind(new IPEndPoint(address, port));

            m_disposed = false;
        }

        public void Start()
        {
            m_listener.Listen(10);
            m_listener.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult iar)
        {
            if (m_disposed) { return; }

            try
            {
                Socket client = m_listener.EndAccept(iar);

                var session = new Session(client);

                if (OnClientAccepted != null)
                    OnClientAccepted(session);

                session.Start();
            }
            finally
            {
                if (m_disposed == false)
                       m_listener.BeginAccept(AcceptCallback, null);
            }

        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                m_listener.Dispose();
            }
        }
    }
}
