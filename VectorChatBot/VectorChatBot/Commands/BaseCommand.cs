using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace VectorChatBot.Commands
{
    public abstract class BaseCommand
    {
        public const string CommandKey = "/";
        public virtual bool IsMultiple { get; } = false;
        public virtual Telegram.Bot.Types.Enums.ChatMemberStatus AllowedMemberStatus { get; } = Telegram.Bot.Types.Enums.ChatMemberStatus.Administrator;
        public abstract string Command { get; }
        public abstract string Description { get; }
        public abstract void ExecuteAsync(TelegramBotClient client, string msg, long chatId);

        public bool IsCommandAvailableForMember(ChatMember member)
        {
            if (member.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Creator) return true;
            return member.Status == AllowedMemberStatus;
        }
        

        public static implicit operator Telegram.Bot.Types.BotCommand(BaseCommand botCommand) =>
            new Telegram.Bot.Types.BotCommand() { Command = botCommand.Command, Description = botCommand.Description };
    }
}
