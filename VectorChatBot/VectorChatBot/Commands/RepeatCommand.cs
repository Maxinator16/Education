using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace VectorChatBot.Commands
{
    public class RepeatCommand : BaseCommand
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
}
