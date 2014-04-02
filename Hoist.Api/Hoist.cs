using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Hoist.Api.Exceptions;
using Hoist.Api.Http;
using Hoist.Api.Model;

namespace Hoist.Api
{
    public class Hoist
    {
        private string _apiKey;
        private string _session;
        private IHttpLayer _httpLayer;
        internal ResponseProcessor Processor { get; set; }
        
        public Hoist(string apiKey) : this(apiKey, new HoistHttpLayer()) { } 

        public Hoist(string apiKey, IHttpLayer httpLayer)
        {
            _apiKey = apiKey;
            _httpLayer = httpLayer;
            Processor = new ResponseProcessor();
        }

        public HoistUser Login(string email, string password)
        {
            var response = Post(EndPoints.GenerateEndPoint(eEndPointType.Login), new LoginPayload(email, password)); 
            _session = response.HoistSession;
            return Processor.ProcessHoistData<HoistUser>(response);
        }

        public HoistUser Status()
        {
            var response = Get(EndPoints.GenerateEndPoint(eEndPointType.Status));
            return Processor.ProcessHoistData<HoistUser>(response);
        }

        public void Logout()
        {
            var response = Processor.ProcessHoistData<HoistModel>(Post(EndPoints.GenerateEndPoint(eEndPointType.Logout), null));   
        }

        public List<HoistBucket> ListBuckets()
        {
            var response = Get(EndPoints.GenerateEndPoint(eEndPointType.ListBuckets));
            return Processor.ProcessHoistData<List<HoistBucket>>(response);
        }

        public bool CreateBucket(string bucketName)
        {
            return Processor.ProcessHoistData<HoistModel>(Post(EndPoints.GenerateEndPoint(eEndPointType.CreateBucket, bucketName), new { })) != null;
        }

        public bool SetCurrentBucket(HoistBucket bucket)
        {
            var response = Post(EndPoints.GenerateEndPoint(eEndPointType.SetCurrentBucket, bucket.key), new { });
            Processor.ProcessResponse(response);
            return true;
        }

        public HoistCollection<HoistModel> GetCollection(string collectionName)
        {
            return GetCollection<HoistModel>(collectionName);
        }

        public HoistCollection<CollectionType> GetCollection<CollectionType>(string collectionName) where CollectionType : class
        {
            return new HoistCollection<CollectionType>(this, collectionName);
        }

        public bool SendNotification(string notificationName, object parameters)
        {
            var response = Processor.ProcessHoistData<HoistModel>(Post(EndPoints.GenerateEndPoint(eEndPointType.SendNotification, notificationName ?? ""), parameters));
            return true;            
        }
        
        internal ApiResponse Post(string endPoint, object data)
        {
            return _httpLayer.Post(endPoint, _apiKey, _session, Processor.ToHoist(data));
        }

        internal ApiResponse Get(string endPoint)
        {
            return _httpLayer.Get(endPoint, _apiKey, _session);            
        }

        internal ApiResponse Delete(string endPoint)
        {
            return _httpLayer.Delete(endPoint, _apiKey, _session);
        }


        
    }
}
