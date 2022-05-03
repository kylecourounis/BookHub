namespace BookHub.Server.Protocol.Messages.Server
{
    using System.Net;

    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Protocol.Enums;

    internal class BuyListingResponse : Message
    {
        internal Order Order;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuyListingResponse"/> class.
        /// </summary>
        public BuyListingResponse(User user, HttpListenerContext context) : base(user, context)
        {
            this.Type = MessageType.BuyListingResponse;
        }

        internal override void Encode()
        {
            this.Data.AddString("order", this.Order.ToString());
        }
    }
}
