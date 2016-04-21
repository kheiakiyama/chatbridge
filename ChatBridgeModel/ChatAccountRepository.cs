using Microsoft.Bot.Connector;
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
        /// <param name="account">UserAccount</param>
        /// <returns>RowKey Unique Id like a GUID</returns>
        public async Task<string> CreateBridge(ChannelAccount account)
        {
            var find = await FindBridge(account.Id);
            if (!string.IsNullOrEmpty(find))
                return find;

            var id = Guid.NewGuid();
            var newAccount = new ChatAccount() {
                PartitionKey = account.Id,
                RowKey = id.ToString(),
                OwnerId = account.Id,
                UserId = account.Id,
                Address = account.Address,
                Name = account.Name,
                ChannelId = account.ChannelId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
            };
            await m_Table.ExecuteAsync(TableOperation.Insert(newAccount));
            return newAccount.RowKey;
        }

        /// <summary>
        /// 既存の接続に参加
        /// </summary>
        /// <param name="id">RowKey</param>
        /// <param name="account">UserAccount</param>
        /// <returns>接続したChatAccount</returns>
        public async Task<ChatAccount> OpenBridge(Guid id, ChannelAccount account)
        {
            var ownerAccount = await FindAccounts(id);
            if (ownerAccount == null)
                return null;

            var self = await FindBridges(ownerAccount.UserId, account.Id);
            if (self != null)
                return self;

            var newId = Guid.NewGuid();
            var newAccount = new ChatAccount()
            {
                PartitionKey = ownerAccount.UserId,
                RowKey = newId.ToString(),
                OwnerId = ownerAccount.UserId,
                UserId = account.Id,
                Address = account.Address,
                Name = account.Name,
                ChannelId = account.ChannelId,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
            };
            await m_Table.ExecuteAsync(TableOperation.Insert(newAccount));
            return newAccount;
        }

        private async Task<ChatAccount> FindAccounts(Guid id)
        {
            TableQuery<ChatAccount> chatQuery = new TableQuery<ChatAccount>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));
            var response = await m_Table.ExecuteQuerySegmentedAsync(chatQuery, null);
            return response.Results.Count == 0 ? null : response.Results[0];
        }

        /// <summary>
        /// 接続の終了
        /// </summary>
        /// <param name="id">RowKey</param>
        /// <returns>True: Success Close</returns>
        public async Task<bool> CloseBridge(Guid id)
        {
            var self = await FindAccounts(id);
            if (self == null)
            return false;

            await m_Table.ExecuteAsync(TableOperation.Delete(self));
            return true;
        }

        private async Task<string> FindBridge(string ownerId)
        {
            var bridges = await FindBridges(ownerId);
            return bridges.Length > 0 ? bridges[0].RowKey : null;
        }

        public async Task<ChatAccount[]> FindBridges(string ownerId)
        {
            TableQuery<ChatAccount> chatQuery = new TableQuery<ChatAccount>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, ownerId));
            var response = await m_Table.ExecuteQuerySegmentedAsync(chatQuery, null);
            return response.Results.Count > 0 ? response.Results.ToArray() : new ChatAccount[] { };
        }
        
        private async Task<ChatAccount> FindBridges(string ownerId, string userId)
        {
            TableQuery<ChatAccount> chatQuery = new TableQuery<ChatAccount>()
                .Where(TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, ownerId),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, userId)));
            var response = await m_Table.ExecuteQuerySegmentedAsync(chatQuery, null);
            return response.Results.Count > 0 ? response.Results[0] : null;
        }

        public IEnumerable<ChatAccount> GetConnected(Guid id)
        {
            yield break;
        }
    }
}
