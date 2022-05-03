namespace BookHub.Server.Protocol.Messages.Client
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;
    
    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Network;
    using BookHub.Server.Protocol.Messages.Server;

    internal class SearchRequest : Message
    {
        private string SearchQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequest"/> class.
        /// </summary>
        public SearchRequest(JObject json) : base(json)
        {
            // SearchRequest.
        }

        internal override void Decode()
        {
            this.SearchQuery = this.JSON.GetString("query");
        }

        internal override void Handle()
        {
            string query           = this.SearchQuery.ToLower();
            List<Listing> listings = string.IsNullOrEmpty(this.SearchQuery) ? Listings.GetCount(50, this.User.College) : Listings.FindAll(listing => listing.ISBN.Contains(query) || listing.Title.ToLower().Contains(query) || listing.Authors.Any(author => author.ToLower().Contains(query)) || listing.Seller.Name.ToLower().Contains(query));
            
            new SearchResponse(this.User, this.Context)
            {
                Listings = listings.ToArray()
            }.Send();
        }
    }
}