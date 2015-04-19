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
            string hwid = p.ReadMapleString();
            byte region = p.ReadByte();

            bool result = WvsServer.Instance.AuthCenter.AddClient(hwid);

            if(result)
            {
                //Set hwid if login is successful.
                //if you set it and you have failed addclient()
                //when client dsiconencts it calls removeclient and decrements the client limit
                c.Hwid = p.ReadMapleString();
                c.SendLoginSuccess(region);
            }
            else
            {
                c.Dispose(); //client is useless
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
