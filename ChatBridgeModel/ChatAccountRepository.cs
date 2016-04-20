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
        public async Task<Guid> CreateBridge(string userId, string channelId)
        {
            var find = await FindBridge(userId);
            if (find.HasValue)
                return find.Value;

            var account = new ChatAccount() {
                PartitionKey = userId,
                RowKey = GetRowKey(userId, userId),
                Id = Guid.NewGuid(),
                OwnerId = userId,
                UserId = userId,
                ChannelId = channelId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
            };
            await m_Table.ExecuteAsync(TableOperation.Insert(account));
            return account.Id;
        }

        public async Task<bool> ConnectBridge(Guid id, string userId, string channelId)
        {
            return false;
        }

        private async Task<Guid?> FindBridge(string ownerId)
        {
            TableQuery<ChatAccount> chatQuery = new TableQuery<ChatAccount>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, ownerId));
            var response = await m_Table.ExecuteQuerySegmentedAsync(chatQuery, null);
            return response.Results.Count > 0 ? response.Results[0].Id : (Guid?)null;
        }

        private string GetRowKey(string ownerId, string userId)
        {
            return $"{ownerId}-{userId}";
        }

        public IEnumerable<ChatAccount> GetConnected(Guid id)
        {
            yield break;
        }
    }
}
