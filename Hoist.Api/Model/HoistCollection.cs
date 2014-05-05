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
        private static readonly List<string> RequiredProperties = new List<String> { "_id" };

        public string Name { get; private set; }
        public Type ConversionType { get; private set; }
        internal HoistClient Client { get; private set; }
        private string _endpoint;

        //TODO: Need to work out how to deal with _id , _rev

        public HoistCollection(HoistClient client, string name)
        {
            Client = client;
            Name = name;
            ConversionType = typeof(CollectionType);
            _endpoint = EndPoints.GenerateEndPoint(eEndPointType.Data, name);
        }

        public CollectionType Insert(CollectionType modelToInsert)
        {
            //TODO: Strip reserved parameters
            var response = Client.Post(_endpoint, modelToInsert);
            return Client.Processor.ProcessHoistData<CollectionType>(response);
        }

        public List<CollectionType> ToList()
        {
            var response = Client.Get(_endpoint);
            return Client.Processor.ProcessHoistData<List<CollectionType>>(response);
        }

        public CollectionType Get(string id)
        {
            var response = Client.Get(EndPoints.AddToEndPoint(_endpoint, id));
            return Client.Processor.ProcessHoistData<CollectionType>(response);
        }

        public CollectionType Update(CollectionType modelToUpdate, bool force = false)
        {

            IEnumerable<string> publicProperties;
            string id = null;
            if (modelToUpdate is HoistModel)
            {
                var hModel = modelToUpdate as HoistModel;
                publicProperties = hModel.Keys;
                id = hModel.Get("_id") as string;
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
                var url = EndPoints.AddToEndPoint(_endpoint, id, force ? new string[] { "force=true" } : null);
                var response = Client.Post(url, modelToUpdate);
                return Client.Processor.ProcessHoistData<CollectionType>(response);
            }
        }

        public bool Delete(string id)
        {
            if (id == null) { throw new ArgumentException("id can not be null"); }
            Client.Processor.ProcessResponse(Client.Delete(EndPoints.AddToEndPoint(_endpoint, id)));
            return true;
        }

        public bool Delete()
        {
            Client.Processor.ProcessResponse(Client.Delete(_endpoint));
            return true;
        }
               
    }
}
