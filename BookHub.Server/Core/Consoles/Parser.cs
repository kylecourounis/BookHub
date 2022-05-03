namespace BookHub.Server.Core.Consoles
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;

    using BookHub.Server.Helpers;
    using BookHub.Server.Logic;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Logic.Enums;
    using BookHub.Server.Network;

    internal class Parser
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="Parser"/> has been initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="Parser"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (Parser.Initialized)
            {
                return;
            }

            new Thread(() =>
            {
                while (true)
                {
                    int cursorTop = Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
                    Console.Write($"root@{Constants.LocalIP.ToString().Split(":")[0]} > ");

                    string[] command = Console.ReadLine()?.Split(' ');

                    Console.SetCursorPosition(0, cursorTop - 1);
                    Console.Write(new string(' ', Console.BufferWidth) + Environment.NewLine);
                    Console.SetCursorPosition(0, cursorTop - 2);

                    switch (command?[0].Replace("/", string.Empty))
                    {
                        case "stats":
                        {
                            if (Loader.Initialized)
                            {
                                Console.Write(Environment.NewLine);
                                Console.Write($"#  {DateTime.Now.ToString("d")} ---- STATS ---- {DateTime.Now.ToString("t")} #" + Environment.NewLine);
                                Console.Write("# ----------------------------------- #" + Environment.NewLine);
                                Console.Write($"#  Total Requests   # {Processor.TotalRequests.ToString().Pad(15)} #" + Environment.NewLine);
                                Console.Write($"#  Total Responses  # {Processor.TotalResponses.ToString().Pad(15)} #" + Environment.NewLine);
                                Console.Write("# ----------------------------------- #" + Environment.NewLine);
                                Console.Write(Environment.NewLine);
                            }

                            break;
                        }
                        
                        case "addlistings":
                        {
                            var isbns  = File.ReadAllLines("Resources/ISBNs.txt");
                            var random = new Random();
                            
                            if (command.Length == 3)
                            {
                                if (long.TryParse(command[1], out long id) && int.TryParse(command[2], out int count))
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        try
                                        {
                                            Listing listing = new Listing(isbns[random.Next(isbns.Length)])
                                            {
                                                Seller    = Users.Get(id),
                                                Condition = (Condition)random.Next(0, Enum.GetNames(typeof(Condition)).Length)
                                            };

                                            listing.Price = listing.MaxPrice;

                                            Listings.Create(listing);
                                        }
                                        catch
                                        {
                                            Thread.Sleep(5000);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Users.ForEach(user =>
                                {
                                    MakeListing:
                                    try
                                    {
                                        Listing listing = new Listing(isbns[random.Next(isbns.Length)])
                                        {
                                            Seller    = user,
                                            Condition = (Condition)random.Next(0, Enum.GetNames(typeof(Condition)).Length)
                                        };

                                        listing.Price = listing.MaxPrice;

                                        Logger.Debug($"Added listing for seller with id {listing.Seller.Identifier}");

                                        Listings.Create(listing);
                                    }
                                    catch
                                    {
                                        Thread.Sleep(5000);
                                        goto MakeListing;
                                    }
                                    finally
                                    {
                                        Thread.Sleep(1000);
                                    }
                                });
                            }

                            break;
                        }

                        case "help":
                        {
                            StringBuilder helpText = new StringBuilder();

                            helpText.AppendLine("  > /stats - Displays basic server statistics");
                            helpText.AppendLine("  > /addlistings - Adds one listing with a random ISBN for each user.");
                            helpText.AppendLine("  > /addlistings <seller-id> <count> - Adds the specified number of listings to be sold by the specified user.");
                            helpText.AppendLine("  > /exit, /shutdown, /stop - Shuts down the server.");

                            Console.WriteLine("Available Commands: ");
                            Console.Write(helpText.ToString()); 
                            
                            break;
                        }

                        case "clear":
                        {
                            Console.Clear();
                            break;
                        }

                        case "exit":
                        case "shutdown":
                        case "stop":
                        {
                            Environment.Exit(0);
                            break;
                        }

                        default:
                        {
                            Console.WriteLine();
                            break;
                        }
                    }
                }
            }).Start();

            Parser.Initialized = true;
        }
    }
}