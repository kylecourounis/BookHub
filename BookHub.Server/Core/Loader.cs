namespace BookHub.Server.Core
{
    using BookHub.Server.Core.Consoles;
    using BookHub.Server.Core.Events;
    using BookHub.Server.Database;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Network;
    using BookHub.Server.Protocol;

    internal static class Loader
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="Loader"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="Loader"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (Loader.Initialized)
            {
                return;
            }

            Factory.Initialize();

            Mongo.Initialize();

            Users.Initialize();
            Listings.Initialize();
            Orders.Initialize();
            Colleges.Initialize();

            WebServer.Initialize();

            ServerTimers.Initialize();

            Tests.Initialize();
            
            Loader.Initialized = true;
            
            EventsHandler.Initialize();
            Parser.Initialize();
        }
    }
}
