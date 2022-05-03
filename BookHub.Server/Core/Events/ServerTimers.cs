namespace BookHub.Server.Core.Events
{
    using System.Timers;

    using BookHub.Server.Logic.Collections;

    internal class ServerTimers
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="ServerTimers"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="ServerTimers"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (ServerTimers.Initialized)
            {
                return;
            }
            
            ServerTimers.SaveAll.Start();

            ServerTimers.Initialized = true;
        }

        /// <summary>
        /// An instance of <see cref="Timer"/> that periodically saves all data to the database.
        /// </summary>
        private static Timer SaveAll
        {
            get
            {
                Timer timer = new Timer
                {
                    Interval  = 120000, // 2 minutes
                    AutoReset = true
                };

                timer.Elapsed += delegate
                {
                    Users.Save();
                    Listings.Save();
                    Orders.Save();
                    Colleges.Save();
                };

                return timer;
            }
        }
    }
}
