using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hoist.Api;

namespace Hoist.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Hoist.Api.Logging.LogManager.SetGetLoggerFunc(x => new ConsoleLogger());
            var client = new Hoist.Api.Hoist(args[0]);
            try
            {
                var usr = client.Login(args[1], args[2]);
                System.Console.Out.WriteLine(usr != null ? usr.ToString() : "User Failed to Log In");
                usr =  client.Status();
                System.Console.Out.WriteLine(usr != null ? usr.ToString() : "No User Logged In");

                var votes = client.GetCollection(args[3]);
                foreach (var hd in votes.ToList())
                {
                    System.Console.Out.WriteLine(hd.Get("_id"));
                }

                client.Logout();
                
            }
            catch (Exception ex)
            {
                System.Console.Out.WriteLine("EXCEPTION:\n{0}", ex);
            }

        }
    }
}
