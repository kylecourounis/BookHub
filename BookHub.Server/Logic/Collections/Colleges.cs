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

    internal static class Colleges
    {
        private static ConcurrentDictionary<long, College> Pool;

        private static long Seed;

        /// <summary>
        /// Gets the count.
        /// </summary>
        internal static int Count
        {
            get
            {
                return Colleges.Pool.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Colleges"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="Colleges"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (Colleges.Initialized)
            {
                return;
            }

            Colleges.Pool = new ConcurrentDictionary<long, College>();

            foreach (CollegeDb dbEntry in Mongo.Colleges.Find(db => true).ToList())
            {
                if (dbEntry != null)
                {
                    College college = new College(dbEntry.Identifier);

                    JsonConvert.PopulateObject(dbEntry.College.ToJson(), college, CollegeDb.JsonSettings);

                    Colleges.Add(college);
                }
            }

            if (false)
            {
                Colleges.Create(new College
                {
                    Name         = "Marist College",
                    DropLocation = "Rotunda"
                });

                Colleges.Create(new College
                {
                    Name         = "Vassar College",
                    DropLocation = "Somewhere"
                });

                Colleges.Create(new College
                {
                    Name         = "SUNY New Paltz",
                    DropLocation = "Somewhere"
                });
            }

            Colleges.Seed = Mongo.CollegeSeed;

            Console.Write($"and {Colleges.Count} colleges." + Environment.NewLine + Environment.NewLine);

            Colleges.Initialized = true;
        }

        /// <summary>
        /// Adds the specified <see cref="College"/> to the cache.
        /// </summary>
        internal static void Add(College college)
        {
            if (Colleges.Pool.ContainsKey(college.Identifier))
            {
                if (!Colleges.Pool.TryUpdate(college.Identifier, college, college))
                {
                    Logger.Error("Unsuccessfully updated the specified college to the dictionary.");
                }
            }
            else
            {
                if (!Colleges.Pool.TryAdd(college.Identifier, college))
                {
                    Logger.Error("Unsuccessfully added the specified college to the dictionary.");
                }
            }
        }

        /// <summary>
        /// Removes the specified <see cref="College"/> from the cache.
        /// </summary>
        internal static void Remove(College college)
        {
            if (Colleges.Pool.ContainsKey(college.Identifier))
            {
                if (!Colleges.Pool.TryRemove(college.Identifier, out College tmpCollege))
                {
                    Logger.Error("Unsuccessfully removed the specified college from the dictionary.");
                }
                else
                {
                    if (!tmpCollege.Equals(college))
                    {
                        Logger.Error("Successfully removed a college from the list but the returned college was not equal to the college.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="College"/> using the specified id.
        /// </summary>
        internal static College Get(long id)
        {
            if (!Colleges.Pool.TryGetValue(id, out College college))
            {
                CollegeDb save = CollegeDb.Load(id).GetAwaiter().GetResult();

                if (save != null)
                {
                    college = Colleges.Load(save.College.ToJson());

                    if (college == null)
                    {
                        Logger.Error($"Unable to load college with the id {id}.");
                        return null;
                    }
                }
            }

            return college;
        }

        /// <summary>
        /// Creates a new <see cref="College"/> in the database and caches it.
        /// </summary>
        internal static void Create(College college)
        {
            college.Identifier = Interlocked.Increment(ref Colleges.Seed);
            
            CollegeDb.Create(college).GetAwaiter().GetResult();
            Colleges.Add(college);
        }

        /// <summary>
        /// Updates the specified <see cref="College"/> in the cache.
        /// </summary>
        internal static void Update(College college)
        {
            if (!Colleges.Pool.TryUpdate(college.Identifier, college, Colleges.Get(college.Identifier)))
            {
                Logger.Error($"Error updating college with id {college.Identifier}");
            }
        }

        /// <summary>
        /// Saves the specified <see cref="College"/> directly to the <see cref="Mongo"/> database.
        /// </summary>
        internal static async void Save(College college)
        {
            await CollegeDb.Save(college);
        }

        /// <summary>
        /// Deletes the specified <see cref="College"/> directly from the <see cref="Mongo"/> database.
        /// </summary>
        internal static async void Delete(College college)
        {
            Colleges.Remove(college);

            await CollegeDb.Delete(college.Identifier);
        }
        
        /// <summary>
        /// Retrieves all the unsold listings that match the conditions defined by the specified predicate.
        /// </summary>
        internal static List<College> FindAll(Predicate<College> predicate)
        {
            return Colleges.Pool.Values.ToList().ToList().FindAll(predicate);
        }
        
        /// <summary>
        /// Uses the specified JSON to initialize an instance of <see cref="College"/>.
        /// </summary>
        private static College Load(string json)
        {
            College college = new College();
            JsonConvert.PopulateObject(json, college, CollegeDb.JsonSettings);
            return college;
        }

        /// <summary>
        /// Saves this collection to the <see cref="Mongo"/> database.
        /// </summary>
        internal static void Save()
        {
            Colleges.ForEach(college =>
            {
                try
                {
                    Colleges.Save(college);
                }
                catch (Exception exception)
                {
                    Logger.Error($"{exception.GetType().Name} : did not succeed in saving college [{college}].");
                }
            });

            Logger.Info($"Saved {Colleges.Count} Colleges.");
        }

        /// <summary>
        /// Executes an action on every college in the collection.
        /// </summary>
        internal static void ForEach(Action<College> action)
        {
            int count = 0;

            foreach (College college in Colleges.Pool.Values)
            {
                action.Invoke(college);
                count++;
            }

            // Logger.Debug($"Executed an action on {count} Books.");
        }
    }
}
