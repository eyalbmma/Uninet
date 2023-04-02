using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;

namespace Uninet.DATA.Services
{
    public class UninetSimulateGreenVoiceServiceDataAccess: IUninetSimulateGreenVoiceServiceDataAccess
    {
        private readonly IMongoCollection<BsonDocument> _greenvoicedocument;


        public UninetSimulateGreenVoiceServiceDataAccess(IMongoClient client)
        {
            var database = client.GetDatabase("Uninet");
           
            var greenvoicedocument = database.GetCollection<BsonDocument>("GreenVoice");

            _greenvoicedocument = greenvoicedocument;
            
        }

        public async Task<List<BsonDocument>> GeneralQueryBynameValue(string name, string value)
        {
            try
            {
                List<BsonDocument> Temp = new List<BsonDocument>();

                var query = Builders<BsonDocument>.Filter.Eq(name, value);
                //var query = Builders<BsonDocument>.Filter.Eq("id", "da5d064b-aac7-fcb4-45e9-c1123b3899d2");
                var results = _greenvoicedocument.Find(query).ToList();




                return results;


                //var client = new MongoClient("mongodb://localhost:27017");mongodb://localhost:27017/Uninet
                //var database = client.GetDatabase("Uninet");

                //// Get a reference to the collection
                //var collection = database.GetCollection<BsonDocument>("GreenVoice");

                //// Create a filter to match the "id" field
                //var filter = Builders<BsonDocument>.Filter.Eq("id", "da5d064b-aac7-fcb4-45e9-c1123b3899d2");

                //// Find the document with the matching "id" field
                //var document = collection.Find(filter).FirstOrDefault();
                // return null;


            }
            catch (Exception ex) { return null; }

        }



    }
}
