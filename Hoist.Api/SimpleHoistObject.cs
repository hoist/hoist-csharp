using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api
{
    public class SimpleHoistObject : IJsonObject
    {
        private Dictionary<string, string> _values;

        public SimpleHoistObject(Dictionary<string, string> values)
        {
            _values = new Dictionary<string, string>();
            foreach (var item in values)
            {
                _values[item.Key] = item.Value;
            }
        }

        public string Get(string key)
        {
            if (!_values.ContainsKey(key))
            {
                return null;
            }
            else
            {
                return _values[key];
            }
        }
    }
}
