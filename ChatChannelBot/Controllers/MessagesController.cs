﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;
using ChatBridgeModel;

namespace ChatChannelBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private ChatAccountRepository m_Repository = new ChatAccountRepository();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                if (message.Text.ToLower() == CreateCommand)
                {
                    return await CreateBridgeMessage(message);
                }
                // calculate something for us to return
                int length = (message.Text ?? string.Empty).Length;

                // return our reply to the user
                return message.CreateReplyMessage($"You sent {length} characters\r\n\r\nChannelId:{message.From.ChannelId} Address:{message.From.Address} Id:{message.From.Id} Name:{message.From.Name} Me: {message.To.Address}");
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private async Task<Message> CreateBridgeMessage(Message message)
        {
            var res = await m_Repository.CreateBridge(message.From.Id, message.From.ChannelId);
            return message.CreateReplyMessage($"bridge created. Please tell them chat with you.\r\n`open bridge {res.ToString()}`");
        }

        private readonly static string CreateCommand = "create bridge";

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}