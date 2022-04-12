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
    public class RestrictMemberCommand : BaseCommand
    {
        public RestrictMemberCommand(TelegramBotClient client) : base(client)
        {
            Command = "restrict";
            Description = "Restrict";
        }

        private const string timeKey = "-t";
        public override string Command { get; }
        public override string Description { get; }
        private ChatMemberStatus _allowedMemberStatus = ChatMemberStatus.Member;
        public override ChatMemberStatus AllowedMemberStatus => _allowedMemberStatus;

        private ChatPermissions restrictSetting = new ChatPermissions()
        {
            CanSendMessages = false,
            CanSendMediaMessages = false,
            CanSendPolls = false,
            CanSendOtherMessages = false,
            CanAddWebPagePreviews = false,
            CanChangeInfo = false,
            CanInviteUsers = false,
            CanPinMessages = false,
        };

        public override async void ExecuteAsync(string msg, long chatId)
        {
            if (string.IsNullOrEmpty(msg)) return;

            var timeIndex = msg.LastIndexOf(timeKey);
            if (timeIndex == -1) return;
            
            var strMember = msg.Substring(0, timeIndex);
            var strTime = msg.Substring(timeIndex + timeKey.Length, msg.Length - timeIndex - timeKey.Length);

            if (int.TryParse(strTime, out var intTime) == false) return;

            var dateUntil = DateTime.UtcNow.AddSeconds(intTime);

            var member = VectorChatBot.Registries.ChatMemberRegistry.GetMemberByName(chatId, strMember);
            if (member == null) return;
          
            var chat = new Telegram.Bot.Types.ChatId(chatId);
            await client.RestrictChatMemberAsync(chat, member.User.Id, restrictSetting, dateUntil);
        }
    }
}
