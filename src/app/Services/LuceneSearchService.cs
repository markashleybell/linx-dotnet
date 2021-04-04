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
            var indexPath = GetIndexPath(userID);

            System.IO.Directory.CreateDirectory(indexPath);

            using var dir = FSDirectory.Open(indexPath);

            var analyzer = new StandardAnalyzer(_luceneVersion);

            var writerConfig = new IndexWriterConfig(_luceneVersion, analyzer) {
                OpenMode = OpenMode.CREATE
            };

            var docs = links.Select(AsDocument);

            using var writer = new IndexWriter(dir, writerConfig);

            writer.AddDocuments(docs);

            writer.Flush(triggerMerge: false, applyAllDeletes: false);
        }

        public void AddLink(Guid userID, Link link)
        {
            var indexPath = GetIndexPath(userID);

            using var dir = FSDirectory.Open(indexPath);

            var analyzer = new StandardAnalyzer(_luceneVersion);

            var writerConfig = new IndexWriterConfig(_luceneVersion, analyzer);

            using var writer = new IndexWriter(dir, writerConfig);

            writer.AddDocument(AsDocument(link));

            writer.Flush(triggerMerge: false, applyAllDeletes: false);
        }

        public void RemoveLink(Guid userID, Link link)
        {
            var indexPath = GetIndexPath(userID);

            using var dir = FSDirectory.Open(indexPath);

            var analyzer = new StandardAnalyzer(_luceneVersion);

            var writerConfig = new IndexWriterConfig(_luceneVersion, analyzer);

            using var writer = new IndexWriter(dir, writerConfig);

            writer.DeleteDocuments(new Term("id", link.ID.ToString()));

            writer.Flush(triggerMerge: false, applyAllDeletes: true);
        }

        public void UpdateLink(Guid userID, Link link)
        {
            var indexPath = GetIndexPath(userID);

            using var dir = FSDirectory.Open(indexPath);

            var analyzer = new StandardAnalyzer(_luceneVersion);

            var writerConfig = new IndexWriterConfig(_luceneVersion, analyzer);

            using var writer = new IndexWriter(dir, writerConfig);

            writer.UpdateDocument(new Term("id", link.ID.ToString()), AsDocument(link));

            writer.Flush(triggerMerge: false, applyAllDeletes: false);
        }

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
    }
}
