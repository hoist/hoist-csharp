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
        CreateUser
    }

    public class EndPoints
    {
        private const string AuthBase = "https://auth.hoi.io/";
        private const string NotificationBase = "https://notify.hoi.io/";
        public const string DataBase = "https://data.hoi.io/";
        //public const string CreateUser = AuthBase + "user";
        private const string Login = AuthBase + "login";
        private const string Status = AuthBase + "status";
        private const string Logout = AuthBase + "logout";
        private const string SendNotification = NotificationBase + "notification";

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
                default:
                    return "";
            }
        }

        public static string AddToEndPoint(string endPoint, string key, string[] queryStringParmeters=null)
        {
            var newUrl = endPoint;
            if (!newUrl.EndsWith("/"))
            {
                newUrl += "/";
            }
            newUrl += WebUtility.UrlEncode(key);
            if (queryStringParmeters!=null) {
                newUrl += "?" + String.Join("&", queryStringParmeters);
            }
            return newUrl;
        }

    }
}
