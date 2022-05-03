namespace BookHub.Server.Database.Models
{
    using System.Threading.Tasks;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    using Newtonsoft.Json;

    using BookHub.Server.Logic;
    using BookHub.Server.Logic.Models;

    internal class CollegeDb
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
            Formatting            = Formatting.Indented
        };

        [BsonId] internal BsonObjectId Id;

        [BsonElement("Identifier")] internal long Identifier;

        [BsonElement("College")] internal BsonDocument College;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollegeDb"/> class.
        /// </summary>
        internal CollegeDb(long id, string json)
        {
            this.Identifier = id;
            this.College    = BsonDocument.Parse(json);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollegeDb"/> class.
        /// </summary>
        internal CollegeDb(College college) : this(college.Identifier, JsonConvert.SerializeObject(college, CollegeDb.JsonSettings))
        {
            // CollegeDb.
        }

        /// <summary>
        /// Creates the specified college.
        /// </summary>
        internal static async Task Create(College college)
        {
            await Mongo.Colleges.InsertOneAsync(new CollegeDb(college));
        }

        /// <summary>
        /// Saves the college to the database.
        /// </summary>
        internal static async Task<CollegeDb> Save(College college)
        {
            var updatedEntity = await Mongo.Colleges.FindOneAndUpdateAsync(db => 
                db.Identifier == college.Identifier,
                Builders<CollegeDb>.Update.Set(db => db.College, BsonDocument.Parse(JsonConvert.SerializeObject(college, CollegeDb.JsonSettings)))
            );

            if (updatedEntity != null)
            {
                if (updatedEntity.Identifier == college.Identifier)
                {
                    return updatedEntity;
                }
            }

            return null;
        }

        /// <summary>
        /// Loads this instance from the database.
        /// </summary>
        internal static async Task<CollegeDb> Load(long id)
        {
            if (id > 0)
            {
                var entities = await Mongo.Colleges.FindAsync(college => college.Identifier == id);

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
        /// Deletes this instance from the database.
        /// </summary>
        internal static async Task<bool> Delete(long id)
        {
            if (id > 0)
            {
                var result = await Mongo.Colleges.DeleteOneAsync(college => college.Identifier == id);

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
        internal bool Deserialize(out College college)
        {
            if (this.College != null)
            {
                college = JsonConvert.DeserializeObject<College>(this.College.ToJson(), CollegeDb.JsonSettings);

                if (college != null)
                {
                    return true;
                }
            }
            else
            {
                college = null;
            }

            return false;
        }
    }
}
