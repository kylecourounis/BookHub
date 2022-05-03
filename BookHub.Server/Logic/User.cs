namespace BookHub.Server.Logic
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using BookHub.Server.Database.Models;
    using BookHub.Server.Logic.Collections;
    using BookHub.Server.Logic.Models;

    [JsonObject(MemberSerialization.OptIn)]
    internal class User
    {
        [JsonProperty] internal long Identifier;

        [JsonProperty] internal string Name;

        [JsonProperty] internal string Email;

        [JsonProperty] internal string Password;

        [JsonProperty] internal College College;

        [JsonProperty] internal PaymentInfo PaymentInfo;

        [JsonProperty] internal List<Order> Orders;

        [JsonProperty] internal DateTime Created;
        
        internal bool SerializeAllInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        internal User()
        {
            this.College          = new College(this);
            this.PaymentInfo      = new PaymentInfo(this);
            this.Orders           = new List<Order>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        internal User(long id) : this()
        {
            this.Identifier       = id;
            this.SerializeAllInfo = true;
        }

        /// <summary>
        /// Updates this instance in the cache.
        /// </summary>
        internal void Save()
        {
            Users.Update(this);
        }
        
        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, UserDb.JsonSettings);
        }
    }
}
