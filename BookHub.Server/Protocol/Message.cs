namespace BookHub.Server.Protocol
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    
    using BookHub.Server.Helpers.Stream;
    using BookHub.Server.Logic;
    using BookHub.Server.Protocol.Enums;

    internal abstract class Message
    {
        private int Identifier;
        
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        internal MessageType Type
        {
            get => (MessageType)this.Identifier;
            set => this.Identifier = (int)value;
        }

        /// <summary>
        /// Gets the json.
        /// </summary>
        internal JObject JSON
        {
            get;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        internal Dictionary<string, object> Data
        {
            get;
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        internal User User
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        internal HttpListenerContext Context
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        internal Message(User user, HttpListenerContext context)
        {
            this.User    = user;
            this.Context = context;
            this.Data    = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        internal Message(JObject json)
        {
            this.JSON = json;
        }

        /// <summary>
        /// Encodes this instance.
        /// </summary>
        internal virtual void Encode()
        {
            // Encode.
        }

        /// <summary>
        /// Decodes this instance.
        /// </summary>
        internal virtual void Decode()
        {
            // Decode.
        }

        /// <summary>
        /// Handles this instance.
        /// </summary>
        internal virtual void Handle()
        {
            // Handle.
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            StringWriter writer = new StringWriter(new StringBuilder());

            using (JsonWriter jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.AddAllData(this.Data, this.Identifier);
            }

            return $"{this.Context.Request.QueryString["callback"]}({writer});";
        }
    }
}
