namespace BookHub.Server.Helpers.Stream
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    internal static class Reader
    {
        /// <summary>
        /// Gets the int.
        /// </summary>
        internal static int GetInt(this JObject json, string key)
        {
            bool parsed = int.TryParse(json.GetString(key), out int value);
            return parsed ? value : -1;
        }

        /// <summary>
        /// Gets the double.
        /// </summary>
        internal static double GetDouble(this JObject json, string key)
        {
            bool parsed = double.TryParse(json.GetString(key), out double value);
            return parsed ? value : -1;
        }

        /// <summary>
        /// Gets the long.
        /// </summary>
        internal static long GetLong(this JObject json, string key)
        {
            bool parsed = long.TryParse(json.GetString(key), out long value);
            return parsed ? value : -1;
        }
        
        /// <summary>
        /// Gets the boolean.
        /// </summary>
        internal static bool GetBoolean(this JObject json, string key)
        {
            bool parsed = bool.TryParse(json.GetString(key), out bool value);
            return parsed && value;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        internal static string GetString(this JObject json, string key)
        {
            return json["data"][key].ToString();
        }

        /// <summary>
        /// Gets the array.
        /// </summary>
        internal static string[] GetArray(this JObject json, string key)
        {
            return JArray.Parse(json.GetString(key)).ToObject<IEnumerable<string>>().ToArray();
        }
    }
}
