using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Linx.Domain;
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
            var indexPath = Path.Combine(_cfg.SearchIndexBasePath, userID.ToString());

            System.IO.Directory.CreateDirectory(indexPath);

            using var dir = FSDirectory.Open(indexPath);

            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(_luceneVersion);

            // Create an index writer
            var writerConfig = new IndexWriterConfig(_luceneVersion, analyzer) {
                OpenMode = OpenMode.CREATE
            };

            var docs = links.Select(l => new Document {
                new StringField("id", l.ID.ToString(), Field.Store.YES),
                new TextField("title", l.Title, Field.Store.YES),
                new TextField("abstract", l.Abstract, Field.Store.YES)
            });

            using var writer = new IndexWriter(dir, writerConfig);

            writer.AddDocuments(docs);

            writer.Flush(triggerMerge: false, applyAllDeletes: false);
        }

        public void AddLink(Guid userID, Link link) =>
            throw new NotImplementedException();

        public void RemoveLink(Guid userID, Link link) =>
            throw new NotImplementedException();

        public void UpdateLink(Guid userID, Link link) =>
            throw new NotImplementedException();

        public IEnumerable<string> Search(Guid userID, string query)
        {
            var indexPath = Path.Combine(_cfg.SearchIndexBasePath, userID.ToString());

            using var dir = FSDirectory.Open(indexPath);

            using IndexReader reader = DirectoryReader.Open(dir);

            var searcher = new IndexSearcher(reader);

            var analyzer = new StandardAnalyzer(_luceneVersion);

            var parser = new MultiFieldQueryParser(_luceneVersion, new[] { "title", "abstract" }, analyzer);

            var hits = searcher.Search(parser.Parse(query), n: 20).ScoreDocs;

            var results = hits.Select(h => {
                var doc = searcher.Doc(h.Doc);
                return new {
                    Score = h.Score,
                    Doc = new { Title = doc.Get("title"), Abstract = doc.Get("abstract") }
                };
            });

            // IMPORTANT: ToList materialises the query results; if we don't do this we get an ObjectDisposedException
            return results.ToList().Select(r => $"{r.Score}: {r.Doc.Title}");
        }
    }
}
