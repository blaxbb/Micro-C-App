using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Sandbox.Queries;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Documents.Extensions;

namespace MicroCBuilder
{
    public static class LocalSearch
    {
        static bool Initialized = false;
        static LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
        static string Path => $"{Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path}/Index";
        static IndexWriter writer;
        public static void Init()
        {
            if(Initialized)
            {
                return;
            }
            Initialized = true;

            var dir = FSDirectory.Open(Path);
            //create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);

            //create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            writer = new IndexWriter(dir, indexConfig);

        }

        public static void ReplaceItems(List<Item> items)
        {
            writer.DeleteAll();
            writer.Flush(triggerMerge: false, applyAllDeletes: false);
            writer.Commit();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var doc = new Document()
                {
                    new TextField("Name", item.Name, Field.Store.YES),
                    new TextField("Brand", item.Brand, Field.Store.YES),
                    new Int32Field("index", i, Field.Store.YES)
                };
                writer.AddDocument(doc);
            }
            writer.Flush(triggerMerge: false, applyAllDeletes: false);
            writer.Commit();
        }

        public static IEnumerable<Item> Search(string query, List<Item> items)
        {
            var phrase = new FuzzyLikeThisQuery(10, new StandardAnalyzer(LuceneVersion.LUCENE_48));
            //var phrase = parser.Parse($"{query}");
            //var phrase = new Lucene.Net.Search.WildcardQuery(new Term("Name", query));
            var parts = query.Split(' ');
            foreach (var part in parts)
            {
                phrase.AddTerms(part, "Name", 0, 20);
            }
            phrase.AddTerms(parts[0], "Brand", 0, 5);

            var searcher = new IndexSearcher(writer.GetReader(true));

            var hits = searcher.Search(phrase, 50).ScoreDocs;
            Debug.WriteLine($"Hit :{hits.Count()}");
            foreach (var hit in hits)
            {
                var doc = searcher.Doc(hit.Doc);
                var index = doc.GetField<StoredField>("index").GetInt32Value().Value;
                var item = items[index];
                yield return item;
            }
        }
    }
}
