using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Logging
{
    public class LogManager
    {
        private static Func<Type, ILogger> Create = null;


        public static ILogger GetLogger(Type myType)
        {
            if (Create == null) {
                return new NullLogger();
            }
            return Create(myType);
        }

        public static void SetGetLoggerFunc(Func<Type, ILogger> getFunction)
        {
            Create = getFunction;
        }
    }
}
