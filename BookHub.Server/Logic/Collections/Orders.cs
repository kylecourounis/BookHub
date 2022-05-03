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
    using BookHub.Server.Helpers;

    internal static class Orders
    {
        private static ConcurrentDictionary<long, Order> Pool;

        private static long Seed;

        /// <summary>
        /// Gets the count.
        /// </summary>
        internal static int Count
        {
            get
            {
                return Orders.Pool.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Orders"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="Orders"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (Orders.Initialized)
            {
                return;
            }

            Orders.Pool = new ConcurrentDictionary<long, Order>();

            foreach (OrderDb dbEntry in Mongo.Orders.Find(db => true).ToList())
            {
                if (dbEntry != null)
                {
                    dbEntry.Deserialize(out Order order);
                    Orders.Add(order);
                }
            }

            Orders.Seed = Mongo.OrderSeed;

            Orders.Initialized = true;
        }

        /// <summary>
        /// Adds the specified <see cref="Order"/> to the cache.
        /// </summary>
        internal static void Add(Order order)
        {
            if (Orders.Pool.ContainsKey(order.Identifier))
            {
                if (!Orders.Pool.TryUpdate(order.Identifier, order, order))
                {
                    Logger.Error("Unsuccessfully updated the specified order to the dictionary.");
                }
            }
            else
            {
                if (!Orders.Pool.TryAdd(order.Identifier, order))
                {
                    Logger.Error("Unsuccessfully added the specified order to the dictionary.");
                }
            }
        }

        /// <summary>
        /// Removes the specified <see cref="Order"/> from the cache.
        /// </summary>
        internal static void Remove(Order order)
        {
            if (Orders.Pool.ContainsKey(order.Identifier))
            {
                if (!Orders.Pool.TryRemove(order.Identifier, out Order tmpOrder))
                {
                    Logger.Error("Unsuccessfully removed the specified order from the dictionary.");
                }
                else
                {
                    if (!tmpOrder.Equals(order))
                    {
                        Logger.Error("Successfully removed a order from the list but the returned order was not equal to the order.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Order"/> using the specified seed.
        /// </summary>
        internal static Order Get(long seed)
        {
            if (!Orders.Pool.TryGetValue(seed, out Order order))
            {
                OrderDb save = OrderDb.Load(seed).GetAwaiter().GetResult();

                if (save != null)
                {
                    order = Orders.Load(save.Order.ToJson());

                    if (order == null)
                    {
                        Logger.Error($"Unable to load order with the seed {seed}.");
                        return null;
                    }
                }
            }

            return order;
        }

        /// <summary>
        /// Creates a new <see cref="Order"/> in the database and caches it.
        /// </summary>
        internal static void Create(Order order)
        {
            order.Identifier  = Interlocked.Increment(ref Orders.Seed);
            order.OrderNumber = Utils.GenerateRandomString(20);
            
            Orders.Add(order);
        }

        /// <summary>
        /// Updates the specified <see cref="Order"/> in the cache.
        /// </summary>
        internal static void Update(Order order)
        {
            if (!Orders.Pool.TryUpdate(order.Identifier, order, Orders.Get(order.Identifier)))
            {
                Logger.Error($"Error updating order with ID {order.Identifier}");
            }
        }

        /// <summary>
        /// Saves the specified <see cref="Order"/> directly to the <see cref="Mongo"/> database.
        /// </summary>
        internal static async void Save(Order order)
        {
            if (!await OrderDb.Contains(order.Identifier))
            {
                await OrderDb.Create(order);
            }

            await OrderDb.Save(order);
        }

        /// <summary>
        /// Deletes the specified <see cref="Order"/> directly from the <see cref="Mongo"/> database.
        /// </summary>
        internal static async void Delete(Order order)
        {
            await OrderDb.Delete(order.Identifier);
        }
        
        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        internal static List<Order> FindAll(Predicate<Order> predicate)
        {
            return Orders.Pool.Values.ToList().FindAll(predicate);
        }

        /// <summary>
        /// Retrieves the specified number of elements from the values in the collection.
        /// </summary>
        internal static List<Order> GetCount(int count)
        {
            return Orders.Pool.Values.ToList().Take(count).ToList();
        }

        /// <summary>
        /// Uses the specified JSON to initialize an instance of <see cref="Order"/>.
        /// </summary>
        private static Order Load(string json)
        {
            Order order = new Order();
            JsonConvert.PopulateObject(json, order, OrderDb.JsonSettings);
            return order;
        }

        /// <summary>
        /// Saves this collection to the <see cref="Mongo"/> database.
        /// </summary>
        internal static void Save()
        {
            Orders.ForEach(order =>
            {
                try
                {
                    Orders.Save(order);
                }
                catch (Exception exception)
                {
                    Logger.Error($"{exception.GetType().Name} : did not succeed in saving order [{order}].");
                }
            });

            Logger.Info($"Saved {Orders.Count} Orders.");
        }

        /// <summary>
        /// Executes an action on every order in the collection.
        /// </summary>
        internal static void ForEach(Action<Order> action)
        {
            int count = 0;

            Parallel.ForEach(Orders.Pool.Values.Reverse(), order =>
            {
                action.Invoke(order);
                count++;
            });

            // Logger.Debug($"Executed an action on {count} Books.");
        }
    }
}
