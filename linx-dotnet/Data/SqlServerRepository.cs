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
        private readonly Guid? _userId;

        public SqlServerRepository(
            IOptionsMonitor<Settings> optionsMonitor,
            Guid? userId)
        {
            _cfg = optionsMonitor.CurrentValue;
            _userId = userId;
        }

        public async Task<Link> CreateLinkAsync(Link link) =>
            await WithConnectionAsync(async conn => {
                var param = new {
                    UserID = _userId,
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

                return await ReadLinkAsync(link.ID);
            });

        public async Task<Link> ReadLinkAsync(Guid id) =>
            await WithConnectionAsync(conn => {
                return conn.QuerySingleOrDefaultSpAsync<Link>(
                    sql: "ReadLink",
                    param: new { id }
                );
            });

        public async Task<IEnumerable<Link>> ReadAllLinksAsync() =>
            await WithConnectionAsync(conn => {
                return conn.QuerySpAsync<Link>(
                    sql: "ReadLinks",
                    param: new { UserID = _userId }
                );
            });

        public async Task<Link> UpdateLinkAsync(Link link) =>
            await WithConnectionAsync(async conn => {
                var param = new {
                    UserID = _userId,
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

                return await ReadLinkAsync(link.ID);
            });

        public async Task DeleteLinkAsync(Guid id) =>
            await WithConnectionAsync(conn => {
                return conn.ExecuteSpAsync(
                    sql: "DeleteDocument",
                    param: new {
                        ID = id
                    }
                );
            });

        public async Task<IEnumerable<Tag>> ReadAllTagsAsync() =>
            await WithConnectionAsync(conn => {
                return conn.QueryAsync<Tag>(
                    sql: "SELECT ID, Label, (SELECT COUNT(*) FROM Tags_Links td WHERE td.TagID = t.ID) AS UseCount FROM Tags t ORDER BY t.Label"
                );
            });

        public async Task MergeTagsAsync(Guid id, IEnumerable<Guid> tagIdsToMerge) =>
            await WithConnectionAsync(conn => {
                return conn.ExecuteSpAsync(
                    sql: "MergeTags",
                    param: new {
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
