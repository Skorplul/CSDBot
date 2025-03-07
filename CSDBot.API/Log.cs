using Discord;

namespace CSDBot.API
{
    public static class Log
    {
	/// <summary>
        /// Sends an Info level messages to the Bot console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Info(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}] [INFO]  {message}", Console.ForegroundColor = ConsoleColor.Green);
        }

        /// <summary>
        /// Sends a Debug level messages to the Bot console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Debug(string message)
        {
            if (Config.Instance.IsDebug)
                Console.WriteLine($"[{DateTime.Now.ToString()}] [DEBUG]  {message}", Console.ForegroundColor = ConsoleColor.Cyan);
        }

        /// <summary>
        /// Sends a Warn level messages to the Bot console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Warn(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}] [WARN]  {message}", Console.ForegroundColor = ConsoleColor.Yellow);
        }

        /// <summary>
        /// Sends an Error level messages to the Bot console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Error(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}] [ERROR]  {message}", Console.ForegroundColor = ConsoleColor.Red);
        }

        /// <summary>
        /// Throws a Fatal error level.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="err">The error description.</param>
        public static void Fatal(string message, string err)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}] [FATAL]  {message}", Console.ForegroundColor = ConsoleColor.Red);
            throw new Exception(err);
        }

        /// <summary>
        /// Task for Startup. Don't use anywere else!
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Task Loging(LogMessage msg)
        {
            Info(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
