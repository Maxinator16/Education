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
        private ChatMemberStatus _allowedMemberStatus = ChatMemberStatus.Administrator;
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

        public override async void ExecuteAsync(string msg, long chatId, MessageEntity[] messageEntities)
        {
            if (string.IsNullOrEmpty(msg)) return;

            var timeIndex = msg.LastIndexOf(timeKey);
            if (timeIndex == -1) return;

            var strMember = msg.Substring(0, timeIndex);
            var strTime = msg.Substring(timeIndex + timeKey.Length, msg.Length - timeIndex - timeKey.Length);

            if (int.TryParse(strTime, out var intTime) == false) return;

            var dateUntil = DateTime.UtcNow.AddMinutes(intTime);

            var member = VectorChatBot.Registries.ChatMemberRegistry.TryGetMemberByName(chatId, strMember);
            if (member == null && messageEntities.Length == 1) return;

            var chat = new Telegram.Bot.Types.ChatId(chatId);

            if (member != null)
            {
                await client.RestrictChatMemberAsync(chat, member.User.Id, restrictSetting, dateUntil);
            }

            for (int i = 1; i < messageEntities.Length; i++)
            {
                if (messageEntities[i].Type == MessageEntityType.TextMention && messageEntities[i].User != null)
                {
                    try
                    {
                        await client.RestrictChatMemberAsync(chat, messageEntities[i].User.Id, restrictSetting, dateUntil);
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }
                }
            }

            
        }
    }
}
