using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianServer.Buffer;
using XianServer.Network;
using XianServer.Packet;
using XianServer.Server;

namespace XianServer.User
{
    public sealed class Client : IDisposable
    {
        private Session m_session;

        public string Hwid { get; set; }
        public string Name { get; private set; }

        public Client (Session session)
        {
            m_session = session;
            m_session.OnPacket = OnPacket;
            m_session.OnDisconnected = Dispose;

            Name = session.ToString();
            WvsServer.Instance.AddClient(this);
        }

        private void OnPacket(byte[] buffer)
        {
            var p = new Buffer.BufferReader(buffer);
            short opcode = p.ReadShort();

            try
            {
                switch (opcode)
                {
                    case CClientMsg.LoginRequest:
                        PacketHandler.HandleLoginRequest(this, p);
                        break;
                    case CClientMsg.Ping:
                        //ping
                        break;
                    case CClientMsg.JewShit:
                        PacketHandler.HandleJewShit(this, p);
                        break;
                    default:
                        {
                            string buffer_string = BitConverter.ToString(buffer);
                            Logger.Write("Unknown packet: {0}", buffer_string);
                            break;
                        }
                }
            }
            catch( Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public void Send(BufferWriter packet)
        {
            var final = packet.ToArray();
            m_session.Send(final);
        }

        public void Dispose()
        {
            WvsServer.Instance.RemoveClient(this);
        }
    }
}
