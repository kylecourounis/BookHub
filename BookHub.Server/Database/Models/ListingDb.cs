namespace BookHub.Server.Database.Models
{
    using System.Threading.Tasks;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    using Newtonsoft.Json;

    using BookHub.Server.Helpers.Json;
    using BookHub.Server.Logic;

    internal class ListingDb
    {
        /// <summary>
        /// The settings for the <see cref="JsonConvert"/> class.
        /// </summary>
        [BsonIgnore]
        internal static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling      = TypeNameHandling.None,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling  = DefaultValueHandling.Include,
            NullValueHandling     = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting            = Formatting.Indented,
            ContractResolver      = new ShouldSerializeContractResolver()
        };

        [BsonId] internal BsonObjectId Id;

        [BsonElement("Seed")] internal long Seed;

        [BsonElement("Listing")] internal BsonDocument Listing;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListingDb"/> class.
        /// </summary>
        internal ListingDb(long seed, string json)
        {
            this.Seed    = seed;
            this.Listing = BsonDocument.Parse(json);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListingDb"/> class.
        /// </summary>
        internal ListingDb(Listing listing) : this(listing.Seed, JsonConvert.SerializeObject(listing, ListingDb.JsonSettings))
        {
            // ListingDb.
        }

        /// <summary>
        /// Creates the specified listing.
        /// </summary>
        internal static async Task Create(Listing listing)
        {
            await Mongo.Listings.InsertOneAsync(new ListingDb(listing));
        }

        /// <summary>
        /// Saves the listing to the database.
        /// </summary>
        internal static async Task<ListingDb> Save(Listing listing)
        {
            var updatedEntity = await Mongo.Listings.FindOneAndUpdateAsync(db => 
                db.Seed == listing.Seed,
                Builders<ListingDb>.Update.Set(db => db.Listing, BsonDocument.Parse(JsonConvert.SerializeObject(listing, ListingDb.JsonSettings)))
            );

            if (updatedEntity != null)
            {
                if (updatedEntity.Seed == listing.Seed)
                {
                    return updatedEntity;
                }
            }

            return null;
        }

        /// <summary>
        /// Loads this instance from the database.
        /// </summary>
        internal static async Task<ListingDb> Load(long seed)
        {
            if (seed > 0)
            {
                var entities = await Mongo.Listings.FindAsync(listing => listing.Seed == seed);

                if (entities != null)
                {
                    var entity = entities.FirstOrDefault();

                    if (entity != null)
                    {
                        return entity;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether this instance contains a listing with the specified seed.
        /// </summary>
        internal static async Task<bool> Contains(long seed)
        {
            if (seed > 0)
            {
                var entities = await Mongo.Listings.FindAsync(listing => listing.Seed == seed);

                if (entities != null)
                {
                    return await entities.AnyAsync();
                }
            }

            return false;
        }

        /// <summary>
        /// Deletes this instance from the database.
        /// </summary>
        internal static async Task<bool> Delete(long seed)
        {
            if (seed > 0)
            {
                var result = await Mongo.Listings.DeleteOneAsync(listing => listing.Seed == seed);

                if (result.IsAcknowledged)
                {
                    if (result.DeletedCount > 0)
                    {
                        return result.DeletedCount == 1;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Deserializes the specified entity.
        /// </summary>
        internal bool Deserialize(out Listing listing)
        {
            if (this.Listing != null)
            {
                listing = JsonConvert.DeserializeObject<Listing>(this.Listing.ToJson(), ListingDb.JsonSettings);

                if (listing != null)
                {
                    return true;
                }
            }
            else
            {
                listing = null;
            }

            return false;
        }
    }
}
