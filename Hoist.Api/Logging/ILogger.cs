using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Logging
{
    public interface ILogger
    {
        void Error(string format, params string[] parameters);
        void Info(string format, params string[] parameters);
        void Debug(string format, params string[] parameters);

    }
}
