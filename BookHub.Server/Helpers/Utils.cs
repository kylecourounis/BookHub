namespace BookHub.Server.Helpers
{
    using System;
    using System.Linq;

    internal class Utils
    {
        /// <summary>
        /// Generates a random string with the specified length.
        /// </summary>
        public static string GenerateRandomString(int length = 40)
        {
            var random = new Random();
            var chars  = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            
            return new string(Enumerable.Repeat(chars, length).Select(str => str[random.Next(str.Length)]).ToArray());
        }
    }
}
