using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using ChatBridgeModel;

namespace ChatBridgeBot.Command
{
    public class HelpCommand : ICommand
    {
        public bool DoHandle(Message message)
        {
            return message.Text.ToLower() == "help";
        }

        public async Task<Message> Reply(Message message)
        {
            return message.CreateReplyMessage(@"create bridge, open bridge {GUID}, close bridge");
        }
    }
}
