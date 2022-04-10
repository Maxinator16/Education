
// See https://aka.ms/new-console-template for more information

using Telegram.Bot;
using Telegram.Bot.Types;

//Max_Channel_C/MaxChannel(name)
//bot name (Max_bot) username(@Max_test2_bot)


/*const string apiToken = "2056632803:AAGXDzMm-5fdDdc5BWcx44vMRlyVfqJg2_M";

var client = new TelegramBotClient(apiToken);
//await client.SetMyCommandsAsync(new BotCommand[] { new BotCommand() { Command = "hello", Description = "HelloCommand" } })

var result = await client.TestApiAsync();
if (!result) throw new Exception("Invalid Token");

//var chatId = new Telegram.Bot.Types.ChatId("@MaxChannel");

//var GroupChatId = new Telegram.Bot.Types.ChatId(-1001769738312);
//var ChannelId = new Telegram.Bot.Types.ChatId(-1001736387256);
//var chat = await client.GetChatAsync(GroupChatId);

var offset = 0;

while (true)
{
    //var sendMsg = await client.SendTextMessageAsync(ChannelId, "Im Bot");
    Thread.Sleep(10000);
    var messages = await client.GetUpdatesAsync(offset);

    foreach (var msg in messages)
    {
        offset = msg.Id + 1;
    }
} */

var manager = new BotManager();

manager.Start();

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

    private readonly Dictionary<string, BotCommand> commands = new Dictionary<string, BotCommand>();

    private void Initialize()
    {
        var repeatCommand = new RepeatCommand();
        commands.Add(BotCommand.CommandKey + repeatCommand.Command, repeatCommand);

        client.SetMyCommandsAsync(new Telegram.Bot.Types.BotCommand[] {(Telegram.Bot.Types.BotCommand)repeatCommand });
    }

    public void Start()
    {
        
        var offset = 0;
        while (true)
        {
            Thread.Sleep(5000);
            var messages = client.GetUpdatesAsync(offset).Result;

            foreach (var fMsg in messages)
            {
                offset = fMsg.Id + 1;

                if (fMsg.Message == null || fMsg.Message.Entities == null) continue;
                
                foreach(var fEnt in fMsg.Message.Entities)
                {
                    if(fEnt == null) continue;
                    var commandFromText = fMsg.Message.Text?.Substring(fEnt.Offset, fEnt.Length).ToLower();

                    if(commandFromText == null) continue;

                    if (commands.TryGetValue(commandFromText, out var botCommand) == false) continue;

                    botCommand.ExecuteAsync(client, fMsg.Message.Text.Substring(fEnt.Offset + fEnt.Length), fMsg.Message.Chat.Id);
                }
                
                   
            }
        }
    }

    

}

public abstract class BotCommand

{

    public const string CommandKey = "/";
    public abstract string Command { get; }
    public abstract string Description { get; }
    public abstract void ExecuteAsync(TelegramBotClient client, string msg, long chatId);

    public static implicit operator Telegram.Bot.Types.BotCommand(BotCommand botCommand) => 
        new Telegram.Bot.Types.BotCommand() {Command = botCommand.Command, Description = botCommand.Description};

}

public class RepeatCommand : BotCommand
{
    public RepeatCommand()
    {
        Command = "repeat";
        Description = "repeat me";
    }
    public override string Command { get; }
    public override string Description { get; }

    public override async void ExecuteAsync(TelegramBotClient client, string msg, long chatId)
    {
        if (string.IsNullOrEmpty(msg)) return;
        
        var chat = new Telegram.Bot.Types.ChatId(chatId);

        await client.SendTextMessageAsync(chat, msg);



    }

    
}

public class BanGroupCommand : BotCommand
{
    
    public BanGroupCommand()
    {
        Command = "banGroup";
        Description = "BanDesc";
    }
    

   

    public override string Command { get; }
    public override string Description { get; }

    public override void ExecuteAsync(TelegramBotClient client, string msg, long chatId)
    {
        throw new NotImplementedException();
    }
}

