namespace BookHub.Server.Protocol.Messages.Client
{
    using Newtonsoft.Json.Linq;
    
    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Logic.Enums;
    using BookHub.Server.Network;
    using BookHub.Server.Protocol.Messages.Server;

    internal class AddListingRequest : Message
    {
        private string ISBN;
        private Condition Condition;
        private double Price;
        private string SellersNotes;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddListingRequest"/> class.
        /// </summary>
        public AddListingRequest(JObject json) : base(json)
        {
            // AddListingRequest.
        }

        internal override void Decode()
        {
            this.ISBN         = this.JSON.GetString("isbn").Replace("-", "");
            this.Condition    = (Condition)this.JSON.GetInt("condition");
            this.Price        = this.JSON.GetDouble("price");
            this.SellersNotes = this.JSON.GetString("sellersNotes");
        }

        internal override void Handle()
        {
            AddListingResponse response = new AddListingResponse(this.User, this.Context);

            try
            {
                Listing listing = new Listing(this.ISBN)
                {
                    Condition    = this.Condition,
                    Price        = this.Price,
                    SellersNotes = this.SellersNotes,
                    Seller       = this.User
                };

                listing.Seller.SerializeAllInfo = false;

                Listings.Create(listing);

                listing.Seller.SerializeAllInfo = true;

                response.Success = true;
            }
            catch
            {
                response.Success = false;
            }
            finally
            {
                response.Send();
            }
        }
    }
}
