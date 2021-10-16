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
    using YaBot.App.Core.State.Impl;
    using YaBot;
    using YaBot.App.Extensions;
    using YaBot.Database;
    using YaBot.Extensions;
    using YaBot.IO;
    using YaBot.IO.Format;
    using File = System.IO.File;

    internal partial class App
    {
        private const string Settings = "config.json";

        public static App Init(Action<string> log)
        {
            var config = JsonConfig.Create(Settings);

            var credentials = config["credentials"].GetString()
                ?._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<Credentials>)
                ?? 
                throw new Exception("Credentials is null");

            var keys = config["keys"].ToString()
                ._(File.ReadAllText)
                ._(JsonConvert.DeserializeObject<Dictionary<string, string[]>>)
                ?.ToDictionary(
                    _ => _.Key,
                    _ => _.Value._(Words.Create))
                ??
                throw new Exception("Config is null");

            var context = new Context(credentials.Database);

            var places = new Crudl<Context, Place>(context, _ => _.Places);

            var error = keys["Error"];

            var formattedText = new FormattedText(new IToken[]
                {
                    new Token(MessageEntityType.Bold, "**"),
                    new Token(MessageEntityType.Italic, "~~"),
                    new LinkToken(MessageEntityType.TextLink, "^^", "|")
                }
                .ToImmutableArray());
            var outputs = new OutputFactory(formattedText.Deserialize);
            var random = new Random(DateTime.Now.Millisecond);

            var nounCache = new RandomCrudlCache<int, Noun>(
                (x, y)=> random.Next(x, y + 1),
                new Crudl<Context,Noun>(
                    context,
                    _ => _.Nouns),
                new RandomCrudlCache<int, Noun>.Args
                {
                    Min = config["nounCacheMinIndex"].GetInt32(),
                    Max = config["nounCacheMaxIndex"].GetInt32(),
                    Count = config["nounCacheCount"].GetInt32(),
                    Limit = config["nounCacheLimit"].GetInt32()
                });

            var startState = new StartState(
                keys["Names"],
                keys["Ping"],
                new IState[]
                {
                    new PlaceCrudlState(
                        new PlaceCrudlState.Keys
                        {
                            Create = new PlaceCrudlState.StateKeys
                            {
                                Keys = keys["PlaceCrudlState_Create_Keys"],
                                Start = keys["PlaceCrudlState_Create_Start"],
                                Success = keys["PlaceCrudlState_Create_Success"]
                            },
                            Read = new PlaceCrudlState.StateKeys
                            {
                                Keys = keys["PlaceCrudlState_Read_Keys"],
                                Start = keys["PlaceCrudlState_Read_Start"]
                            },
                            Delete = new PlaceCrudlState.StateKeys
                            {
                                Keys = keys["PlaceCrudlState_Delete_Keys"],
                                Start = keys["PlaceCrudlState_Delete_Start"],
                                Success = keys["PlaceCrudlState_Delete_Success"]
                            },
                            List =  new PlaceCrudlState.StateKeys
                            {
                                Keys = keys["PlaceCrudlState_List_Keys"],
                                Success = keys["PlaceCrudlState_List_Success"],
                                Next = keys["PlaceCrudlState_List_Next"],
                                Previous = keys["PlaceCrudlState_List_Previous"]
                            },
                            Error = error
                        },
                        places,
                        outputs,
                        new Page(config["pagination"].GetInt32()).Create,
                        new Title(
                            config["titleLength"].GetInt32(),
                            _ => formattedText.Deserialize(_).Item1)
                            .Create),
                    new GetRandomPlaceState(
                        keys["GetRandomPlace_Keys"],
                        keys["GetRandomPlace_Next"],
                        places.All,
                        outputs
                        ),
                    new OrQuestionState(
                        keys["Question_Success"],
                        outputs,
                        () => random.Next(2)),
                    new WhoQuestionState(
                        keys["Question_Success"],
                        keys["WhoQuestion_Answers"],
                        outputs),
                    new QuestionState(
                        keys["Question_Success"],
                        outputs,
                        () => nounCache.Next().Text)
                }
                .ToImmutableArray(),
                outputs.Create);

            States CreateStates()
            {
                var status = new StatusState(
                    keys["Status"],
                    outputs,
                    config["version"].GetString());
                var states = new States(
                    startState,
                    new IState[]
                        {
                            status,
                            new AufState(
                                keys["Auf_Auf_Key"], outputs, keys["Auf_Auf_Success"]),
                            new AufState(keys["Auf_Work_Key"], outputs, keys["Auf_Work_Success"])
                        }
                        .ToImmutableArray(),
                    new IState[]
                        {
                            new AufState(keys["Reset"], outputs, keys["Auf"], false)
                        }
                        .ToImmutableArray(),
                    new IState[]
                        {
                            new AufState(keys["Stop_Key"], outputs, keys["Stop_Success"], false)
                        }
                        .ToImmutableArray(),
                    log);

                states.Changed += status.Update;
                status.Update(startState.Name);

                return states;
            }

            var bot = new Bot(createReceiver: 
                () => CreateStates().Process,
                begin: DateTime.UtcNow,
                log);

            var inputFactory = new InputFactory(formattedText.Serialize, RemoteFile.Load);

            var handler = new Handler(
                inputFactory.CreateAsync,
                bot.Receive,
                _ => new JsonUpdate(_).ToString(),
                _ => _._(error.ToError)._(outputs.Create),
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