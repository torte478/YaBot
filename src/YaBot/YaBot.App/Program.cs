namespace YaBot.App
{
    using Telegram.Bot;

    class Program
    {
        static void Main()
        {
            using var bot = App.Init();
            bot.Run();
            //
            // var token = File.ReadAllText("token");
            //
            // var bot = new TelegramBotClient(token);
            //
            // var me = await bot.GetMeAsync();
            // Console.WriteLine(me.Username);
            //
            // var cancelation = new CancellationTokenSource();
            //
            // var task =  bot.ReceiveAsync<UpdateHandler>(cancelation.Token);
            //
            // Console.ReadLine();
            // cancelation.Cancel();
        }
        
        // private class UpdateHandler : IUpdateHandler
        // {
        //     public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        //     {
        //         if (update.Message == null)
        //             return Task.CompletedTask;
        //         
        //         return botClient.SendTextMessageAsync(update.Message.Chat, $"Answer: {update.Message.Text}");
        //     }
        //
        //     public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        //     {
        //         return botClient.SendTextMessageAsync(123, exception.ToString());
        //     }
        //
        //     public UpdateType[]? AllowedUpdates => new[] { UpdateType.Message };
        // }
    }
}