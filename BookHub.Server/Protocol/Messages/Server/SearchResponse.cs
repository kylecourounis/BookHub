namespace BookHub.Server.Protocol.Messages.Server
{
    using System.Net;

    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Protocol.Enums;

    internal class SearchResponse : Message
    {
        internal Listing[] Listings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResponse"/> class.
        /// </summary>
        public SearchResponse(User user, HttpListenerContext context) : base(user, context)
        {
            this.Type = MessageType.SearchResponse;
        }

        internal override void Encode()
        {
            foreach (var listing in this.Listings)
            {
                this.Data.AddString(listing.Seed.ToString(), listing.ToString());
            }
        }
    }
}
