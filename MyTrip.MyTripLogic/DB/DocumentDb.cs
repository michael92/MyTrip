using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using MyTrip.MyTripLogic.Models;

namespace MyTrip.MyTripLogic.DB
{
    public class DocumentDb
    {
        private static string _databaseId;
        private static string _collectionId;
        private static Database _database;
        private static DocumentCollection _collection;
        private static DocumentClient _client;

        public DocumentDb(string database, string collection)
        {
            _databaseId = database;
            _collectionId = collection;
            ReadOrCreateDatabase().Wait();
            ReadOrCreateCollection(_database.SelfLink).Wait();
        }

        public static DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["endpoint"];
                    string authKey = ConfigurationManager.AppSettings["authKey"];

                    Uri endpointUri = new Uri(endpoint);
                    _client = new DocumentClient(endpointUri, authKey);
                }
                return _client;
            }
        }

        public DocumentClient getClient()
        {
            string endpoint = ConfigurationManager.AppSettings["endpoint"];
            string authKey = ConfigurationManager.AppSettings["authKey"];

            Uri endpointUri = new Uri(endpoint);
            return new DocumentClient(endpointUri, authKey);

        }

        public DocumentCollection getCollection()
        {
            return _collection;
        }

        public static string Database = "MyTripDb";

        protected static DocumentCollection Collection
        {
            get { return _collection; }
        }

        private static async Task ReadOrCreateCollection(string databaseLink)
        {
            var collections = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(col => col.Id == _collectionId).ToArray();

            if (collections.Any())
            {
                _collection = collections.First();
            }
            else
            {
                _collection = await Client.CreateDocumentCollectionAsync(databaseLink,
                    new DocumentCollection { Id = _collectionId });
            }
        }

        private static async Task ReadOrCreateDatabase()
        {
            var query = Client.CreateDatabaseQuery()
                            .Where(db => db.Id == _databaseId);

            var databases = query.ToArray();
            if (databases.Any())
            {
                _database = databases.First();
            }
            else
            {
                _database = await Client.CreateDatabaseAsync(new Microsoft.Azure.Documents.Database { Id = _databaseId });
            }
        }
        public static List<Media> GetMedia(string id)
        {
            return Client.CreateDocumentQuery<Media>(Collection.DocumentsLink)
                        .Where(x => x.Id == id)
                       .AsEnumerable()
                       .ToList<Media>();
        }
        public static async Task<Document> UpdateItemAsync(Media item)
        {
            Document doc = GetDocument(item.Id);
            return await Client.ReplaceDocumentAsync(doc.SelfLink, item);
        }
        public static Document GetDocument(string id)
        {
            return Client.CreateDocumentQuery(Collection.DocumentsLink)
                          .Where(d => d.Id == id)
                          .AsEnumerable()
                          .FirstOrDefault();
        }

        public Document GetDocumentById(string id)
        {
            return _client.CreateDocumentQuery(Collection.DocumentsLink)
                          .Where(d => d.Id == id)
                          .AsEnumerable()
                          .FirstOrDefault();
        }
    }
}