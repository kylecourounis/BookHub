namespace BookHub.Server.Protocol.Messages.Client
{
    using Newtonsoft.Json.Linq;
    
    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Logic.Enums;
    using BookHub.Server.Network;
    using BookHub.Server.Protocol.Messages.Server;

    internal class BuyListingRequest : Message
    {
        private int Seed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuyListingRequest"/> class.
        /// </summary>
        public BuyListingRequest(JObject json) : base(json)
        {
            // BuyListingRequest.
        }

        internal override void Decode()
        {
            this.Seed = this.JSON.GetInt("seed");
        }

        internal override void Handle()
        {
            Listing listing = Listings.Get(this.Seed);
            
            Order order = new Order(this.User, listing);
            order.Process();

            if (order.State != OrderState.Failed)
            {
                listing.Sold = true;

                listing.Seller.SerializeAllInfo = false;

                Orders.Create(order);
                this.User.Orders.Add(order);

                listing.Seller.SerializeAllInfo = true;

                new BuyListingResponse(this.User, this.Context)
                {
                    Order = order
                }.Send();
            }
        }
    }
}
