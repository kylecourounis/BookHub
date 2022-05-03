namespace BookHub.Server.Logic.Models
{
    using System;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class PaymentInfo
    {
        internal User User;

        [JsonProperty] internal string Number;
        
        [JsonProperty] internal string Expiration;

        [JsonProperty] internal string CVV;

        /// <summary>
        /// Gets a value indicating whether the <see cref="PaymentInfo"/> has expired.
        /// </summary>
        internal bool IsExpired
        {
            get
            {
                if (this.Expiration != null)
                {
                    string[] expires = this.Expiration.Split("/");
                    return DateTime.Now > new DateTime(int.Parse(expires[1]), int.Parse(expires[0]), 1);
                }

                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentInfo"/> class.
        /// </summary>
        internal PaymentInfo()
        {
            // PaymentInfo.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentInfo"/> class.
        /// </summary>
        internal PaymentInfo(User user)
        {
            this.User = user;
        }
    }
}
