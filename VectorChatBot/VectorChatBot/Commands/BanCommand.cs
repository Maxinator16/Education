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
    public class BanCommand : BaseCommand
    {
        public BanCommand(TelegramBotClient client) : base(client)
        {
            Command = "ban";
            Description = "Ban user";
        }
       
        public override string Command { get; }
        public override string Description { get; }
        private ChatMemberStatus _allowedMemberStatus = ChatMemberStatus.Administrator;
        public override ChatMemberStatus AllowedMemberStatus => _allowedMemberStatus;

        public override async void ExecuteAsync(string msg, long chatId)
        {
            if (string.IsNullOrEmpty(msg)) return;

            var member = VectorChatBot.Registries.ChatMemberRegistry.GetMemberByName(chatId, msg);
            if (member == null) return;

            var chat = new Telegram.Bot.Types.ChatId(chatId);
            await client.BanChatMemberAsync(chat, member.User.Id);
        }
    }
}
