namespace BookHub.Server.Network
{
    using System.Net;
    using System.Text;

    using BookHub.Server.Helpers;
    using BookHub.Server.Protocol;

    internal static class Processor
    {
        internal static int TotalRequests;
        internal static int TotalResponses;

        /// <summary>
        /// Handles the request.
        /// </summary>
        internal static void EndProcess(this HttpListenerContext context)
        {
            Factory.CreateMessage(context, out var message);
            
            Logger.Info($"Packet {message.GetType().Name.Pad()} received from {message.Context.Request.RemoteEndPoint}");

            message.Decode();
            message.Handle();

            Processor.TotalRequests++;
        }

        /// <summary>
        /// Sends the specified message as an HTTP response.
        /// </summary>
        internal static void Send(this Message message)
        {
            message.Encode();

            byte[] buffer = Encoding.UTF8.GetBytes(message.ToString());
            
            var response = message.Context.Response;

            response.ContentLength64 = buffer.Length;
            response.ContentType = "application/json";

            response.AddHeader("Access-Control-Allow-Origin", "*");
            
            var stream = response.OutputStream;
            stream.WriteAsync(buffer, 0, buffer.Length).GetAwaiter().GetResult();
            stream.FlushAsync().GetAwaiter().GetResult();
            stream.Close();
            
            Logger.Info($"Packet {message.GetType().Name.Pad()} sent to {message.Context.Request.RemoteEndPoint}");

            message.Handle();

            Processor.TotalResponses++;
        }
    }
}
