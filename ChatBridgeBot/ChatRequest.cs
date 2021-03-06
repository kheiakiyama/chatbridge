﻿using ChatBridgeModel;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBridgeBot
{
    public class ChatRequest
    {
        private Dictionary<string, string> m_Adresses = new Dictionary<string, string>();

        public void Recive(Message message)
        {
            if (!m_Adresses.ContainsKey(message.To.ChannelId))
                m_Adresses.Add(message.To.ChannelId, message.To.Address);
        }

        public Message CreateMessage(string address, string channelId, string text)
        {
            if (!m_Adresses.ContainsKey(channelId))
                throw new NotSupportedException();

            return new Message()
            {
                From = new ChannelAccount() { ChannelId = channelId, Address = m_Adresses[channelId], },
                To = new ChannelAccount() { ChannelId = channelId, Address = address },
                Text = text,
                Language = "ja"
            };
        }

        public void SetAccountData(Message message, ChatAccount account)
        {
            message.SetBotUserData(PropertyAccount, account);
        }

        public ChatAccount GetAccountData(Message message)
        {
            return message.GetBotUserData<ChatAccount>(PropertyAccount);
        }

        private static readonly string PropertyAccount = "Account";
    }
}
