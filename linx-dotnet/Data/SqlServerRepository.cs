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
        private readonly int? _userId;

        public SqlServerRepository(
            IOptionsMonitor<Settings> optionsMonitor,
            int? userId)
        {
            _cfg = optionsMonitor.CurrentValue;
            _userId = userId;
        }

        public async Task<User> FindUserByEmail(string email) =>
            await WithConnectionAsync(conn => {
                return conn.QuerySingleOrDefaultAsync<User>(
                    sql: "SELECT * FROM Users WHERE email = @email",
                    param: new { email }
                );
            });

        public async Task<IEnumerable<Link>> GetLinksAsync() =>
            await WithConnectionAsync(conn => {
                return conn.QueryAsync<Link>(
                    sql: "SELECT * FROM links WHERE user_id = @user_id",
                    param: new { user_id = _userId }
                );
            });

        public async Task<IEnumerable<Tag>> GetTags() =>
            await WithConnectionAsync(conn => {
                return conn.QueryAsync<Tag>(
                    sql: "SELECT ID, Label, (SELECT COUNT(*) FROM Tags_Documents td WHERE td.TagID = t.ID) AS UseCount FROM Tags t ORDER BY t.Label"
                );
            });

        public async Task MergeTags(Guid id, IEnumerable<Guid> tagIdsToMerge) =>
            await WithConnectionAsync(conn => {
                return conn.ExecuteSpAsync(
                    sql: "MergeTags",
                    param: new {
                        TagID = id,
                        TagIdsToMerge = tagIdsToMerge.AsDataRecords().AsTableValuedParameter("dbo.GuidList")
                    }
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
