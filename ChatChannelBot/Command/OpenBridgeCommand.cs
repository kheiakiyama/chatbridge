using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using ChatBridgeModel;
using System.Text.RegularExpressions;

namespace ChatChannelBot.Command
{
    public class OpenBridgeCommand : ICommand
    {
        public bool DoHandle(Message message)
        {
            return message.Text.ToLower().Contains("open bridge");
        }

        public async Task<Message> Reply(Message message)
        {
            var idText = Regex.Match(message.Text.ToLower(), @"(\{){0,1}[0-9a-f]{8}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{4}\-[0-9a-f]{12}(\}){0,1}").Value;
            Guid id;
            if (!Guid.TryParse(idText, out id))
                return message.CreateReplyMessage($"can't convert {idText}");
            var account = await CommandTool.Instance.Repository.OpenBridge(id, message.From);
            if (account != null)
            {
                await SendMessageConnected(account);
                var msg = message.CreateReplyMessage($"open your bridge.");
                msg.SetBotUserData(CommandTool.PropertyIdName, account.RowKey);
                return msg;
            }
            else
                return message.CreateReplyMessage($"bridge can't open. Please check id you sent.");
        }

        private async Task SendMessageConnected(ChatAccount account)
        {
            var bridges = await CommandTool.Instance.Repository.FindBridges(account.OwnerId);
            foreach (var item in bridges.Where(q => q.UserId != account.UserId))
            {
                var msg = CommandTool.Instance.Request.CreateMessage(item.Address, item.ChannelId, $"{account.Name} connected this bridge.");
                await CommandTool.Instance.Client.Messages.SendMessageAsync(msg);
            }
        }
    }
}
