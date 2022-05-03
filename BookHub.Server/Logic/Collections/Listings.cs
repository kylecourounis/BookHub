namespace BookHub.Server.Logic.Collections
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Newtonsoft.Json;

    using BookHub.Server.Database;
    using BookHub.Server.Database.Models;
    using BookHub.Server.Logic.Models;

    internal static class Listings
    {
        private static ConcurrentDictionary<long, Listing> Pool;

        private static long Seed;

        /// <summary>
        /// Gets the count.
        /// </summary>
        internal static int Count
        {
            get
            {
                return Listings.Pool.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Listings"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="Listings"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (Listings.Initialized)
            {
                return;
            }

            Listings.Pool = new ConcurrentDictionary<long, Listing>();

            foreach (ListingDb dbEntry in Mongo.Listings.Find(db => true).ToList())
            {
                if (dbEntry != null)
                {
                    Listing listing = new Listing(dbEntry.Seed);

                    JsonConvert.PopulateObject(dbEntry.Listing.ToJson(), listing, ListingDb.JsonSettings);

                    Listings.Add(listing);
                }
            }

            Listings.Seed = Mongo.ListingSeed;

            Console.Write($"{Listings.Count} listings, ");

            Listings.Initialized = true;
        }

        /// <summary>
        /// Adds the specified <see cref="Listing"/> to the cache.
        /// </summary>
        internal static void Add(Listing listing)
        {
            if (Listings.Pool.ContainsKey(listing.Seed))
            {
                if (!Listings.Pool.TryUpdate(listing.Seed, listing, listing))
                {
                    Logger.Error("Unsuccessfully updated the specified listing to the dictionary.");
                }
            }
            else
            {
                if (!Listings.Pool.TryAdd(listing.Seed, listing))
                {
                    Logger.Error("Unsuccessfully added the specified listing to the dictionary.");
                }
            }
        }

        /// <summary>
        /// Removes the specified <see cref="Listing"/> from the cache.
        /// </summary>
        internal static void Remove(Listing listing)
        {
            if (Listings.Pool.ContainsKey(listing.Seed))
            {
                if (!Listings.Pool.TryRemove(listing.Seed, out Listing tmpListing))
                {
                    Logger.Error("Unsuccessfully removed the specified listing from the dictionary.");
                }
                else
                {
                    if (!tmpListing.Equals(listing))
                    {
                        Logger.Error("Successfully removed a listing from the list but the returned listing was not equal to the listing.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Listing"/> using the specified seed.
        /// </summary>
        internal static Listing Get(long seed)
        {
            if (!Listings.Pool.TryGetValue(seed, out Listing listing))
            {
                ListingDb save = ListingDb.Load(seed).GetAwaiter().GetResult();

                if (save != null)
                {
                    listing = Listings.Load(save.Listing.ToJson());

                    if (listing == null)
                    {
                        Logger.Error($"Unable to load listing with the seed {seed}.");
                        return null;
                    }
                }
            }

            return listing;
        }

        /// <summary>
        /// Creates a new <see cref="Listing"/> in the database and caches it.
        /// </summary>
        internal static void Create(Listing listing)
        {
            listing.Seed = Interlocked.Increment(ref Listings.Seed);
            
            Listings.Add(listing);
        }

        /// <summary>
        /// Updates the specified <see cref="Listing"/> in the cache.
        /// </summary>
        internal static void Update(Listing listing)
        {
            if (!Listings.Pool.TryUpdate(listing.Seed, listing, Listings.Get(listing.Seed)))
            {
                Logger.Error($"Error updating listing with seed {listing.Seed}");
            }
        }

        /// <summary>
        /// Saves the specified <see cref="Listing"/> directly to the <see cref="Mongo"/> database.
        /// </summary>
        internal static async void Save(Listing listing)
        {
            if (!await ListingDb.Contains(listing.Seed))
            {
                await ListingDb.Create(listing);
            }

            await ListingDb.Save(listing);
        }

        /// <summary>
        /// Deletes the specified <see cref="Listing"/> directly from the <see cref="Mongo"/> database.
        /// </summary>
        internal static async void Delete(Listing listing)
        {
            Listings.Remove(listing);

            await ListingDb.Delete(listing.Seed);
        }
        
        /// <summary>
        /// Retrieves all the unsold listings that match the conditions defined by the specified predicate.
        /// </summary>
        internal static List<Listing> FindAll(Predicate<Listing> predicate)
        {
            return Listings.Pool.Values.ToList().Where(listing => !listing.Sold).ToList().FindAll(predicate);
        }

        /// <summary>
        /// Retrieves the specified number of unsold listings from the collection.
        /// </summary>
        internal static List<Listing> GetCount(int count)
        {
            return Listings.Pool.Values.ToList().Where(listing => !listing.Sold).Take(count).ToList();
        }

        /// <summary>
        /// Retrieves the specified number of unsold listings at the specified college from the collection.
        /// </summary>
        internal static List<Listing> GetCount(int count, College college)
        {
            return Listings.Pool.Values.ToList().Where(listing => !listing.Sold && listing.Seller.College.Identifier == college.Identifier).Take(count).ToList();
        }

        /// <summary>
        /// Uses the specified JSON to initialize an instance of <see cref="Listing"/>.
        /// </summary>
        private static Listing Load(string json)
        {
            Listing listing = new Listing();
            JsonConvert.PopulateObject(json, listing, ListingDb.JsonSettings);
            return listing;
        }

        /// <summary>
        /// Saves this collection to the <see cref="Mongo"/> database.
        /// </summary>
        internal static void Save()
        {
            Listings.ForEach(listing =>
            {
                try
                {
                    if (listing.Sold)
                    {
                        Listings.Delete(listing);
                    }
                    else
                    {
                        Listings.Save(listing);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error($"{exception.GetType().Name} : did not succeed in saving listing [{listing}].");
                }
            });

            Logger.Info($"Saved {Listings.Count} Listings.");
        }

        /// <summary>
        /// Executes an action on every listing in the collection.
        /// </summary>
        internal static void ForEach(Action<Listing> action)
        {
            int count = 0;

            Parallel.ForEach(Listings.Pool.Values.Reverse(), listing =>
            {
                action.Invoke(listing);
                count++;
            });

            // Logger.Debug($"Executed an action on {count} Books.");
        }
    }
}
