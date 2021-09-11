using System;

namespace YaBot.PriorityApp
{
    using System.IO;

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
                File.WriteAllText("last_exception.txt" , ex.ToString());
                Console.WriteLine(ex);
                Console.ReadLine();
                throw;
            }
        }
    }
}