using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Bot.Connector;

namespace PipeMessageJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var connector = new ConnectorClient();

            //Message message = new Message();
            //message.From = new ChannelAccount() { ChannelId = "slack", Address = "U11E03Q85", };
            //message.To = new ChannelAccount() { ChannelId = "slack", Address = "U0B3WGWTV", Id = "kheiakiyama", Name = "kheiakiyama" };
            //message.Text = "test slack message";
            //message.Language = "ja";
            Message message = new Message();
            message.From = new ChannelAccount() { ChannelId = "slack", Address = "U11E03Q85", };
            message.To = new ChannelAccount() { ChannelId = "slack" };
            message.Participants = new List<ChannelAccount>();
            message.Participants.Add(new ChannelAccount() { ChannelId = "slack", Address = "U0B3WGWTV", Id = "kheiakiyama", Name = "kheiakiyama" });
            message.Participants.Add(new ChannelAccount() { ChannelId = "slack", Address = "U11ERC674", Id = "slacktest", Name = "slacktest" });
            message.Text = "multiple users message";
            message.Language = "ja";

            connector.Messages.SendMessage(message);

            //var host = new JobHost();
            //// The following code will invoke a function called ManualTrigger and 
            //// pass in data (value in this case) to the function
            //host.Call(typeof(Functions).GetMethod("ManualTrigger"), new { value = 20 });
        }
    }
}
