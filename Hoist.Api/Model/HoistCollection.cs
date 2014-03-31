using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Hoist.Api.Exceptions;
using Hoist.Api.Http;

namespace Hoist.Api.Model
{
    public class HoistCollection<CollectionType> where CollectionType : class
    {
        private static readonly List<string> RequiredProperties = new List<String> { "_id", "_rev" };

        public string Name { get; private set; }
        public Type ConversionType { get; private set; }
        internal Hoist Client { get; private set; }
        private string _endpoint;

        //TODO: Need to work out how to deal with _id , _rev

        public HoistCollection(Hoist client, string name)
        {
            Client = client;
            Name = name;
            ConversionType = typeof(CollectionType);
            _endpoint = "https://data.hoi.io/" + WebUtility.UrlEncode(name);
        }

        public CollectionType Insert(CollectionType modelToInsert)
        {
            //TODO: Strip reserved parameters
            var response = Client.Post(_endpoint, modelToInsert);  
            return ProcessHoistData<CollectionType>(response);
        }
        
        public List<CollectionType> ToList()
        {
            var response = Client.Get(_endpoint);
            return ProcessHoistData<List<CollectionType>>(response);
        }

        public CollectionType Get(string id)
        {
            var response = Client.Get(_endpoint + "/" + WebUtility.UrlEncode(id));
            return ProcessHoistData<CollectionType>(response);
        }

        public CollectionType Update(CollectionType modelToUpdate, bool force=false)
        {
            
            IEnumerable<string> publicProperties;
            string id = null;
            if (modelToUpdate is HoistModel)
            {
                var hModel = modelToUpdate as HoistModel;
                publicProperties = hModel.Keys;
                id = hModel.Get("_id");
            }
            else
            {
                publicProperties = ReflectionUtils.ExtractProperties(modelToUpdate, ref id);
            }

            var missingProperties = RequiredProperties.Except(publicProperties);
            if (missingProperties.Count() > 0)
            {
                throw new MissingPropertiesException(missingProperties.ToArray());
            }
            else
            {
                var url = _endpoint + "/" + WebUtility.UrlEncode(id ?? "");
                if (force)
                {
                    url += "?force=true";
                }
                var response = Client.Post(url, modelToUpdate);
                return ProcessHoistData<CollectionType>(response);
            }
        }

        public bool Delete(string id)
        {
            if (id == null) { throw new ArgumentException("id can not be null"); }

            ProcessResponse(Client.Delete(_endpoint + "/" + WebUtility.UrlEncode(id)));
            return true;
        }

        public bool Delete()
        {
            ProcessResponse(Client.Delete(_endpoint));
            return true;
        }

        private ApiResponse ProcessResponse(ApiResponse response)
        {
            //Throw Exceptions on bad responses otherwise just return self..
            if (response.Code == 200)
            {
                return response;
            }
            else if (response.Code == 401 && response.WithWWWAuthenticate)
            {
                throw new BadApiKeyException();
            }
            else if (response.Code == 404)
            {
                throw new DataNotFoundException();
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

        private T ProcessHoistData<T>(ApiResponse response)
        {
            return Client.Serialiser.Deserialize<T>(ProcessResponse(response).Payload);
        }
    }
}
