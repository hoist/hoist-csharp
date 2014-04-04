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

        public HoistBucket CreateBucket(string bucketName)
        {
            return CreateBucket(bucketName, new HoistModel());
        }

        public HoistBucket CreateBucket(string bucketName, HoistModel hoistModel)
        {
            return Processor.ProcessHoistData<HoistBucket>(Post(EndPoints.GenerateEndPoint(eEndPointType.CreateBucket, bucketName), hoistModel));
        }

        public HoistBucket EnterBucket(string key)
        {
            return Processor.ProcessHoistData<HoistBucket>(Post(EndPoints.GenerateEndPoint(eEndPointType.SetCurrentBucket, key), new { }));
        }

        public bool LeaveBucket()
        {
            var obj = Processor.ProcessResponse(Post(EndPoints.GenerateEndPoint(eEndPointType.SetCurrentBucket, "default"), new { }));
            return true;
        }

        public HoistBucket CurrentBucket()
        {
            return Processor.ProcessHoistData<HoistBucket>(Get(EndPoints.GenerateEndPoint(eEndPointType.GetCurrentBucket)), ignore404:true );
        }

        public HoistBucket UpdateBucket(HoistBucket bucket)
        {
            return Processor.ProcessHoistData<HoistBucket>(Post(EndPoints.GenerateEndPoint(eEndPointType.UpdateBucket, bucket.key), bucket.meta), ignore401:false);
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

        public HoistProxy GetProxy(string proxyName)
        {
            return GetProxy(proxyName, null);
        }

        public HoistProxy GetProxy(string proxyName, string proxyToken)
        {
            return new HoistProxy(this, proxyName, proxyToken);
        }

        internal ApiResponse Post(string endPoint, object data, string oauthToken = null)
        {
            return _httpLayer.Post(endPoint, _apiKey, _session, oauthToken, Processor.ToHoist(data));
        }

        internal ApiResponse Put(string endPoint, object data, string oauthToken = null)
        {
            return _httpLayer.Put(endPoint, _apiKey, _session, oauthToken, Processor.ToHoist(data));
        }

        internal ApiResponse Get(string endPoint, string oauthToken=null)
        {
            return _httpLayer.Get(endPoint, _apiKey, _session, oauthToken);            
        }

        internal ApiResponse Delete(string endPoint, string oauthToken = null)
        {
            return _httpLayer.Delete(endPoint, _apiKey, _session, oauthToken);
        }





        
    }
}
