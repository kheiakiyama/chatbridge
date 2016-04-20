using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using ChatBridgeModel;

namespace ChatChannelBot.Command
{
    public class CloseBridgeCommand : ICommand
    {
        public bool DoHandle(Message message)
        {
            return message.Text.ToLower() == "close bridge";
        }

        public async Task<Message> Reply(Message message)
        {
            var idText = message.GetBotUserData<string>(CommandTool.PropertyIdName);
            Guid id;
            if (!Guid.TryParse(idText, out id))
                return null;
            var res = await CommandTool.Instance.Repository.CloseBridge(id);
            return message.CreateReplyMessage($"bridge closed. see you next time!");
        }
    }
}
