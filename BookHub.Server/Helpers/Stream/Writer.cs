namespace BookHub.Server.Helpers.Stream
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    
    internal static class Writer
    {
        /// <summary>
        /// Adds the int.
        /// </summary>
        internal static void AddInt(this Dictionary<string, object> data, string key, int value)
        {
            data.Add(key, value);
        }

        /// <summary>
        /// Adds the long.
        /// </summary>
        internal static void AddLong(this Dictionary<string, object> data, string key, long value)
        {
            data.Add(key, value);
        }

        /// <summary>
        /// Adds the boolean.
        /// </summary>
        internal static void AddBoolean(this Dictionary<string, object> data, string key, bool value)
        {
            data.Add(key, value);
        }

        /// <summary>
        /// Adds the string.
        /// </summary>
        internal static void AddString(this Dictionary<string, object> data, string key, string value)
        {
            data.Add(key, value);
        }

        /// <summary>
        /// Adds all data.
        /// </summary>
        internal static void AddAllData(this JsonWriter writer, Dictionary<string, object> data, int id)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("id");
            writer.WriteValue(id);

            writer.WritePropertyName("data");
            writer.WriteStartObject();
            foreach (var (key, value) in data)
            {
                writer.WritePropertyName(key);
                writer.WriteValue(value);
            }
            writer.WriteEndObject();

            writer.WriteEndObject();
        }
    }
}
