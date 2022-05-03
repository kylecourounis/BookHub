namespace BookHub.Server.Network
{
    using System;
    using System.Net;
    using System.Threading;
    
    internal static class WebServer
    {
        private static HttpListener Listener;

        /// <summary>
        /// Gets a value indicating whether this <see cref="WebServer"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="WebServer"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (WebServer.Initialized)
            {
                return;
            }

            try
            {
                WebServer.Listener = new HttpListener();
                WebServer.Listener.Prefixes.Add("http://*:8080/");
                WebServer.Listener.Start();

                WebServer.Initialized = true;

                Console.WriteLine("Web Server is listening on 0.0.0.0:8080!" + Environment.NewLine);

                new Thread(() =>
                {
                    var result = WebServer.Listener.BeginGetContext(WebServer.StartProcess, WebServer.Listener);
                    result.AsyncWaitHandle.WaitOne();
                }).Start();
            }
            catch
            {
                Console.WriteLine("Failed to start the listener!");
                
                Program.RestartAsAdmin();
            }
        }

        /// <summary>
        /// Processes the received request.
        /// </summary>
        private static void StartProcess(IAsyncResult result)
        {
            try
            {
                var listener = (HttpListener)result.AsyncState;
                var context = listener.EndGetContext(result);

                context.EndProcess();
            }
            catch (Exception ex)
            {
                Logger.Error($"A {ex.GetType()} occurred while processing the request!");
                Logger.Error(ex.StackTrace);
            }
            finally
            {
                WebServer.Listener.BeginGetContext(WebServer.StartProcess, WebServer.Listener);
            }
        }
    }
}
