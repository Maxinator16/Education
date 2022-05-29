using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace VectorChatBot.Commands
{
    public class UnbanCommand : BaseCommand
    {
        public UnbanCommand(TelegramBotClient client) : base(client)
        {
            Command = "unban";
            Description = "Unban user";
        }
       
        public override string Command { get; }
        public override string Description { get; }
        private ChatMemberStatus _allowedMemberStatus = ChatMemberStatus.Administrator;
        public override ChatMemberStatus AllowedMemberStatus => _allowedMemberStatus;
        public override async void ExecuteAsync(string msg, long chatId, MessageEntity[] messageEntities)
        {
            if (string.IsNullOrEmpty(msg)) return;

            var member = VectorChatBot.Registries.ChatMemberRegistry.TryGetMemberByName(chatId, msg);
            if (member == null && messageEntities.Length == 1) return;
            var chat = new Telegram.Bot.Types.ChatId(chatId);

            if (member != null)
            {
                await client.UnbanChatMemberAsync(chat, member.User.Id, true);                
            }

            for (int i = 1; i < messageEntities.Length; i++)
            {
                if (messageEntities[i].Type == MessageEntityType.TextMention && messageEntities[i].User != null)
                {
                    try
                    {
                        await client.UnbanChatMemberAsync(chat, messageEntities[i].User.Id, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }
    }
}
