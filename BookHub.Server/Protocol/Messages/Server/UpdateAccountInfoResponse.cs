namespace BookHub.Server.Protocol.Messages.Server
{
    using System.Net;
    
    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Protocol.Enums;

    internal class UpdateAccountInfoResponse : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAccountInfoResponse"/> class.
        /// </summary>
        public UpdateAccountInfoResponse(User user, HttpListenerContext context) : base(user, context)
        {
            this.Type = MessageType.UpdateAccountInfoResponse;
        }

        internal override void Encode()
        {
            this.Data.AddString("user", this.User.ToString());
        }
    }
}
