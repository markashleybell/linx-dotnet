using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Linx.Domain;
using Linx.Support;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Linx.Data
{
    public class SqlServerRepository : IRepository
    {
        private readonly Settings _cfg;

        public SqlServerRepository(IOptionsMonitor<Settings> optionsMonitor) =>
            _cfg = optionsMonitor.CurrentValue;

        public async Task<Link> CreateLinkAsync(Guid userID, Link link) =>
            await WithConnectionAsync(async conn => {
                var param = new {
                    userID,
                    link.ID,
                    link.Title,
                    link.Url,
                    link.Abstract,
                    Tags = link.Tags.AsDataRecords().AsTableValuedParameter("dbo.TagList")
                };

                await conn.ExecuteSpAsync(
                    sql: "CreateLink",
                    param: param
                );

                return await ReadLinkAsync(userID, link.ID);
            });

        public async Task<Link> ReadLinkAsync(Guid userID, Guid id) =>
            await WithConnectionAsync(conn => {
                return conn.QuerySingleOrDefaultSpAsync<Link>(
                    sql: "ReadLink",
                    param: new { userID, id }
                );
            });

        public async Task<IEnumerable<Link>> ReadAllLinksAsync(Guid userID) =>
            await WithConnectionAsync(conn => {
                return conn.QuerySpAsync<Link>(
                    sql: "ReadLinks",
                    param: new { userID }
                );
            });

        public async Task<Link> UpdateLinkAsync(Guid userID, Link link) =>
            await WithConnectionAsync(async conn => {
                var param = new {
                    UserID = userID,
                    link.ID,
                    link.Title,
                    link.Url,
                    link.Abstract,
                    Tags = link.Tags.AsDataRecords().AsTableValuedParameter("dbo.TagList")
                };

                await conn.ExecuteSpAsync(
                    sql: "UpdateLink",
                    param: param
                );

                return await ReadLinkAsync(userID, link.ID);
            });

        public async Task DeleteLinkAsync(Guid userID, Guid id) =>
            await WithConnectionAsync(conn => {
                return conn.ExecuteSpAsync(
                    sql: "DeleteLink",
                    param: new { userID, id }
                );
            });

        public async Task<IEnumerable<Tag>> ReadAllTagsAsync(Guid userID) =>
            await WithConnectionAsync(conn => {
                return conn.QueryAsync<Tag>(
                    sql: "SELECT ID, Label, (SELECT COUNT(*) FROM Tags_Links td WHERE td.TagID = t.ID) AS UseCount FROM Tags t WHERE t.UserID = @UserID ORDER BY t.Label",
                    param: new { userID }
                );
            });

        public async Task MergeTagsAsync(Guid userID, Guid id, IEnumerable<Guid> tagIdsToMerge) =>
            await WithConnectionAsync(conn => {
                return conn.ExecuteSpAsync(
                    sql: "MergeTags",
                    param: new {
                        userID,
                        TagID = id,
                        TagIdsToMerge = tagIdsToMerge.AsDataRecords().AsTableValuedParameter("dbo.GuidList")
                    }
                );
            });

        public async Task<User> FindUserByEmail(string email) =>
            await WithConnectionAsync(conn => {
                return conn.QuerySingleOrDefaultAsync<User>(
                    sql: "SELECT * FROM Users WHERE Email = @Email",
                    param: new { email }
                );
            });

        public async Task<User> FindUserByApiKey(string apiKey) =>
            await WithConnectionAsync(conn => {
                return conn.QuerySingleOrDefaultAsync<User>(
                    sql: "SELECT * FROM Users WHERE ApiKey = @ApiKey",
                    param: new { apiKey }
                );
            });

        private async Task WithConnectionAsync(Func<SqlConnection, Task> action)
        {
            using (var connection = new SqlConnection(_cfg.ConnectionString))
            {
                await action(connection);
            }
        }

        private async Task<T> WithConnectionAsync<T>(Func<SqlConnection, Task<T>> action)
        {
            using (var connection = new SqlConnection(_cfg.ConnectionString))
            {
                return await action(connection);
            }
        }
    }
}
