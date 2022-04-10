
// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using VectorChatBot;
using VectorChatBot.Commands;

var manager = new BotManager();

manager.Start().Wait();

public class BotManager
{
    private const string token = "2056632803:AAGXDzMm-5fdDdc5BWcx44vMRlyVfqJg2_M";


    public BotManager()
    {
        client = new TelegramBotClient(token);

        var result = client.TestApiAsync().Result;
        if (!result) throw new Exception("Invalid Token");

        Initialize();

    }
    private TelegramBotClient client { get; }

    private readonly Dictionary<string, BaseCommand> commands = new Dictionary<string, BaseCommand>();

    private readonly Dictionary<long, Dictionary<long, ChatMember>> ChatAdministrators = new Dictionary<long, Dictionary<long, ChatMember>>();
    private void Initialize()
    {
        var allCommands = Assembly.GetExecutingAssembly().GetTypes().Where(f => f.IsAbstract == false && f.IsClass && f.HasBaseType(typeof(BaseCommand)));
        if (allCommands.HasItems() == false) throw new ArgumentNullException("No command found");
        var allCommandInstances = allCommands.Select(f => Activator.CreateInstance(f) as BaseCommand);

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

            await updateAdmins(updates);

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

                    //TODO : из message доставть правильный Id участника(Member)
                    //if (ChatAdministrators[fUpdt.Message.Chat.Id].TryGetValue(fUpdt.Message.From.Id, out var member) == false) continue;
                    //if (botCommand.IsCommandAvailableForMember(member) == false) continue;

                    botCommand.ExecuteAsync(client, fUpdt.Message.Text.Substring(fEnt.Offset + fEnt.Length), fUpdt.Message.Chat.Id);
                }
            }
        }
    }

    private async Task updateAdmins(Update[] updates)
    {
        if (updates.HasItems() == false) return;

        foreach (var fUpdt in updates)
        {
            if (ChatAdministrators.ContainsKey(fUpdt.Message.Chat.Id) == false)
            {
                var admins = await client.GetChatAdministratorsAsync(fUpdt.Message.Chat.Id);
                ChatAdministrators.Add(fUpdt.Message.Chat.Id, new Dictionary<long, ChatMember>());
                admins.ForEach(f => ChatAdministrators[fUpdt.Message.Chat.Id].Add(f.User.Id, f));
            }
            else
            {
                var admins = await client.GetChatAdministratorsAsync(fUpdt.Message.Chat.Id);
                ChatAdministrators[fUpdt.Message.Chat.Id] = admins.ToDictionary(f => f.User.Id, f => f);
            }
        }

    }
}

