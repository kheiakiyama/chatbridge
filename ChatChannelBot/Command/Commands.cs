using ChatBridgeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatChannelBot.Command
{
    public static class Commands
    {
        public static IEnumerable<ICommand> GetItems()
        {
            yield return new CreateBridgeCommand();
            yield return new OpenBridgeCommand();
            yield return new CloseBridgeCommand();
            yield return new MessagePipeCommand();
        }
    }
}
