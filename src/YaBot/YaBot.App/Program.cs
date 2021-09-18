namespace YaBot.App
{
    using System;
    using System.IO;

    class Program
    {
        static void Main()
        {
            try
            {
                using var app = App.Init(Log);
                app.Run();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                Console.ReadLine();
                throw;
            }
        }

        private static void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            var formatted = $"{DateTime.Now.ToLongTimeString()} => {message}{Environment.NewLine}";
            Console.Write(formatted);
            File.AppendAllText("log.txt", formatted);
        }
    }
}