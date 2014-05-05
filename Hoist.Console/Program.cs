using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Hoist.Api;

namespace Hoist.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger(1);

            Hoist.Api.Logging.LogManager.SetGetLoggerFunc(x => logger);
            var client = new Hoist.Api.HoistClient(args[0]);
            var col = client.GetCollection(args[1]);


            foreach (var obj in col.ToList())
            {
                logger.Info(obj.ToString());
                
            }

            col.Insert(new Hoist.Api.Model.HoistModel( new Dictionary<string, string>() { { "cheese","Shop" } } ));

            

            //try
            //{
            //    var usr = client.Login(args[1], args[2]);
            //    System.Console.Out.WriteLine(usr != null ? usr.ToString() : "User Failed to Log In");
            //    usr =  client.Status();
            //    System.Console.Out.WriteLine(usr != null ? usr.ToString() : "No User Logged In");

            //    var votes = client.GetCollection(args[3]);
            //    foreach (var hd in votes.ToList())
            //    {
            //        System.Console.Out.WriteLine(hd.Get("_id"));
            //    }

            //    client.Logout();
                
            //}
            //catch (Exception ex)
            //{
            //    System.Console.Out.WriteLine("EXCEPTION:\n{0}", ex);
            //}

            //StreamReader sr = new StreamReader(args[0]);
            /*
            var usr = client.Login(args[1], args[2]);

            //client.CreateBucket(args[3], new Api.Model.HoistModel(new Dictionary<string, string>() { {"Hello","World"} })); 
            
            
            var buckets = client.ListBuckets();

            foreach (var b in buckets) {
                logger.Debug("{0} META: {1}", b.key, "[" + String.Join(",",b.meta.Select(x=>x.ToString()).ToArray()) + "]"); 
            }

            client.EnterBucket(args[3]);

            var accountMappings = client.GetCollection("AccountMapping");

            accountMappings.Insert( new Api.Model.HoistModel(new Dictionary<string, string>() { {"Hello","World"} })); 
             */

        }
    }
}
