namespace BookHub.Server.Logic.Models
{
    using Newtonsoft.Json;

    using BookHub.Server.Database.Models;

    [JsonObject(MemberSerialization.OptIn)]
    internal class College
    {
        internal User User;

        [JsonProperty] internal long Identifier;

        [JsonProperty] internal string Name;

        [JsonProperty] internal string DropLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="College"/> class.
        /// </summary>
        internal College()
        {
            // College.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="College"/> class.
        /// </summary>
        internal College(long id)
        {
            this.Identifier = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="College"/> class.
        /// </summary>
        internal College(User user)
        {
            this.User = user;
        }
        
        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, CollegeDb.JsonSettings);
        }
    }
}
