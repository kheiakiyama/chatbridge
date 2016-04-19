using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBridgeModel
{
    /*
     *  RowKey Owner(partialKey)  User  Channel
     *    A-A       A              A     slack
     *    A-B       A              B     skype
     *    C-C       C              C     slack
     */

    public class ChatAccount : TableEntity
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; }
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
