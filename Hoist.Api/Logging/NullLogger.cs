using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Logging
{
    public class NullLogger : ILogger
    {
        public void Error(string format, params string[] parameters) { }

        public void Info(string format, params string[] parameters) { }

        public void Debug(string format, params string[] parameters) { }
    }
}
