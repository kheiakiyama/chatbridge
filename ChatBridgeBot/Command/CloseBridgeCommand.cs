using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using ChatBridgeModel;

namespace ChatBridgeBot.Command
{
    public class CloseBridgeCommand : ICommand
    {
        public bool DoHandle(Message message)
        {
            return message.Text.ToLower() == "close bridge";
        }

        public async Task<Message> Reply(Message message)
        {
            var account = CommandTool.Instance.Request.GetAccountData(message);
            if (account == null)
                return message.CreateReplyMessage($"can't close bridge.");
            var res = await CommandTool.Instance.Repository.CloseBridge(new Guid(account.RowKey));
            var msg = message.CreateReplyMessage($"bridge closed. see you next time!");
            CommandTool.Instance.Request.SetAccountData(msg, null);
            return msg;
        }
    }
}
