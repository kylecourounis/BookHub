namespace BookHub.Server.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Newtonsoft.Json.Linq;

    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Protocol.Enums;
    using BookHub.Server.Protocol.Messages.Client;

    internal static class Factory
    {
        private static readonly Dictionary<MessageType, Type> Messages = new Dictionary<MessageType, Type>();

        /// <summary>
        /// Gets a value indicating whether this <see cref="Factory"/> is initialized.
        /// </summary>
        internal static bool Initialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="Factory"/> class.
        /// </summary>
        internal static void Initialize()
        {
            if (Factory.Initialized)
            {
                return;
            }

            Factory.LoadMessages();

            Factory.Initialized = true;
        }

        /// <summary>
        /// Loads the messages.
        /// </summary>
        internal static void LoadMessages()
        {
            Factory.Messages.Add(MessageType.Login, typeof(LoginRequest));
            Factory.Messages.Add(MessageType.AddListing, typeof(AddListingRequest));
            Factory.Messages.Add(MessageType.Search, typeof(SearchRequest));
            Factory.Messages.Add(MessageType.BuyListing, typeof(BuyListingRequest));
            Factory.Messages.Add(MessageType.UpdateAccountInfo, typeof(UpdateAccountInfoRequest));
            Factory.Messages.Add(MessageType.GetColleges, typeof(GetCollegesRequest));
        }

        /// <summary>
        /// Creates a message using the specified type.
        /// </summary>
        internal static void CreateMessage(HttpListenerContext context, out Message message)
        {
            var json = JObject.Parse(context.Request.QueryString["json"]);
            
            var id = json["id"].ToObject<int>();

            var type = (MessageType)id;

            if (Factory.Messages.TryGetValue(type, out var messageType))
            {
                message = (Message)Activator.CreateInstance(messageType, json);

                message.Type    = type;
                message.Context = context;
                
                var userId = json["userId"].ToObject<long>();

                if (userId != -1)
                {
                    message.User = Users.Get(userId);
                }
            }
            else
            {
                Logger.Warning($"Failed to create a message with ID {id}!");
                message = null;
            }
        }
    }
}
