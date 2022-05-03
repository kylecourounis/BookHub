namespace BookHub.Server.Helpers
{
    using System.Runtime.InteropServices;

    internal static class Natives
    {
        /// <summary>
        /// Gets a value indicating whether or not the program is running with administrator privileges.
        /// </summary>
        internal static bool IsElevated
        {
            get
            {
                return Natives.IsUserAnAdmin();
            }
        }

        [DllImport("shell32.dll")] public static extern bool IsUserAnAdmin();
    }
}
