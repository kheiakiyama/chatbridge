using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using ChatBridgeModel;

namespace ChatBridgeBot.Command
{
    public class MessagePipeCommand : ICommand
    {
        public bool DoHandle(Message message)
        {
            var account = CommandTool.Instance.Request.GetAccountData(message);
            return account != null;
        }

        public async Task<Message> Reply(Message message)
        {
            var account = CommandTool.Instance.Request.GetAccountData(message);
            if (account == null)
                return message.CreateReplyMessage($"can't bridge. id:[{account.RowKey}]");

            var bridges = await CommandTool.Instance.Repository.FindBridges(account.OwnerId);
            foreach (var item in bridges.Where(q => q.UserId != message.From.Id))
            {
                var msg = CommandTool.Instance.Request.CreateMessage(item.Address, item.ChannelId, $"{message.From.Name}: {message.Text}");
                await CommandTool.Instance.Client.Messages.SendMessageAsync(msg);
            }
            return message.CreateReplyMessage();
        }
    }
}
