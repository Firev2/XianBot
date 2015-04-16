using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianServer.Network
{
    public static class Crypt
    {
        public const int Seed = 0x1337;

        public static void Encrypt(byte[] data, int seed)
        {
#if FAGGOT
            int len = data.Length;

            byte[] seedKey = BitConverter.GetBytes(seed);
            byte[] lenKey = BitConverter.GetBytes(len);

            int round = 0;

            for (int i = 0; i < len; i++)
            {
                if (round >= 4)
                    round = 0;

                byte input = data[i];

                input = (byte)(~input);
                input ^= seedKey[round];
                input ^= lenKey[round];
                input = (byte)((input << 3) | (input >> 5));

                data[i] = input;
                round++;
            }
#endif
        }
        public static void Decrypt(byte[] data, int seed)
        {
#if FAGGOT
            int len = data.Length;

            byte[] seedKey = BitConverter.GetBytes(seed);
            byte[] lenKey = BitConverter.GetBytes(len);

            int round = 0;

            for (int i = 0; i < len; i++)
            {
                if (round >= 4)
                    round = 0;

                byte input = data[i];

                input = (byte)((input >> 3) | (input << 5));
                input ^= lenKey[round];
                input ^= seedKey[round];
                input = (byte)(~input);


                data[i] = input;

                round++;
            }
#endif
        }
    }
}
