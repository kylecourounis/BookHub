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
    
    internal static class Users
    {
        private static ConcurrentDictionary<long, User> Pool;

        private static long Seed;

        /// <summary>
        /// Gets the count.
        /// </summary>
        internal static int Count
        {
            get
            {
                return Users.Pool.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Users"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="Users"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (Users.Initialized)
            {
                return;
            }

            Users.Pool = new ConcurrentDictionary<long, User>();

            foreach (UserDb dbEntry in Mongo.Users.Find(db => true).ToList())
            {
                if (dbEntry != null)
                {
                    User user = new User(dbEntry.Identifier);

                    JsonConvert.PopulateObject(dbEntry.Profile.ToJson(), user, UserDb.JsonSettings);

                    Users.Add(user);
                }
            }
            
            Users.Seed = Mongo.UserSeed;

            Console.Write($"[+] Cached {Users.Count} users, ");

            Users.Initialized = true;
        }

        /// <summary>
        /// Adds the specified <see cref="User"/> to the cache.
        /// </summary>
        internal static void Add(User user)
        {
            if (Users.Pool.ContainsKey(user.Identifier))
            {
                if (!Users.Pool.TryUpdate(user.Identifier, user, user))
                {
                    Logger.Error("Unsuccessfully updated the specified user to the dictionary.");
                }
            }
            else
            {
                if (!Users.Pool.TryAdd(user.Identifier, user))
                {
                    Logger.Error("Unsuccessfully added the specified user to the dictionary.");
                }
            }
        }

        /// <summary>
        /// Removes the specified <see cref="User"/> from the cache.
        /// </summary>
        internal static void Remove(User user)
        {
            if (Users.Pool.ContainsKey(user.Identifier))
            {
                if (!Users.Pool.TryRemove(user.Identifier, out User tmpUser))
                {
                    Logger.Error("Unsuccessfully removed the specified user from the dictionary.");
                }
                else
                {
                    if (!tmpUser.Equals(user))
                    {
                        Logger.Error("Successfully removed a user from the list but the returned user was not equal to the user.");
                    }
                }
            }

            user.Save();
        }

        /// <summary>
        /// Gets a <see cref="User"/> with the specified identifier from either the cache or the <see cref="Mongo"/> database.
        /// </summary>
        internal static User Get(long id)
        {
            if (!Users.Pool.TryGetValue(id, out User user))
            {
                UserDb save = UserDb.Load(id).GetAwaiter().GetResult();

                if (save != null)
                {
                    user = Users.Load(save.Profile.ToJson());

                    if (user == null)
                    {
                        Logger.Error($"Unable to load account with the ID {id}.");
                        return null;
                    }
                }
            }

            return user;
        }

        /// <summary>
        /// Gets the <see cref="User"/> with the specified email address from either the cache or the <see cref="Mongo"/> database.
        /// </summary>
        internal static User Get(string email)
        {
            User u = Users.Pool.Values.ToList().Find(u => u.Email.Equals(email));

            if (u != null)
            {
                long id = u.Identifier;

                if (!Users.Pool.TryGetValue(id, out User user))
                {
                    UserDb save = UserDb.Load(id).GetAwaiter().GetResult();

                    if (save != null)
                    {
                        user = Users.Load(save.Profile.ToJson());

                        if (user == null)
                        {
                            Logger.Error($"Unable to load account with the ID {id}.");
                            return null;
                        }
                    }
                }

                return user;
            }

            return null;
        }

        /// <summary>
        /// Creates a new <see cref="User"/> in the database and caches it.
        /// </summary>
        internal static User Create(string email, string password)
        {
            long id = Interlocked.Increment(ref Users.Seed);

            User user = new User(id)
            {
                Email    = email,
                Password = password,
                Created  = DateTime.Now
            };
            
            Users.Add(user);

            return user;
        }

        /// <summary>
        /// Updates the specified <see cref="User"/> in the cache.
        /// </summary>
        internal static void Update(User user)
        {
            if (!Users.Pool.TryUpdate(user.Identifier, user, Users.Get(user.Identifier)))
            {
                Logger.Error($"Error updating user with ID {user.Identifier}");
            }
        }

        /// <summary>
        /// Saves the specified <see cref="User"/> directly to the <see cref="Mongo"/> database.
        /// </summary>
        internal static async void Save(User user)
        {
            if (!await UserDb.Contains(user.Identifier))
            {
                await UserDb.Create(user);
            }

            await UserDb.Save(user);
        }

        /// <summary>
        /// Deletes the specified <see cref="User"/> directly from the <see cref="Mongo"/> database.
        /// </summary>
        internal static async void Delete(User user)
        {
            await UserDb.Delete(user.Identifier);
        }
        
        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        internal static List<User> FindAll(Predicate<User> predicate)
        {
            return Users.Pool.Values.ToList().FindAll(predicate);
        }

        /// <summary>
        /// Uses the specified JSON to initialize an instance of <see cref="User"/>.
        /// </summary>
        private static User Load(string json)
        {
            User user = new User();
            JsonConvert.PopulateObject(json, user, UserDb.JsonSettings);
            return user;
        }

        /// <summary>
        /// Saves the specified DBMS.
        /// </summary>
        internal static void Save()
        {
            Users.ForEach(user =>
            {
                try
                {
                    Users.Save(user);
                }
                catch (Exception exception)
                {
                    Logger.Error($"{exception.GetType().Name} : did not succeed in saving user [{user}].");
                }
            });

            Logger.Info($"Saved {Users.Count} Users.");
        }

        /// <summary>
        /// Executes an action on every user in the collection.
        /// </summary>
        internal static void ForEach(Action<User> action)
        {
            int count = 0;

            Parallel.ForEach(Users.Pool.Values.Reverse(), user =>
            {
                action.Invoke(user);
                count++;
            });

            // Logger.Debug($"Executed an action on {count} Users.");
        }
    }
}
