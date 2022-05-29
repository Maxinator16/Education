using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace VectorChatBot.Commands
{
    public class RepeatCommand : BaseCommand
    {
        public RepeatCommand(TelegramBotClient client) : base(client)
        {
            Command = "repeat";
            Description = "repeat me";
        }

        public override string Command { get; }
        public override string Description { get; }

        private ChatMemberStatus _allowedMemberStatus = ChatMemberStatus.Member;
        public override ChatMemberStatus AllowedMemberStatus => _allowedMemberStatus;

        public override async void ExecuteAsync(string msg, long chatId, MessageEntity[] messageEntities)
        {
            if (string.IsNullOrEmpty(msg)) return;

            var chat = new Telegram.Bot.Types.ChatId(chatId);

            await client.SendTextMessageAsync(chat, msg);
        }
    }
}
