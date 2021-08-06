namespace YaBot.App
{
    using System;

    class Program
    {
        static void Main()
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