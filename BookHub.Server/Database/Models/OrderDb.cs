namespace BookHub.Server.Database.Models
{
    using System.Threading.Tasks;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Driver;

    using Newtonsoft.Json;

    using BookHub.Server.Helpers.Json;
    using BookHub.Server.Logic;

    internal class OrderDb
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
        [BsonElement("OrderNumber")] internal string OrderNumber;

        [BsonElement("Order")] internal BsonDocument Order;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDb"/> class.
        /// </summary>
        internal OrderDb(long id, string json)
        {
            this.Identifier = id;
            this.Order = BsonDocument.Parse(json);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDb"/> class.
        /// </summary>
        internal OrderDb(Order order) : this(order.Identifier, JsonConvert.SerializeObject(order, OrderDb.JsonSettings))
        {
            this.OrderNumber = order.OrderNumber;
        }

        /// <summary>
        /// Creates the specified User.
        /// </summary>
        internal static async Task Create(Order order)
        {
            await Mongo.Orders.InsertOneAsync(new OrderDb(order));
        }

        /// <summary>
        /// Saves the User to the database.
        /// </summary>
        internal static async Task<OrderDb> Save(Order order)
        {
            var updatedEntity = await Mongo.Orders.FindOneAndUpdateAsync(db => 
                db.Identifier == order.Identifier,
                Builders<OrderDb>.Update.Set(db => db.Order, BsonDocument.Parse(JsonConvert.SerializeObject(order, OrderDb.JsonSettings)))
            );

            if (updatedEntity != null)
            {
                if (updatedEntity.Identifier == order.Identifier)
                {
                    return updatedEntity;
                }
            }

            return null;
        }

        /// <summary>
        /// Loads this instance from the database.
        /// </summary>
        internal static async Task<OrderDb> Load(string orderNumber)
        {
            if (orderNumber != null)
            {
                var entities = await Mongo.Orders.FindAsync(order => order.OrderNumber.Equals(orderNumber));

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
        /// Loads this instance from the database.
        /// </summary>
        internal static async Task<OrderDb> Load(long id)
        {
            if (id > 0)
            {
                var entities = await Mongo.Orders.FindAsync(order => order.Identifier == id);

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
        /// Determines whether this instance contains an order with the specified id.
        /// </summary>
        internal static async Task<bool> Contains(long id)
        {
            if (id > 0)
            {
                var entities = await Mongo.Orders.FindAsync(order => order.Identifier == id);

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
                var result = await Mongo.Orders.DeleteOneAsync(order => order.Identifier == id);

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
        internal bool Deserialize(out Order order)
        {
            if (this.Order != null)
            {
                order = JsonConvert.DeserializeObject<Order>(this.Order.ToJson(), OrderDb.JsonSettings);

                if (order != null)
                {
                    return true;
                }
            }
            else
            {
                order = null;
            }

            return false;
        }
    }
}
