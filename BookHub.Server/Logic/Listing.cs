namespace BookHub.Server.Logic
{
    using System.Net;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using BookHub.Server.Database.Models;
    using BookHub.Server.Logic.Enums;

    [JsonObject(MemberSerialization.OptIn)]
    internal class Listing
    {
        [JsonProperty] internal long Seed;

        [JsonProperty] internal string ISBN;

        [JsonProperty] internal Condition Condition;

        [JsonProperty] internal double Price;

        [JsonProperty] internal User Seller;
        [JsonProperty] internal string SellersNotes;

        [JsonProperty] internal string Title;
        [JsonProperty] internal string[] Authors;
        [JsonProperty] internal string Publisher;
        [JsonProperty] internal string PublishedDate;
        [JsonProperty] internal string Description;

        [JsonProperty] internal string SmallThumbnail;
        [JsonProperty] internal string Thumbnail;

        internal bool Sold;

        /// <summary>
        /// Gets the maximum price based on <see cref="Listing.Condition"/>.
        /// </summary>
        internal double MaxPrice
        {
            get
            {
                switch (this.Condition)
                {
                    case Condition.Poor:
                    {
                        return 39.99;
                    }
                    case Condition.Fair:
                    {
                        return 49.99;
                    }
                    case Condition.Good:
                    {
                        return 59.99;
                    }
                    case Condition.Excellent:
                    {
                        return 69.99;
                    }
                    default:
                    {
                        return 39.99;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Listing"/> class.
        /// </summary>
        internal Listing()
        {
            // Listing.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Listing"/> class.
        /// </summary>
        internal Listing(long seed)
        {
            this.Seed = seed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Listing"/> class.
        /// </summary>
        internal Listing(string isbn)
        {
            this.ISBN = isbn;

            using (WebClient client = new WebClient())
            {
                JObject searchJson  = JObject.Parse(client.DownloadString($"https://www.googleapis.com/books/v1/volumes?q=isbn%3A{this.ISBN}"));
                
                // If we're searching by ISBN, there should only be one result and that's the book we're looking for
                var book            = JObject.Parse(client.DownloadString(searchJson["items"][0]["selfLink"].ToString()));

                var volumeInfo      = book["volumeInfo"];
                this.Title          = volumeInfo["title"].ToString();
                this.Authors        = volumeInfo["authors"].ToObject<string[]>();
                
                this.Publisher      = volumeInfo["publisher"].ToString();
                this.PublishedDate  = volumeInfo["publishedDate"].ToString();

                if (volumeInfo["description"] != null)
                {
                    this.Description = volumeInfo["description"].ToString();
                }
                
                var imageLinks      = volumeInfo["imageLinks"];

                if (imageLinks != null)
                {
                    this.SmallThumbnail = imageLinks["smallThumbnail"].ToString();
                    this.Thumbnail      = imageLinks["thumbnail"].ToString();
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, ListingDb.JsonSettings);
        }
    }
}
