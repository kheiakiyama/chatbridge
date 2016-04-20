using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBridgeModel
{
    public class ChatAccountRepository
    {
        public ChatAccountRepository()
        {
            var connectionString = Settings.Get("StorageConnectionString");
            Trace.WriteLine(connectionString);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            m_Table = tableClient.GetTableReference(tableName);
            m_Table.CreateIfNotExists();
        }

        private static readonly string tableName = "chataccount";
        private CloudTable m_Table;

        /// <summary>
        /// 接続の開始
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="channelId">ChannelId</param>
        /// <returns></returns>
        public async Task<string> CreateBridge(string userId, string channelId)
        {
            var find = await FindBridge(userId);
            if (!string.IsNullOrEmpty(find))
                return find;

            var id = Guid.NewGuid();
            var account = new ChatAccount() {
                PartitionKey = userId,
                RowKey = id.ToString(),
                OwnerId = userId,
                UserId = userId,
                ChannelId = channelId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
            };
            await m_Table.ExecuteAsync(TableOperation.Insert(account));
            return account.RowKey;
        }

        public async Task<bool> OpenBridge(Guid id, string userId, string channelId)
        {
            TableQuery<ChatAccount> chatQuery = new TableQuery<ChatAccount>()
              .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));
            var response = await m_Table.ExecuteQuerySegmentedAsync(chatQuery, null);
            if (response.Results.Count == 0)
                return false;

            var newId = Guid.NewGuid();
            var account = new ChatAccount()
            {
                PartitionKey = userId,
                RowKey = newId.ToString(),
                OwnerId = response.Results[0].UserId,
                UserId = userId,
                ChannelId = channelId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
            };
            await m_Table.ExecuteAsync(TableOperation.Insert(account));
            return true;
        }

        private async Task<string> FindBridge(string ownerId)
        {
            TableQuery<ChatAccount> chatQuery = new TableQuery<ChatAccount>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, ownerId));
            var response = await m_Table.ExecuteQuerySegmentedAsync(chatQuery, null);
            return response.Results.Count > 0 ? response.Results[0].RowKey : null;
        }

        public IEnumerable<ChatAccount> GetConnected(Guid id)
        {
            yield break;
        }
    }
}
