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
            var resultString = Regex.Replace(message.Text.ToLower(), @"^[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$", "'$0'", RegexOptions.IgnoreCase);
            Guid id;
            if (!Guid.TryParse(resultString, out id))
                return message.CreateReplyMessage($"can't convert {resultString}");
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
            foreach (var item in bridges)
            {
                var msg = CommandTool.Instance.Request.CreateMessage(item.UserId, item.ChannelId, $"{account.Name} connected this bridge.");
                await CommandTool.Instance.Client.Messages.SendMessageAsync(msg);
            }
        }
    }
}
