using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hoist.Api.Http;

namespace Hoist.Api.Exceptions
{
    public class UnexpectedResponseException : Exception
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public bool WithWWWAuthenticate { get; set; }
        public string RawPayload { get; set; }

        public UnexpectedResponseException(ApiResponse response)
        {
            Code = response.Code;
            Description = response.Description;
            WithWWWAuthenticate = response.WithWWWAuthenticate;
            //RawPayload = 
        }
    }
}
