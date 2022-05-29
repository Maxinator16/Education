
// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Reflection;
using Telegram.Bot;
using VectorChatBot;
using VectorChatBot.Commands;
using VectorChatBot.Registries;
var token = System.Environment.GetEnvironmentVariable("TelegramBotToken");

var manager = new BotManager(token);
manager.Start().Wait();

public sealed class BotManager
{
    public BotManager(string token)
    {
        if(string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
        client = new TelegramBotClient(token);

        var result = client.TestApiAsync().Result;
        if (!result) throw new Exception($"Token:{token} is invalid");

        Initialize();
        ChatMemberRegistry.Initialize(client);
    }

    private TelegramBotClient client { get; }

    private readonly Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();

    private void Initialize()
    {
        var allCommands = Assembly.GetExecutingAssembly().GetTypes().Where(f => f.IsAbstract == false && f.IsClass && f.HasBaseType(typeof(BaseCommand)));
        if (allCommands.HasItems() == false) throw new ArgumentNullException("No command found");
        var allCommandInstances = allCommands.Select(f => Activator.CreateInstance(f, client) as BaseCommand);

        allCommandInstances.ForEach(f => commands.Add(BaseCommand.CommandKey + f.Command, f));

        client.SetMyCommandsAsync(allCommandInstances.Select(f => (Telegram.Bot.Types.BotCommand)f));
    }

    public async Task Start()
    {
        var offset = 0;
        while (true)
        {
            Thread.Sleep(5000);
            var updates = await client.GetUpdatesAsync(offset);

            if (updates.Length == 0) continue;
            foreach (var fUpdt in updates)
            {
                offset = fUpdt.Id + 1;
                var member = await ChatMemberRegistry.GetMember(fUpdt);

                if (fUpdt.Message == null || fUpdt.Message.Entities == null) continue;

                foreach (var fEnt in fUpdt.Message.Entities)
                {
                    if (fEnt == null) continue;

                    var commandFromText = fUpdt.Message.Text?.Substring(fEnt.Offset, fEnt.Length).ToLower();

                    if (commandFromText == null) continue;
                    if (commands.TryGetValue(commandFromText, out var botCommand) == false) continue;

                    if (member == null) continue;

                    if (botCommand.IsCommandAvailableForMember(member) == false) continue;

                    try
                    {
                        botCommand.ExecuteAsync(fUpdt.Message.Text.Substring(fEnt.Offset + fEnt.Length), fUpdt.Message.Chat.Id, fUpdt.Message.Entities);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                    }
                }
            }
        }
    }


}

