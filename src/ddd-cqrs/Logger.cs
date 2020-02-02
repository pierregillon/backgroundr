using System;

namespace ddd_cqrs
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now}] - {message}");
        }
    }
}