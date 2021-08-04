namespace YaBot.App
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Configs;
    using Core;
    using Core.Database;
    using Core.State;
    using Extensions;
    using Newtonsoft.Json;
    using Telegram.Bot;
    using File = System.IO.File;

    internal partial class App
    {
        private const string ConfigPath = "config.json";
        private const string CredentialsPath = "credentials.json";

        public static App Init()
        {
            var credentials = CredentialsPath
                ._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<Credentials>)
                ?? 
                throw new Exception("Credentials is null");

            var config = ConfigPath
                ._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<Dictionary<string, string[]>>)
                ?.ToDictionary(
                    _ => _.Key,
                    _ => _.Value._(Words.Create))
                ?? 
                throw new Exception("Config is null"); 
            
            var context = new Context(credentials.Database);
            
            var places = new Crudl<Place>(context, _ => _.Places);

            var error = config["Error"];
            
            var startState = new StartState(
                config["Names"],
                config["Ping"],
                new IState[]
                {
                    new PlaceCrudlState(
                        new PlaceCrudlState.Keys
                        {
                            Create = new PlaceCrudlState.StateKeys
                            {   
                                Keys = config["PlaceCrudlState_Create_Keys"],
                                Start = config["PlaceCrudlState_Create_Start"],
                                Success = config["PlaceCrudlState_Create_Success"]
                            },
                            Read = new PlaceCrudlState.StateKeys
                            {
                                Keys = config["PlaceCrudlState_Read_Keys"],
                                Start = config["PlaceCrudlState_Read_Start"]
                            },
                            Delete = new PlaceCrudlState.StateKeys
                            {
                                Keys = config["PlaceCrudlState_Delete_Keys"],
                                Start = config["PlaceCrudlState_Delete_Start"],
                                Success = config["PlaceCrudlState_Delete_Success"] 
                            },
                            List =  new PlaceCrudlState.StateKeys
                            {
                                Keys = config["PlaceCrudlState_List_Keys"],
                                Success = config["PlaceCrudlState_List_Success"] 
                            },
                            Error = error
                        },
                        places
                        ),
                    new GetRandomPlaceState(
                        config["GetRandomPlace"],
                        places.Enumerate)
                }
                .ToImmutableArray());

            var bot = new Bot(createReceiver: 
                () => new States(
                    startState, 
                    config["Reset"],
                config["Auf"],
                    Log)
                    .Process,
                begin: DateTime.UtcNow); 

            var handler = new Handler(bot.ReceiveAsync, Log);

            return new App(
                new TelegramBotClient(credentials.Telegram),
                new CancellationTokenSource(),
                (client, cancellation) => client.ReceiveAsync(handler, cancellation),
                context,
                Log);
        }

        private static void Log(string message)
        {
            #if DEBUG

            if (string.IsNullOrEmpty(message))
                return;
            
            $"{DateTime.Now.ToLongTimeString()} {message}"
                ._(Console.WriteLine);
            
            #endif
        }
    }
}