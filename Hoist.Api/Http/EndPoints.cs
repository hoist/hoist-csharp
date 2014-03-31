using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Http
{
    public enum eEndPointType
    {
        Auth,
        Data,
        Notify,
        File
    }

    public class EndPoints
    {
        public const string AuthBase = "https://auth.hoi.io/";
        public const string NotificationBase = "https://notify.hoi.io/";
        public const string DataBase = "https://data.hoi.io/";
        public const string CreateUser = AuthBase + "user";
        public const string Login = AuthBase + "login";
        public const string Status = AuthBase + "status";
        public const string Logout = AuthBase + "logout";
        public const string SendNotification = NotificationBase + "notification";



    }
}
