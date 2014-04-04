using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Console
{
    class ConsoleLogger : Hoist.Api.Logging.ILogger
    {
        int _tabs;
        public ConsoleLogger(int tabs)
        {
            _tabs = tabs;
        }

        private void Out(string msg)
        {
            System.Console.Write(String.Concat(Enumerable.Repeat("\t", _tabs)));
            System.Console.WriteLine(msg);
        }

        public void Debug(string format, params string[] parameters)
        {
           Out(String.Format("DEBUG: "+format,  parameters));
        }

        public void Error(string format, params string[] parameters)
        {
            Out(String.Format("ERROR: " + format, parameters));
        }

        public void Info(string format, params string[] parameters)
        {
            Out(String.Format("INFO: " + format, parameters));
        }
    }
}
