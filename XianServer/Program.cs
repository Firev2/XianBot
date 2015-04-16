using System.IO;
using XianServer.Server;

namespace XianServer
{
    public static class Program
    {
        static void Main(string[] args)
        {
            string connection = File.ReadAllText("MySql.txt");

            using (WvsServer.Instance = new WvsServer(7575, connection))
            {
                WvsServer.Instance.Run();
            }
        }
    }
}
