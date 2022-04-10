using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace VectorChatBot.Commands
{
    public class BanGroupCommand : BaseCommand
    {
        public BanGroupCommand()
        {
            Command = "banGroup";
            Description = "BanDesc";
        }

        public override string Command { get; }
        public override string Description { get; }

        public override void ExecuteAsync(TelegramBotClient client, string msg, long chatId)
        {
            throw new NotImplementedException();
        }
    }
}
