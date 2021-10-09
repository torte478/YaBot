namespace YaBot.App
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Core;
    using Core.Database;
    using Core.State;
    using Newtonsoft.Json;
    using Telegram.Bot;
    using Telegram.Bot.Types.Enums;
    using TelegramApi;
    using YaBot.Core;
    using YaBot.Core.Database;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;
    using YaBot.Core.IO.Format;
    using File = System.IO.File;

    internal partial class App
    {
        // TODO : to config
        private const string Version = "1.0.0";
        private const int Pagination = 20;
        
        private const string ConfigPath = "config.json";
        private const string CredentialsPath = "credentials.json";

        public static App Init(Action<string> log)
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
            
            var places = new Crudl<Context, Place>(context, _ => _.Places);

            var error = config["Error"];

            var formattedText = new FormattedText(new Dictionary<MessageEntityType, IToken>
                {
                    // TODO : concat key and impl
                    { MessageEntityType.Bold, new BoldToken("**") },
                    { MessageEntityType.Italic, new ItalicToken("~~") },
                    { MessageEntityType.TextLink, new TextLinkToken("^^", "|") }
                }
                .ToImmutableDictionary());
            var outputs = new OutputFactory(formattedText.Deserialize);
            var random = new Random(DateTime.Now.Millisecond);

            var nounCache = new RandomCrudlCache<int, Noun>(
                (x, y)=> random.Next(x, y + 1),
                new Crudl<Context,Noun>(
                    context,
                    _ => _.Nouns),
                new RandomCrudlCache<int, Noun>.Args
                {
                    // TODO : to config
                    Min = 1,
                    Max = 2,
                    Count = 2,
                    Limit = 1000
                });

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
                                Success = config["PlaceCrudlState_List_Success"],
                                Next = config["PlaceCrudlState_List_Next"],
                                Previous = config["PlaceCrudlState_List_Previous"]
                            },
                            Error = error
                        },
                        places,
                        outputs,
                        new Page(Pagination).Create,
                        new Title(
                            50, //TODO : to config
                            _ => formattedText.Deserialize(_).Item1)
                            .Create),
                    new GetRandomPlaceState(
                        config["GetRandomPlace_Keys"],
                        config["GetRandomPlace_Next"],
                        places.Enumerate,
                        outputs
                        ),
                    new OrQuestionState(
                        config["Question_Success"],
                        outputs,
                        () => random.Next(2)),
                    new WhoQuestionState(
                        config["Question_Success"],
                        config["WhoQuestion_Answers"],
                        outputs),
                    new QuestionState(
                        config["Question_Success"],
                        outputs,
                        () => nounCache.Next().Text)
                }
                .ToImmutableArray(),
                outputs.Create);

            States CreateStates() => new States(
                Version,
                startState, 
                new IState[]
                {
                    new AufState(
                        config["Auf_Auf_Key"],
                        config["Auf_Auf_Success"],
                        outputs),
                    new AufState(
                        config["Auf_Work_Key"],
                        config["Auf_Work_Success"],
                        outputs)
                }
                    .ToImmutableArray(),
                config["Reset"],
                config["Auf"],
                config["Status"],
                outputs,
                log);

            var bot = new Bot(createReceiver: 
                () => CreateStates().Process,
                begin: DateTime.UtcNow,
                log);

            var handler = new Handler(
                new InputFactory(formattedText.Serialize).CreateAsync,
                bot.Receive,
                log);

            return new App(
                new TelegramBotClient(credentials.Telegram),
                new CancellationTokenSource(),
                (client, cancellation) => client.ReceiveAsync(handler, cancellationToken: cancellation),
                context,
                log);
        }
    }
}