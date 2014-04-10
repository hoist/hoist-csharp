using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Hoist.Api.Model
{
    public class HoistModelJavaScriptConverter : JavaScriptConverter
    {

        public override object Deserialize(IDictionary<string, object> dict, Type type, JavaScriptSerializer serializer)
        {
            if (dict == null)
                throw new ArgumentNullException("dictionary");

            if (type == typeof(HoistModel))
            {
                // Create the instance to deserialize into.
                HoistModel retval = new HoistModel();
                foreach (var key in dict.Keys)
                {
                    if (dict[key].GetType() == dict.GetType())
                    {
                        retval.Set(key, Deserialize(dict[key] as Dictionary<string, object>, type, serializer));
                    }
                    else
                    {
                        retval.Set(key, dict[key].ToString());
                    }
                }
                return retval;                
            }
            return null;

        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var hoistModel = obj as HoistModel;
            if (hoistModel != null)
            {
                var retval = new Dictionary<string, object>();
                foreach (var key in hoistModel.Keys)
                {
                    retval[key] = hoistModel.Get(key);
                }
                return retval;

            }
            return new Dictionary<string, object>();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(HoistModel) }));  }
        }
    }
}
