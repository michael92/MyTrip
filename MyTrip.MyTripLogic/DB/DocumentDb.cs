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
        private string _databaseId;
        private string _collectionId;
        private Database _database;
        private DocumentCollection _collection;
        private DocumentClient _client;

        public DocumentDb(string database, string collection)
        {
            _databaseId = database;
            _collectionId = collection;
            ReadOrCreateDatabase().Wait();
            if (collection != null)
            {
                ReadOrCreateCollection(_database.SelfLink).Wait();
            }
        }

        public DocumentClient Client
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

        public DocumentCollection Collection
        {
            get { return _collection; }
        }

        private async Task ReadOrCreateCollection(string databaseLink)
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

        private async Task ReadOrCreateDatabase()
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
        public List<Media> GetMedia(string id)
        {
            return Client.CreateDocumentQuery<Media>(Collection.DocumentsLink)
                        .Where(x => x.Id == id)
                       .AsEnumerable()
                       .ToList<Media>();
        }
        public async Task<Document> UpdateItemAsync(Media item)
        {
            Document doc = GetDocument(item.Id);
            return await Client.ReplaceDocumentAsync(doc.SelfLink, item);
        }
        public Document GetDocument(string id)
        {
            return Client.CreateDocumentQuery(Collection.DocumentsLink)
                          .Where(d => d.Id == id)
                          .AsEnumerable()
                          .FirstOrDefault();
        }
    }
}