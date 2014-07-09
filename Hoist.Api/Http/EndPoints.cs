using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Http
{
    public enum eEndPointType
    {
        Login,
        Logout,
        Status,
        SendNotification,
        Data,
        CreateUser,
        ListBuckets,
        CreateBucket,
        SetCurrentBucket,
        GetCurrentBucket,
        UpdateBucket,
        Proxy
    }

    public class EndPoints
    {
        private const string AuthBase = "https://auth.hoi.io/";
        private const string NotificationBase = "https://notify.hoi.io/";
        private const string DataBase = "https://data.hoi.io/";
        private const string ProxyBase = "https://proxy.hoi.io/";
        //public const string CreateUser = AuthBase + "user";
        private const string Login = AuthBase + "login";
        private const string Status = AuthBase + "status";
        private const string Logout = AuthBase + "logout";
        private const string SendNotification = NotificationBase + "notification";
        private const string ListBuckets = AuthBase + "buckets";
        private const string BucketBase = AuthBase + "bucket/";
        private const string CurrentBucket = BucketBase + "current";

        public static string GenerateEndPoint(eEndPointType type, params string[] additionalParameters)
        {
            switch (type)
            {
                case eEndPointType.Login:
                    return Login;
                case eEndPointType.Logout:
                    return Logout;
                case eEndPointType.Status:
                    return Status;
                case eEndPointType.SendNotification:
                    return SendNotification + "/" + String.Join("/", additionalParameters.Select(x => WebUtility.UrlEncode(x)).ToArray());
                case eEndPointType.Data:
                    return DataBase + String.Join("/", additionalParameters.Select(x => WebUtility.UrlEncode(x)).ToArray());
                case eEndPointType.ListBuckets:
                    return ListBuckets;
                case eEndPointType.CreateBucket:
                    return BucketBase + String.Join("/", additionalParameters.Select(x => WebUtility.UrlEncode(x)).ToArray());
                case eEndPointType.SetCurrentBucket:
                    return CurrentBucket + "/" + String.Join("/", additionalParameters.Select(x => WebUtility.UrlEncode(x)).ToArray());
                case eEndPointType.GetCurrentBucket:
                    return CurrentBucket;
                case eEndPointType.UpdateBucket:
                    return BucketBase + String.Join("/", additionalParameters.Select(x => WebUtility.UrlEncode(x)).ToArray()) + "/meta";
                case eEndPointType.Proxy:
                    return ProxyBase + String.Join("/", additionalParameters.Select(x => WebUtility.UrlEncode(x)).ToArray());
                default:
                    return "";
            }
        }

        public static string AddToEndPoint(string endPoint, string key, string[] queryStringParmeters=null, bool encodeKey=true)
        {
            var newUrl = endPoint;
            

            if (encodeKey)
            {
                if (!newUrl.EndsWith("/"))
                {
                    newUrl += "/";
                }
                newUrl += WebUtility.UrlEncode(key);
            }
            else
            {
                if (!newUrl.EndsWith("/") && !key.StartsWith("/"))
                {
                    newUrl += "/";
                }
                newUrl += key;
            }
            
            if (queryStringParmeters!=null) {
                newUrl += "?" + String.Join("&", queryStringParmeters);
            }
            return newUrl;
        }

        public static string AddEnvironmentToUrl(string endPoint, string environment)
        {
            var retval = endPoint;
            if (!String.IsNullOrWhiteSpace(environment))
            {
                if (!endPoint.Contains("?"))
                {
                    retval += "?overrideEnvironment=" + WebUtility.UrlEncode(environment);
                }
                else
                {
                    retval += "&overrideEnvironment=" + WebUtility.UrlEncode(environment);
                }
            }
            return retval;
        }

    }
}
