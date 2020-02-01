using System.Configuration;
using System.Linq;

namespace DeepPrint
{
    internal static class Settings
    {
        public static string PrinterName => ConfigurationManager.AppSettings["printer"];
        public static float WidthRatio => float.Parse(ConfigurationManager.AppSettings["width"]);
        public static float HeightRatio => float.Parse(ConfigurationManager.AppSettings["height"]);
        public static bool IsLandscape => bool.Parse(ConfigurationManager.AppSettings["landscape"]);
        public static double LockedDuration => double.Parse(ConfigurationManager.AppSettings["lockedDuration"]);
        public static string[] FileExtensions => ConfigurationManager.AppSettings["fileExtensions"].Split(',');
    }
}

