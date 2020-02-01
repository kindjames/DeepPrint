using System.IO;

namespace DeepPrint
{
    internal partial class MainClass
    {
        public static void Main(string[] args)
        {
            Watcher.Run(Directory.GetCurrentDirectory());
        }
    }
}

