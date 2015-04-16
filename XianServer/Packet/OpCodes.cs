namespace XianServer.Packet
{
    public static class CServerMsg
    {
        public const short LoginResponse = 0;
        public const short Ping = 1;
    }
    public static class CClientMsg
    {
        public const short LoginRequest = 0;
        public const short Ping = 1;
        public const short JewShit = 2;
    }
}
