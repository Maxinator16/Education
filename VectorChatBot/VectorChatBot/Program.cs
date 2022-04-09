
// See https://aka.ms/new-console-template for more information

using Telegram.Bot;
using Telegram.Bot.Types;

//Max_Channel_C/MaxChannel(name)
//bot name (Max_bot) username(@Max_test2_bot)


const string apiToken = "2056632803:AAGXDzMm-5fdDdc5BWcx44vMRlyVfqJg2_M";

var client = new TelegramBotClient(apiToken);
//await client.SetMyCommandsAsync(new BotCommand[] { new BotCommand() { Command = "hello", Description = "HelloCommand" } })

var result = await client.TestApiAsync();
if (!result) throw new Exception("Invalid Token");

//var chatId = new Telegram.Bot.Types.ChatId("@MaxChannel");

var GroupChatId = new Telegram.Bot.Types.ChatId(-1001769738312);
var ChannelId = new Telegram.Bot.Types.ChatId(-1001736387256);
var chat = await client.GetChatAsync(GroupChatId);

var offset = 0;

while (true)
{
    var sendMsg = await client.SendTextMessageAsync(ChannelId, "Im Bot");
    Thread.Sleep(10000);
    var messages = await client.GetUpdatesAsync(offset);

    foreach (var msg in messages)
    {
        offset = msg.Id + 1;
    }
}



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

    private void Initialize()
    {

    }
    private TelegramBotClient client { get; }

}

public abstract class BotCommand
{

    public const string CommandKey = "/";
    public abstract string Command { get; }
    public abstract string Description { get; }
    public abstract void ExecuteAsync(TelegramBotClient client, string msg, int chatId);


}

public class RepeatCommand : BotCommand
{
    public RepeatCommand()
    {
        Command = "Repeat";
        Description = "Repeat me";
    }
    public override string Command { get; }
    public override string Description { get; }

    public override async void ExecuteAsync(TelegramBotClient client, string msg, int chatId)
    {
        var chat = new Telegram.Bot.Types.ChatId(chatId);

        await client.SendTextMessageAsync(chat, msg);


    }
}

public class BanGroupCommand : BotCommand
{

    public BanGroupCommand()
    {
        Command = "BanGroup";
        Description = "BanDesc";
    }
    public override string Command { get; }
    public override string Description { get; }

    public override void ExecuteAsync(TelegramBotClient client, string msg, int chatId)
    {
        throw new NotImplementedException();
    }
}

