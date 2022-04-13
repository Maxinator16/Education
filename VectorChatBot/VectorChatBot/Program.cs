
// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Reflection;
using Telegram.Bot;
using VectorChatBot;
using VectorChatBot.Commands;
using VectorChatBot.Registries;

var manager = new BotManager();

manager.Start().Wait();

public sealed class BotManager
{
    private const string token = "2056632803:AAGXDzMm-5fdDdc5BWcx44vMRlyVfqJg2_M";

    public BotManager()
    {
        client = new TelegramBotClient(token);

        var result = client.TestApiAsync().Result;
        if (!result) throw new Exception("Invalid Token");

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

            foreach (var fUpdt in updates)
            {
                offset = fUpdt.Id + 1;

                if (fUpdt.Message == null || fUpdt.Message.Entities == null) continue;

                foreach (var fEnt in fUpdt.Message.Entities)
                {
                    if (fEnt == null) continue;
                    var commandFromText = fUpdt.Message.Text?.Substring(fEnt.Offset, fEnt.Length).ToLower();

                    if (commandFromText == null) continue;

                    if (commands.TryGetValue(commandFromText, out var botCommand) == false) continue;

                    var member = await ChatMemberRegistry.GetMember(fUpdt);
                    if (member == null) continue;

                    if (botCommand.IsCommandAvailableForMember(member) == false) continue;

                    try
                    {
                        botCommand.ExecuteAsync(fUpdt.Message.Text.Substring(fEnt.Offset + fEnt.Length), fUpdt.Message.Chat.Id);
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

