using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security.Permissions;

namespace DeepPrint
{
    internal static class Watcher
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run(string path)
        {
            var stopwatch = Stopwatch.StartNew();
            
            using var watcher = new FileSystemWatcher(path)
            {
                NotifyFilter = NotifyFilters.FileName,
                EnableRaisingEvents = true
            };
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"

     ▓█████▄ ▓█████ ▓█████  ██▓███      ██▓███   ██▀███   ██▓ ███▄    █ ▄▄▄█████▓
    ▒██▀ ██▌▓█   ▀ ▓█   ▀ ▓██░  ██▒   ▓██░  ██▒▓██ ▒ ██▒▓██▒ ██ ▀█   █ ▓  ██▒ ▓▒
    ░██   █▌▒███   ▒███   ▓██░ ██▓▒   ▓██░ ██▓▒▓██ ░▄█ ▒▒██▒▓██  ▀█ ██▒▒ ▓██░ ▒░
    ░▓█▄   ▌▒▓█  ▄ ▒▓█  ▄ ▒██▄█▓▒ ▒   ▒██▄█▓▒ ▒▒██▀▀█▄  ░██░▓██▒  ▐▌██▒░ ▓██▓ ░ 
    ░▒████▓ ░▒████▒░▒████▒▒██▒ ░  ░   ▒██▒ ░  ░░██▓ ▒██▒░██░▒██░   ▓██░  ▒██▒ ░ 
     ▒▒▓  ▒ ░░ ▒░ ░░░ ▒░ ░▒▓▒░ ░  ░   ▒▓▒░ ░  ░░ ▒▓ ░▒▓░░▓  ░ ▒░   ▒ ▒   ▒ ░░   
     ░ ▒  ▒  ░ ░  ░ ░ ░  ░░▒ ░        ░▒ ░       ░▒ ░ ▒░ ▒ ░░ ░░   ░ ▒░    ░    
     ░ ░  ░    ░      ░   ░░          ░░         ░░   ░  ▒ ░   ░   ░ ░   ░      
       ░       ░  ░   ░  ░                        ░      ░           ░          
     ░                                                                          
                                                                                                                               
");
            Console.ResetColor();
            Console.WriteLine($"Printer: {Settings.PrinterName}");
            Console.WriteLine($"Path:    {path}");
            Console.WriteLine($"Files:   {string.Join(", ", Settings.FileExtensions)}");

            watcher.Created += (s, e) =>
            {
                if (Settings.FileExtensions.Any(x => e.Name.EndsWith(x, StringComparison.CurrentCulture)))
                {
                    if (stopwatch.Elapsed < TimeSpan.FromSeconds(Settings.LockedDuration))
                    {
                        Console.WriteLine($"Skipping => {e.Name} (too soon)");
                    }
                    else
                    {
                        Console.WriteLine($"Printing => {e.Name}");
                        stopwatch.Restart();

                        using var document = new PrintDocument();
                        document.DefaultPageSettings.Landscape = Settings.IsLandscape;
                        document.DefaultPageSettings.PrinterSettings.PrinterName = Settings.PrinterName;
                        document.PrintPage += (_, a) =>
                        {
                            var image = Image.FromFile(e.FullPath);
                            a.Graphics.DrawImage(image, 0f, 0f, image.Width * Settings.WidthRatio, image.Height * Settings.HeightRatio);
                        };
                        document.Print();
                    }
                }
                else
                {
                    Console.WriteLine($"Ignoring => {e.Name} (not a recognised file extension)");
                }
            };

            Console.WriteLine($"{Environment.NewLine}Press 'q' to quit.");
            while (Console.ReadKey().Key != ConsoleKey.Q) { }
        }
    }
}

