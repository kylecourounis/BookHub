namespace BookHub.Server
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    using BookHub.Server.Core;
    using BookHub.Server.Core.Consoles;
    using BookHub.Server.Helpers;

    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        private static void Main()
        {
            Console.Title = $"{Assembly.GetExecutingAssembly().GetName().Name} | v{Assembly.GetExecutingAssembly().GetName().Version}";

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write(@"
                              ______________________
    ____              _      |  _    _       _      |
   |  _ \            | |     | | |  | |     | |     |
   | |_) | ___   ___ | | __  | | |__| |_   _| |__   |
   |  _ < / _ \ / _ \| |/ /  | |  __  | | | | '_ \  |
   | |_) | (_) | (_) |   <   | | |  | | |_| | |_) | |
   |____/ \___/ \___/|_|\_\  | |_|  |_|\__,_|_.__/  |
                             |______________________|

            " + Environment.NewLine);

            Console.SetOut(new Prefixed());

            Console.WriteLine("Starting..." + Environment.NewLine);

            Loader.Initialize();

            Thread.Sleep(Timeout.Infinite);
        }
        
        /// <summary>
        /// Restarts the program with administrative privileges.
        /// </summary>
        internal static void RestartAsAdmin()
        {
            Thread.Sleep(750);

            if (!Natives.IsElevated)
            {
                Console.WriteLine("Program was not run as administrator, auto-restarting with admin permissions...");

                new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName         = "dotnet",
                        Arguments        = $"\"{Assembly.GetExecutingAssembly().Location}\"",
                        Verb             = "runas",
                        CreateNoWindow   = false,
                        UseShellExecute  = true
                    }
                }.Start();
                
                Environment.Exit(0);
            }
        }
    }
}
