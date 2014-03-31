using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Exceptions
{
    public class MissingPropertiesException : Exception
    {
        public List<String> MissingProperties = new List<string>();

        public MissingPropertiesException(params string[] properties)
        {
            MissingProperties.AddRange(properties);
        }

    }
}
