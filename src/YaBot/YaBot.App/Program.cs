namespace YaBot.App
{
    using System;
    using System.IO;
    using Telegram.Bot;

    class Program
    {
        static void Main(string[] args)
        {
            var token = File.ReadAllText("token");
            var botClient = new TelegramBotClient(token);
            
            var me = botClient.GetMeAsync().Result;
            
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
        }
    }
}