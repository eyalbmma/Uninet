using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;
using Uninet.Domain.Models;
namespace Uninet.DATA.Services
{
    public class SendDocsRepository : ISendDocsRepository
    {
        private readonly IMongoCollection<SendDoc> _sendDocsCollection;
        private readonly IMongoCollection<BsonDocument> _MorningWebHookData;
        public SendDocsRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("Uninet");
            _sendDocsCollection = database.GetCollection<SendDoc>("send_docs_collection");
        }

        public async Task<bool> DocumentExistsAsync(SendDoc document)
        {
            var filter = Builders<SendDoc>.Filter.And(
                Builders<SendDoc>.Filter.Eq(x => x.documentId, document.documentId),
                Builders<SendDoc>.Filter.Eq(x => x.fis_id, document.fis_id),
                Builders<SendDoc>.Filter.Eq(x => x.entity_id_internal, document.entity_id_internal),
                Builders<SendDoc>.Filter.Eq(x => x.entity_vat_number, document.entity_vat_number)
            );

            var existing = await _sendDocsCollection.Find(filter).FirstOrDefaultAsync();
            return existing != null;
        }

        public async Task InsertDocumentAsync(SendDoc document)
        {
            await _sendDocsCollection.InsertOneAsync(document);
        }
    }
}
