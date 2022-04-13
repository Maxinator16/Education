using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace VectorChatBot.Commands
{
    public class UnbanCommand : BaseCommand
    {
        public UnbanCommand(TelegramBotClient client) : base(client)
        {
            Command = "unban";
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
            await client.UnbanChatMemberAsync(chat, member.User.Id, true);
        }
    }
}
