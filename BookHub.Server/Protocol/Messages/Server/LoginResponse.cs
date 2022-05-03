namespace BookHub.Server.Protocol.Messages.Server
{
    using System.Net;
    
    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Protocol.Enums;

    internal class LoginResponse : Message
    {
        internal int Error;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginResponse"/> class.
        /// </summary>
        public LoginResponse(User user, HttpListenerContext context) : base(user, context)
        {
            this.Type = MessageType.LoginResponse;
        }

        internal override void Encode()
        {
            this.Data.AddInt("errorCode", this.Error);

            if (this.User != null)
            {
                this.Data.AddString("user", this.User.ToString());
            }
        }
    }
}
