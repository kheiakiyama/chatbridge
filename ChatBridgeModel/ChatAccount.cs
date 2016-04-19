using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBridgeModel
{
    public class ChatAccount : TableEntity
    {
        public string OwnerId { get; set; }
        public string Id { get; set; }
        public string ChannelId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
