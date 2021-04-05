using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Linx.Domain;
using Linx.Models;
using Linx.Support;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Options;

namespace Linx.Services
{
    public class LuceneSearchService : ISearchService
    {
#pragma warning disable IDE1006 // Naming Styles
        private const LuceneVersion _luceneVersion = LuceneVersion.LUCENE_48;
#pragma warning restore IDE1006 // Naming Styles

        private readonly Settings _cfg;

        public LuceneSearchService(IOptionsMonitor<Settings> optionsMonitor) =>
            _cfg = optionsMonitor.CurrentValue;

        public void DeleteAndRebuildIndex(Guid userID, IEnumerable<Link> links)
        {
            System.IO.Directory.CreateDirectory(GetIndexPath(userID));

            WithIndexWriter(userID, writer => writer.AddDocuments(links.Select(AsDocument)), overwriteIndex: true);
        }

        public void AddLink(Guid userID, Link link) =>
            WithIndexWriter(userID, writer => writer.AddDocument(AsDocument(link)));

        public void RemoveLink(Guid userID, Link link) =>
            WithIndexWriter(userID, writer => writer.DeleteDocuments(new Term("id", link.ID.ToString())), applyDeletes: true);

        public void UpdateLink(Guid userID, Link link) =>
            WithIndexWriter(userID, writer => writer.UpdateDocument(new Term("id", link.ID.ToString()), AsDocument(link)));

        public IEnumerable<SearchResult> Search(Guid userID, string query)
        {
            using var dir = FSDirectory.Open(GetIndexPath(userID));

            using IndexReader reader = DirectoryReader.Open(dir);

            var searcher = new IndexSearcher(reader);

            var analyzer = new StandardAnalyzer(_luceneVersion);

            var boosts = new Dictionary<string, float> {
                ["title"] = 1.0f,
                ["url"] = 0.5f,
                ["abstract"] = 0.8f,
                ["tags"] = 1.5f
            };

            var parser = new MultiFieldQueryParser(_luceneVersion, new[] { "title", "url", "abstract", "tags" }, analyzer, boosts);

            var luceneQuery = parser.Parse(query);

            var hits = searcher.Search(luceneQuery, n: 20).ScoreDocs;

            var results = hits.Select(h => {
                var doc = searcher.Doc(h.Doc);
                return new SearchResult(
                    h.Score,
                    new Link(
                        new Guid(doc.Get("id")),
                        doc.Get("title"),
                        doc.Get("url"),
                        doc.Get("abstract"),
                        doc.Get("tags")
                    )
                );
            });

            // IMPORTANT: ToList materialises the query results; if we don't do this we get an ObjectDisposedException
            return results.ToList();
        }

        private static Document AsDocument(Link link) =>
            new() {
                new StringField("id", link.ID.ToString(), Field.Store.YES),
                new TextField("title", link.Title, Field.Store.YES),
                new StringField("url", link.Url, Field.Store.YES),
                new TextField("abstract", link.Abstract, Field.Store.YES),
                new TextField("tags", string.Join("|", link.Tags.Select(t => t.Label)), Field.Store.YES)
            };

        private string GetIndexPath(Guid userID) =>
            Path.Combine(_cfg.SearchIndexBasePath, userID.ToString());

        private void WithIndexWriter(Guid userID, Action<IndexWriter> f, bool overwriteIndex = false, bool applyDeletes = false)
        {
            var analyzer = new StandardAnalyzer(_luceneVersion);

            var writerConfig = new IndexWriterConfig(_luceneVersion, analyzer) {
                OpenMode = overwriteIndex ? OpenMode.CREATE : OpenMode.CREATE_OR_APPEND
            };

            using var dir = FSDirectory.Open(GetIndexPath(userID));

            using var writer = new IndexWriter(dir, writerConfig);

            f(writer);

            writer.Flush(triggerMerge: false, applyAllDeletes: applyDeletes);
        }
    }
}
