using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianServer.Buffer;
using XianServer.Tools;
using XianServer.User;

namespace XianServer.Packet
{
    public static class PacketFactory
    {
        public static void SendLoginSuccess(this Client client,byte region)
        {
            using (var p = new BufferWriter(CServerMsg.LoginResponse))
            {
                p.WriteInt(); //success code ( 0 )

                OffsetTable offsets = null;

                switch(region)
                {
                    case 0: //sea
                        offsets = OffsetTable.SeaTable;
                        break;
                    case 1: //eu
                        offsets = OffsetTable.EuTable;
                        break;
                }

                if(offsets == null)
                {
                    p.WriteInt(0); 
                }
                else
                {
                    p.WriteInt(offsets.Offsets.Count); //loop count length

                    foreach (var x in offsets.Offsets)  //all offets
                    {
                        p.WriteInt(x.Key); //Map position
                        p.WriteInt(x.Key ^ x.Value); //Real offset value
                    }
                }

                client.Send(p);
            }
        }
        public static void SendLoginFailed(this Client client,int reason)
        {
            using (var p = new BufferWriter(CServerMsg.LoginResponse))
            {
                //1 = no license avaiable
                p.WriteInt(reason); //failiure code

                client.Send(p);
            }
        }

        public static void SendPing(this Client client)
        {
            using (var p = new BufferWriter(CServerMsg.Ping))
            {
                client.Send(p);
            }
        }
    }
}
