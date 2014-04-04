using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hoist.Api.Http;

namespace Hoist.Api.Model
{
    public class HoistProxy
    {
        public string Name { get; private set; }
        public bool HasToken { get { return !String.IsNullOrEmpty(_proxyToken); } }
        internal HoistClient Client { get; private set; }
        private string _endpoint;
        private string _proxyToken;

        public HoistProxy(HoistClient client, string proxyName, string proxyToken)
        {
            Client = client;
            Name = proxyName;
            _endpoint = EndPoints.GenerateEndPoint(eEndPointType.Proxy, proxyName);
            _proxyToken = proxyToken;
        }

        public HoistProxyResponse Connect()
        {
            var response = Client.Post(EndPoints.AddToEndPoint(_endpoint, "connect"), null);
            
            var retval = Client.Processor.ProcessHoistData<HoistProxyResponse>(response);
            if (!String.IsNullOrEmpty(retval.token))
            {
                _proxyToken = retval.token;
            }
            return retval;
        }

        public bool Disconnect()
        {
            var response = Client.Get(EndPoints.AddToEndPoint(_endpoint, "disconnect"));
            var payload = Client.Processor.ProcessResponse(response);
            _proxyToken = null;
            return true;
        }

        public HoistModel GET(string proxyEndpoint)
        {
            return GET<HoistModel>(proxyEndpoint);
        }

        public T GET<T>(string proxyEndpoint) where T : class
        {
            return Client.Processor.ProcessHoistData<T>(
                Client.Get(
                    EndPoints.AddToEndPoint(_endpoint, proxyEndpoint, encodeKey: false),
                    _proxyToken
                    ));
        }

        public HoistModel POST(string proxyEndpoint, object model)
        {
            return POST<HoistModel>(proxyEndpoint, model);
        }

        public T POST<T>(string proxyEndpoint, object model) where T : class
        {
            return Client.Processor.ProcessHoistData<T>(
                Client.Post(
                    EndPoints.AddToEndPoint(_endpoint, proxyEndpoint, encodeKey: false),
                    model,
                    _proxyToken
                    ));
        }

        public HoistModel DELETE(string proxyEndpoint)
        {
            return DELETE<HoistModel>(proxyEndpoint);
        }

        public T DELETE<T>(string proxyEndpoint) where T: class
        {
            return Client.Processor.ProcessHoistData<T>(
                Client.Delete(
                    EndPoints.AddToEndPoint(_endpoint, proxyEndpoint, encodeKey: false),
                    _proxyToken
                    ));
        }

        public HoistModel PUT(string proxyEndpoint, object model)
        {
            return PUT<HoistModel>(proxyEndpoint, model);
        }

        public T PUT<T>(string proxyEndpoint, object model) where T:class
        {
            return Client.Processor.ProcessHoistData<T>(
                Client.Put(
                    EndPoints.AddToEndPoint(_endpoint, proxyEndpoint, encodeKey: false),
                    model,
                    _proxyToken
                    ));
        }

    }
}
