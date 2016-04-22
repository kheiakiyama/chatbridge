using ChatBridgeModel;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBridgeBot.Command
{
    public class CommandTool
    {
        public ChatAccountRepository Repository { get; private set; }
        public ConnectorClient Client {  get; private set; }
        public ChatRequest Request { get; private set; }
        
        public static CommandTool Instance { get; private set; }

        static CommandTool()
        {
            Instance = new CommandTool();
        }

        private CommandTool()
        {
            Repository = new ChatAccountRepository();
            Client = new ConnectorClient();
            Request = new ChatRequest();
        }
    }
}
