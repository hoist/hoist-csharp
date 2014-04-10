using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Model
{
    public class HoistModel 
    {
        private Dictionary<string, object> _values;

        public HoistModel()
        {
            _values = new Dictionary<string, object>();
        }

        public HoistModel(Dictionary<string, string> values) : this()
        {            
            foreach (var item in values)
            {
                _values[item.Key] = item.Value;
            }
        }

        public HoistModel(Dictionary<string, object> values)
            : this()
        {
            foreach (var item in values)
            {
                _values[item.Key] = item.Value;
            }
        }

        public List<string> Keys 
        {
            get { return _values.Keys.ToList<string>(); }
        }

        public object Get(string key)
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

        public void Set(string key, object value)
        {
            _values[key] = value;
        }
    }
}
