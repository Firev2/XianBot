using System;
using System.Net.Sockets;

namespace XianServer.Network
{
    public sealed class Session : IDisposable
    {
        public const short ReceiveSize = 1024;

        private readonly Socket m_socket;

        private object m_sync;

        private byte[] m_recvBuffer;
        private byte[] m_packetBuffer;
        private int m_cursor;

        private object m_locker;

        private bool m_disposed;

        public Action<byte[]> OnPacket;
        public Action OnDisconnected;

        public bool Disposed
        {
            get
            {
                return m_disposed;
            }
        }

        public Session(Socket socket)
        {
            m_socket = socket;

            m_sync = new object();

            m_recvBuffer = new byte[ReceiveSize];
            m_packetBuffer = new byte[ReceiveSize];
            m_cursor = 0;

            m_locker = new object();

            m_disposed = false;
        }

        internal void Start()
        {
            WaitForData();
        }

        private void WaitForData()
        {
            if (m_disposed) { return; }

            SocketError error = SocketError.Success;

            m_socket.BeginReceive(m_recvBuffer, 0, m_recvBuffer.Length, SocketFlags.None, out error, PacketCallback, null);

            if (error != SocketError.Success)
            {
                Dispose();
            }
        }

        private void Append(int length)
        {
            if (m_packetBuffer.Length - m_cursor < length)
            {
                int newSize = m_packetBuffer.Length * 2;

                while (newSize < m_cursor + length)
                    newSize *= 2;

                Array.Resize<byte>(ref m_packetBuffer, newSize);
            }

            Crypt.Cipher(m_recvBuffer, 0, length); //cipha

            System.Buffer.BlockCopy(m_recvBuffer, 0, m_packetBuffer, m_cursor, length);


            m_cursor += length;
        }
        private void HandleStream()
        {
            while (m_cursor > 4)//header room
            {
                int packetSize = BitConverter.ToInt32(m_packetBuffer, 0);

                if (packetSize < 2 || packetSize > 4096) //illegal
                {
                    Dispose();
                    return;
                }

                if (m_cursor < packetSize + 4) //header + packet room
                {
                    break;
                }

                byte[] packetBuffer = new byte[packetSize];
                System.Buffer.BlockCopy(m_packetBuffer, 4, packetBuffer, 0, packetSize); //copy packet


                m_cursor -= packetSize + 4; //fix len

                if (m_cursor > 0) //move reamining bytes
                {
                    System.Buffer.BlockCopy(m_packetBuffer, packetSize + 4, m_packetBuffer, 0, m_cursor);
                }

                if (OnPacket != null)
                    OnPacket(packetBuffer);
            }

        }

        private void PacketCallback(IAsyncResult iar)
        {
            if (m_disposed) { return; }

            SocketError error;
            int length = m_socket.EndReceive(iar, out error);

            if (length == 0 || error != SocketError.Success)
            {
                Dispose();
            }
            else
            {
                Append(length);
                HandleStream();
                WaitForData();
            }
        }

        public void Send(byte[] packet)
        {
            if (m_disposed) { return; }

            lock (m_locker)
            {
                int length = packet.Length;

                byte[] final = new byte[length + 4];
                System.Buffer.BlockCopy(packet, 0, final, 4, length);

                var buf = BitConverter.GetBytes(length);
                System.Buffer.BlockCopy(buf, 0, final, 0, buf.Length);

                Crypt.Cipher(final, 0, final.Length); //cipha

                SendRaw(final);
            }
        }
        private void SendRaw(byte[] final)
        {
            if (m_disposed) { return; }

            int offset = 0;

            while (offset < final.Length)
            {
                SocketError outError = SocketError.Success;
                int sent = m_socket.Send(final, offset, final.Length - offset, SocketFlags.None, out outError);

                if (sent == 0 || outError != SocketError.Success)
                {
                    Dispose();
                    return;
                }

                offset += sent;
            }
        }

        public override string ToString()
        {
            string endpoint = null;
            try
            {
                if (m_socket != null)
                    endpoint = m_socket.RemoteEndPoint.ToString();
                else
                    endpoint = base.ToString();
            }
            finally { }

            return endpoint;
        }



        public void Dispose()
        {
            lock (m_sync)
            {
                if (!m_disposed)
                {
                    m_disposed = true;

                    try
                    {
                        m_socket.Shutdown(SocketShutdown.Both);
                        m_socket.Close();
                    }
                    catch { }
                    finally
                    {
                        m_packetBuffer = null;
                        m_recvBuffer = null;
                        m_cursor = 0;

                        m_locker = null;

                        if (OnDisconnected != null)
                            OnDisconnected();
                    }
                }
            }
        }
    }
}