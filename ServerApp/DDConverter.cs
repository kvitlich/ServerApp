using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ServerApp
{
    public class DDConverter
    {
        
        public string FromObject<T>(List<T> ddObject)
        {
            Type myType = typeof(T);
            StringBuilder result = new StringBuilder();
            result.Append($"[table:{myType.Name}]");
            result.Append("[columns]");
            var properties = myType.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                result.Append("[column]");
                result.Append($"[name:{properties[0].Name}]");
                result.Append($"[data:");
                for (int j = 0; j < ddObject.Count; j++)
                {
                    result.Append(@$"{ddObject[j]}\");
                }
                result.Append($"]");
                result.Append("[/column]");
            }
            return result.ToString();
        }


        public T ToObject<T>(string ddString)
        {
            T result = null;
            return result;
        }

    }
}
