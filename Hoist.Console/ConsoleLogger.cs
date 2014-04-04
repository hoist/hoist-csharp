using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Console
{
    class ConsoleLogger : Hoist.Api.Logging.ILogger
    {

        public void Debug(string format, params string[] parameters)
        {
            System.Console.WriteLine("DEBUG: "+format,  parameters);
        }

        public void Error(string format, params string[] parameters)
        {
            System.Console.WriteLine("ERROR: " + format, parameters);
        }

        public void Info(string format, params string[] parameters)
        {
            System.Console.WriteLine("INFO: " + format, parameters);
        }
    }
}
