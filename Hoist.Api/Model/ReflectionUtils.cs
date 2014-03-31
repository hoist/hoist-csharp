using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Model
{
    public static class  ReflectionUtils
    {
        public static List<string> ExtractProperties<CollectionType>(CollectionType modelToUpdate, ref string id)
        {
            var ConversionType = typeof(CollectionType);
            List<string> publicProperties;
            //have to reduce Model to dictionary of parameters to check for required parmeters                
            publicProperties = ConversionType.GetFields().Select(x => x.Name).ToList();
            publicProperties.AddRange( ConversionType.GetProperties().Select(x => x.Name).ToList());
            
            var idAttr = ConversionType.GetMembers().Where(x => x.Name == "_id").FirstOrDefault();
            if (idAttr != null)
            {
                object tempId = null;
                if (idAttr.MemberType == MemberTypes.Property)
                {
                    tempId = ConversionType.GetProperty("_id").GetValue(modelToUpdate);
                }
                else if (idAttr.MemberType == MemberTypes.Field)
                {
                    tempId = ConversionType.GetField("_id").GetValue(modelToUpdate);
                }
                id = tempId != null ? tempId.ToString() : "";
            }
            return publicProperties.ToList();
        }
    }
}
