namespace BookHub.Server.Logic
{
    using System;

    using Newtonsoft.Json;

    using BookHub.Server.Database.Models;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Logic.Enums;
    
    [JsonObject(MemberSerialization.OptIn)]
    internal class Order
    {
        [JsonProperty] internal long Identifier;

        [JsonProperty] internal string OrderNumber;

        [JsonProperty] internal long BuyerID;

        [JsonProperty] internal long SellerID;

        [JsonProperty] internal Listing Listing;

        [JsonProperty] internal DateTime Timestamp;

        [JsonProperty] internal OrderState State;

        /// <summary>
        /// Gets the buyer using <see cref="Order.BuyerID"/>
        /// </summary>
        internal User Buyer
        {
            get
            {
                return Users.Get(this.BuyerID);
            }
        }

        /// <summary>
        /// Gets the seller using the <see cref="Order.SellerID"/>
        /// </summary>
        internal User Seller
        {
            get
            {
                return Users.Get(this.SellerID);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        internal Order()
        {
            // Order.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        internal Order(User buyer, Listing listing)
        {
            this.Listing   = listing;
            this.BuyerID   = buyer.Identifier;
            this.SellerID  = listing.Seller.Identifier;
            this.Timestamp = DateTime.Now;
            this.State     = OrderState.Processing;
        }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        internal void Process()
        {
            if (this.Buyer.PaymentInfo.IsExpired)
            {
                this.State = OrderState.Failed;
            }

            // If we were connected to a payment API, this is where some of that logic would go
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, OrderDb.JsonSettings);
        }
    }
}
