using System;

namespace YaBot.PriorityApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using var app = App.Init();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                throw;
            }
        }
    }
}