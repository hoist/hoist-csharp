using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Exceptions
{
    public class NoResponseException : Exception
    {
        public NoResponseException(WebException ex) : base("Unhandled WebException", ex)
        {
            
        }
    }
}
