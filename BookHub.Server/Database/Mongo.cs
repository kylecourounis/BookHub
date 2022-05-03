namespace BookHub.Server.Database
{
    using System;

    using MongoDB.Driver;

    using BookHub.Server.Database.Models;

    internal class Mongo
    {
        internal static IMongoCollection<UserDb> Users;
        internal static IMongoCollection<ListingDb> Listings;
        internal static IMongoCollection<OrderDb> Orders;
        internal static IMongoCollection<CollegeDb> Colleges;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Mongo"/> has been already initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the <see cref="Mongo"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (Mongo.Initialized)
            {
                return;
            }

            var mongoClient = new MongoClient("mongodb://127.0.0.1:27017/");
            var mongoDb     = mongoClient.GetDatabase("BookHub");
            
            Console.WriteLine($"Connected to Mongo Database at {mongoClient.Settings.Server.Host}." + Environment.NewLine);
            
            // Create the collections if they don't already exist
            if (mongoDb.GetCollection<UserDb>("Users") == null)
            {
                mongoDb.CreateCollection("Users");
            }

            if (mongoDb.GetCollection<ListingDb>("Listings") == null)
            {
                mongoDb.CreateCollection("Listings");
            }

            if (mongoDb.GetCollection<OrderDb>("Orders") == null)
            {
                mongoDb.CreateCollection("Orders");
            }

            if (mongoDb.GetCollection<CollegeDb>("Colleges") == null)
            {
                mongoDb.CreateCollection("Colleges");
            }

            Mongo.Users    = mongoDb.GetCollection<UserDb>("Users");
            Mongo.Listings = mongoDb.GetCollection<ListingDb>("Listings");
            Mongo.Orders   = mongoDb.GetCollection<OrderDb>("Orders");
            Mongo.Colleges = mongoDb.GetCollection<CollegeDb>("Colleges");

            Mongo.Users.Indexes.CreateOne(Builders<UserDb>.IndexKeys.Combine(
                Builders<UserDb>.IndexKeys.Ascending(db => db.Identifier)),

                new CreateIndexOptions
                {
                    Name = "entityIds",
                    Background = true
                }
            );

            Mongo.Listings.Indexes.CreateOne(Builders<ListingDb>.IndexKeys.Combine(
                Builders<ListingDb>.IndexKeys.Ascending(db => db.Seed)),

                new CreateIndexOptions
                {
                    Name = "entityIds",
                    Background = true
                }
            );

            Mongo.Orders.Indexes.CreateOne(Builders<OrderDb>.IndexKeys.Combine(
                    Builders<OrderDb>.IndexKeys.Ascending(db => db.Identifier)),

                new CreateIndexOptions
                {
                    Name = "entityIds",
                    Background = true
                }
            );

            Mongo.Colleges.Indexes.CreateOne(Builders<CollegeDb>.IndexKeys.Combine(
                    Builders<CollegeDb>.IndexKeys.Ascending(db => db.Identifier)),

                new CreateIndexOptions
                {
                    Name = "entityIds",
                    Background = true
                }
            );

            Mongo.Initialized = true;
        }


        /// <summary>
        /// Gets the seed for the users collection.
        /// </summary>
        internal static long UserSeed
        {
            get
            {
                return Mongo.Users.Find(db => true)
                           .Sort(Builders<UserDb>.Sort.Descending(db => db.Identifier))
                           .Limit(1)
                           .SingleOrDefault()?.Identifier ?? 0;
            }
        }

        /// <summary>
        /// Gets the seed for the listings collection.
        /// </summary>
        internal static long ListingSeed
        {
            get
            {
                return Mongo.Listings.Find(db => true)
                           .Sort(Builders<ListingDb>.Sort.Descending(db => db.Seed))
                           .Limit(1)
                           .SingleOrDefault()?.Seed ?? 0;
            }
        }

        /// <summary>
        /// Gets the seed for the orders collection.
        /// </summary>
        internal static long OrderSeed
        {
            get
            {
                return Mongo.Orders.Find(db => true)
                    .Sort(Builders<OrderDb>.Sort.Descending(db => db.Identifier))
                    .Limit(1)
                    .SingleOrDefault()?.Identifier ?? 0;
            }
        }

        /// <summary>
        /// Gets the seed for the colleges collection.
        /// </summary>
        internal static long CollegeSeed
        {
            get
            {
                return Mongo.Colleges.Find(db => true)
                    .Sort(Builders<CollegeDb>.Sort.Descending(db => db.Identifier))
                    .Limit(1)
                    .SingleOrDefault()?.Identifier ?? 0;
            }
        }
    }
}
