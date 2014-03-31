using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hoist.Api.Exceptions;
using Hoist.Api.Http;

namespace Hoist.Api.Model
{
    public class HoistCollection<CollectionType> where CollectionType : class
    {
        public string Name { get; private set; }
        public Type ConversionType { get; private set; }
        public Hoist Client { get; private set; }
        private string _endpoint;

        public List<CollectionType> ToList()
        {
            return null;
        }

        public HoistCollection(Hoist client, string name)
        {
            Client = client;
            Name = name;
            ConversionType = typeof(CollectionType);
            _endpoint = "https://data.hoi.io/" + name;
        }

        public CollectionType Insert(CollectionType modelToInsert)
        {
            var response = Client.Post(_endpoint, modelToInsert);
            return ProcessHoistData(response);
        }

        private CollectionType ProcessHoistData(ApiResponse response)
        {
            if (response.Code == 200)
            {
                return Client.Serialiser.Deserialize<CollectionType>(response.Payload);
            }
            else if (response.Code == 401 && response.WithWWWAuthenticate)
            {
                throw new BadApiKeyException();
            }
            else if (response.Code == 409)
            {
                throw new DataConflictException();
            }
            else
            {
                throw new UnexpectedResponseException(response);
            }
        }

    }
}
