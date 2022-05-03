namespace BookHub.Server.Protocol.Messages.Server
{
    using System.Net;

    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Protocol.Enums;

    internal class AddListingResponse : Message
    {
        internal bool Success;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddListingResponse"/> class.
        /// </summary>
        public AddListingResponse(User user, HttpListenerContext context) : base(user, context)
        {
            this.Type = MessageType.AddListingResponse;
        }

        internal override void Encode()
        {
            this.Data.AddBoolean("success", this.Success);
        }
    }
}
