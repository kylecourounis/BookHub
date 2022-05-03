namespace BookHub.Server.Database.Models
{
    using System.Threading.Tasks;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    using Newtonsoft.Json;

    using BookHub.Server.Helpers.Json;
    using BookHub.Server.Logic;

    internal class UserDb
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

        [BsonElement("Identifier")] internal long Identifier;

        [BsonElement("Profile")] internal BsonDocument Profile;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDb"/> class.
        /// </summary>
        internal UserDb(long id, string json)
        {
            this.Identifier = id;
            this.Profile = BsonDocument.Parse(json);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDb"/> class.
        /// </summary>
        internal UserDb(User user) : this(user.Identifier, JsonConvert.SerializeObject(user, UserDb.JsonSettings))
        {
            // UserDb.
        }

        /// <summary>
        /// Creates the specified User.
        /// </summary>
        internal static async Task Create(User user)
        {
            await Mongo.Users.InsertOneAsync(new UserDb(user));
        }

        /// <summary>
        /// Saves the User to the database.
        /// </summary>
        internal static async Task<UserDb> Save(User user)
        {
            var updatedEntity = await Mongo.Users.FindOneAndUpdateAsync(db => 
                db.Identifier == user.Identifier,
                Builders<UserDb>.Update.Set(db => db.Profile, BsonDocument.Parse(JsonConvert.SerializeObject(user, UserDb.JsonSettings)))
            );

            if (updatedEntity != null)
            {
                if (updatedEntity.Identifier == user.Identifier)
                {
                    return updatedEntity;
                }
            }

            return null;
        }

        /// <summary>
        /// Loads this instance from the database.
        /// </summary>
        internal static async Task<UserDb> Load(long id)
        {
            if (id > 0)
            {
                var entities = await Mongo.Users.FindAsync(user => user.Identifier == id);

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
        /// Determines whether this instance contains a user with the specified id.
        /// </summary>
        internal static async Task<bool> Contains(long id)
        {
            if (id > 0)
            {
                var entities = await Mongo.Users.FindAsync(user => user.Identifier == id);

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
        internal static async Task<bool> Delete(long id)
        {
            if (id > 0)
            {
                var result = await Mongo.Users.DeleteOneAsync(user => user.Identifier == id);

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
        internal bool Deserialize(out User user)
        {
            if (this.Profile != null)
            {
                user = JsonConvert.DeserializeObject<User>(this.Profile.ToJson(), UserDb.JsonSettings);

                if (user != null)
                {
                    return true;
                }
            }
            else
            {
                user = null;
            }

            return false;
        }
    }
}
