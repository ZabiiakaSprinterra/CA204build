using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCObjectExtensions
    {

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }
}
