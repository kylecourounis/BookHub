namespace BookHub.Server.Helpers.Json
{
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using BookHub.Server.Logic;

    internal class ShouldSerializeContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Creates a <see cref="JsonProperty" /> for the given <see cref="MemberInfo" />.
        /// </summary>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // Determine whether we are serializing a property of type User
            if (property.DeclaringType == typeof(User))
            {
                switch (property.PropertyName)
                {
                    // If the property name we are serializing in the type User is any one of these, then don't serialize it
                    case "Created":
                    case "Orders":
                    case "PaymentInfo":
                    case "Email":
                    case "Password":
                    {
                        property.ShouldSerialize = instance =>
                        {
                            User user = (User)instance;
                            return user.SerializeAllInfo;
                        };
                        
                        break;
                    }
                }
            }

            return property;
        }
    }
}
