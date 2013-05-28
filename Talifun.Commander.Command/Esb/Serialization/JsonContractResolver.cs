using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Talifun.Commander.Command.Esb.Serialization
{
    public class JsonContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            var dictionaryContract = base.CreateDictionaryContract(objectType);
            dictionaryContract.PropertyNameResolver = x => x;
            return dictionaryContract;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (!property.Writable)
            {
                var propertyInfo = member as PropertyInfo;
                if (propertyInfo != null && propertyInfo.GetSetMethod(true) != null)
                    property.Writable = true;
            }
            return property;
        }
    }
}
