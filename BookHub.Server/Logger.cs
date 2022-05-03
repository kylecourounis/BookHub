namespace BookHub.Server
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using BookHub.Server.Helpers;

    internal static class Logger
    {
        /// <summary>
        /// Logs the specified informative message.
        /// </summary>
        internal static void Info(object message, MethodBase method = null)
        {
            if (method == null)
            {
                method = new StackFrame(1).GetMethod();
            }

            Logger.Log(message, method, LogType.Info);
        }

        /// <summary>
        /// Logs the specified debug message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Debug(object message, MethodBase method = null)
        {
            if (method == null)
            {
                method = new StackFrame(1).GetMethod();
            }

            Logger.Log(message, method, LogType.Debug);
        }

        /// <summary>
        /// Logs the specified warning message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Warning(object message, MethodBase method = null)
        {
            if (method == null)
            {
                method = new StackFrame(1).GetMethod();
            }

            Logger.Log(message, method, LogType.Warning);
        }

        /// <summary>
        /// Logs the specified error message.
        /// </summary>
        internal static void Error(object message, MethodBase method = null)
        {
            if (method == null)
            {
                method = new StackFrame(1).GetMethod();
            }

            Logger.Log(message, method, LogType.Error);
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        private static void Log(object message, MethodBase method, LogType logType)
        {
            string prefix = string.Empty;

            switch (logType)
            {
                case LogType.Info:
                {
                    prefix = "[ INFO  ]";
                    break;
                }
                default:
                {
                    prefix = $"[{logType.ToString().ToUpper().Pad(7)}]";
                    break;
                }
            }

            if (method == null)
            {
                System.Diagnostics.Debug.WriteLine($"{prefix} {"null::null".Pad()} : {message}");
            }
            else
            {
                string msg = $"{prefix} {$"{method.DeclaringType?.Name}::{method.Name.Replace("get_", string.Empty).Replace("set_", string.Empty)}".Pad()} : {message}";
                System.Diagnostics.Debug.WriteLine(msg);
            }
        }

        /// <summary>
        /// Shows the values of the fields in the specified type.
        /// </summary>
        internal static void ShowValues(object obj)
        {
            foreach (FieldInfo field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(field => field != null))
            {
                Logger.Info(field.Name.Pad() + " : " + (!string.IsNullOrEmpty(field.Name) ? (field.GetValue(obj) != null ? field.GetValue(obj).ToString() : "(null)") : "(null)").Pad(40));
            }
        }

        private enum LogType
        {
            Info,
            Debug,
            Warning,
            Error
        }
    }
}
