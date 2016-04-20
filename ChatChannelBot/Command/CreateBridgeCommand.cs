using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using ChatBridgeModel;

namespace ChatChannelBot.Command
{
    public class CreateBridgeCommand : ICommand
    {
        public bool DoHandle(Message message)
        {
            return message.Text.ToLower() == "create bridge";
        }

        public async Task<Message> Reply(Message message)
        {
            var res = await CommandTool.Instance.Repository.CreateBridge(message.From);
            var msg = message.CreateReplyMessage($"bridge created. Please tell them chat with you.\r\n`open bridge {res}`");
            msg.SetBotUserData(CommandTool.PropertyIdName, res);
            return msg;
        }
    }
}
