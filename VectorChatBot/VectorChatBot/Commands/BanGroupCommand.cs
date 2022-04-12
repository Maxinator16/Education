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
        public BanGroupCommand(TelegramBotClient client) : base(client)
        {
            Command = "banGroup";
            Description = "BanDesc";
        }

        public override string Command { get; }
        public override string Description { get; }

        public override void ExecuteAsync(string msg, long chatId)
        {
            throw new NotImplementedException();
        }
    }
}
