using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace VectorChatBot.Registries
{
    internal static class ChatMemberRegistry
    {
        public static void Initialize(TelegramBotClient clientInstance)
        {
            client = clientInstance;
        }

        private static TelegramBotClient client { get; set; }

        private static readonly ConcurrentDictionary<long, ConcurrentDictionary<long, ChatMember>> ChatAdministrators = new ConcurrentDictionary<long, ConcurrentDictionary<long, ChatMember>>();
        private static readonly ConcurrentDictionary<long, ConcurrentDictionary<long, ChatMember>> Members = new ConcurrentDictionary<long, ConcurrentDictionary<long, ChatMember>>();

        //TODO: В новом треде создать процесс, который следит по таймеру, какой чат когда был обновлён и при необходимости запускал обновление.
        public static async void updateAdmins(Update update)
        {
            if (update.Message == null) return;
            if (ChatAdministrators.ContainsKey(update.Message.Chat.Id) == false)
            {
                var admins = await client.GetChatAdministratorsAsync(update.Message.Chat.Id);
                ChatAdministrators.TryAdd(update.Message.Chat.Id, new ConcurrentDictionary<long, ChatMember>());
                admins.ForEach(f => ChatAdministrators[update.Message.Chat.Id].TryAdd(f.User.Id, f));
            }
            else
            {
                var admins = await client.GetChatAdministratorsAsync(update.Message.Chat.Id);
                ChatAdministrators[update.Message.Chat.Id] = new ConcurrentDictionary<long, ChatMember>(admins.ToDictionary(f => f.User.Id, f => f));
            }
        }

        //TODO: Обновлять как? При изменении статуса Мебера необходимо менять флаг UpdateRequired = true. Как обновлять, если статус мембера был обновлён вручную? так же необходим фоновый процесс.
        public static async Task<ChatMember?> GetMember(Update update)
        {
            if (update.Message == null || update.Message.From == null) return null;
            if (Members.ContainsKey(update.Message.Chat.Id) && Members[update.Message.Chat.Id].ContainsKey(update.Message.From.Id))
                return Members[update.Message.Chat.Id][update.Message.From.Id];

            var member = await client.GetChatMemberAsync(update.Message.Chat.Id, update.Message.From.Id);

            if (member == null) return null;
            if (Members.ContainsKey(update.Message.Chat.Id) == false)
            {
                if (Members.TryAdd(update.Message.Chat.Id, new ConcurrentDictionary<long, ChatMember>()))
                    Members[update.Message.Chat.Id].TryAdd(update.Message.From.Id, member);
            }
            else
                Members[update.Message.Chat.Id].TryAdd(update.Message.From.Id, member);

            return member;
        }

        private static string getNormalizeUserName(string firstName, string lastName) => string.Create(System.Globalization.CultureInfo.InvariantCulture,$"{firstName.ToLower()}{lastName.ToLower()}");
        
        public static ChatMember? GetMemberByName(long chatId, string memberName)
        {
            if (string.IsNullOrEmpty(memberName)) throw new ArgumentNullException(nameof(memberName));
            memberName = memberName.RemoveWhitespace().ToLower();
            
            if (Members.TryGetValue(chatId, out var members) == false) return null;
            return members.Values.FirstOrDefault(f=> getNormalizeUserName(f.User.FirstName, f.User.LastName) == memberName);
        }
    }
}
