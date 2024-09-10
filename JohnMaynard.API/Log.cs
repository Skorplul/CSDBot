using System;
using System.Collections.Generic;
using System.Reflection;

namespace JohnMaynard.API
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
    }
}
