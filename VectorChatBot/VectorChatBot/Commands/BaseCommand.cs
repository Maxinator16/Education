using Telegram.Bot;
using Telegram.Bot.Types;

namespace VectorChatBot.Commands
{
    public abstract class BaseCommand
    {
        public BaseCommand(TelegramBotClient client)
        {
            this.client = client;
        }

        protected readonly TelegramBotClient client;

        public const string CommandKey = "/";
        public virtual bool IsMultiple { get; } = false;
        public virtual Telegram.Bot.Types.Enums.ChatMemberStatus AllowedMemberStatus { get; } = Telegram.Bot.Types.Enums.ChatMemberStatus.Administrator;
        public abstract string Command { get; }
        public abstract string Description { get; }
        public abstract void ExecuteAsync(string msg, long chatId);

        public bool IsCommandAvailableForMember(ChatMember member)
        {
            if (member.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Creator) return true;
            return member.Status == AllowedMemberStatus;
        }

        public static implicit operator Telegram.Bot.Types.BotCommand(BaseCommand botCommand) =>
            new Telegram.Bot.Types.BotCommand() { Command = botCommand.Command, Description = botCommand.Description };
    }
}
