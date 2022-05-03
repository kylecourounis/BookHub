namespace BookHub.Server.Core.Events
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using BookHub.Server.Logic.Collections;

    internal static class EventsHandler
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool enabled);

        private static EventHandler ExitHandler;
        private delegate void EventHandler();

        /// <summary>
        /// Gets a value indicating whether this <see cref="EventsHandler"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="EventsHandler"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (EventsHandler.Initialized)
            {
                return;
            }

            EventsHandler.ExitHandler += EventsHandler.Exit;
            EventsHandler.SetConsoleCtrlHandler(EventsHandler.ExitHandler, true);

            EventsHandler.Initialized = true;
        }

        /// <summary>
        /// Shuts down the server.
        /// </summary>
        internal static void Exit()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Shutting down...");

            Logger.Info("Server is shutting down...");

            Task.Run(() =>
            {
                Users.Save();
                Listings.Save();
                Orders.Save();
                Colleges.Save();
            });
            
            Thread.Sleep(2000);
            
            Environment.Exit(0);
        }
    }
}
