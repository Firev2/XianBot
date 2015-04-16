using System;
using System.IO;

namespace XianServer
{
    public static class Logger
    {
        private static object sLocker = new object();

        public static void Title(string title)
        {
            Console.Title = title;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Write(string format, params object[] objects)
        {
            Console.WriteLine(format, objects);
        }
        public static void Exception(Exception ex)
        {
            string exception = ex.ToString();
            string message = string.Format("[{0}]-----------------{1}{2}{1}", DateTime.Now, Environment.NewLine, exception);

            lock (sLocker)
            {
                File.AppendAllText("EXCEPTIONS.txt", message);
            }

            Write(message);
        }
    }
}
