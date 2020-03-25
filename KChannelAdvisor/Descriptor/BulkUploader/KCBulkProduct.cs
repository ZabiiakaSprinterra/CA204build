using KChannelAdvisor.Descriptor.API.Entity;
using System;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.BulkUploader
{
    public class KCBulkProduct
    {
        public KCAPIInventoryItem Product;
        public IEnumerable<KCAPIAttribute> Attributes;

        public KCBulkProduct(KCAPIInventoryItem product, IEnumerable<KCAPIAttribute> attributes)
        {
            Product = product;
            Attributes = attributes;
        }

        public bool IsNonEmpty(Type type, object value)
        {
            Dictionary<Type, Func<object, bool>> nonEmptyChecker = new Dictionary<Type, Func<object, bool>> {
                { typeof(decimal?), (object x) => (decimal?)x != default(decimal?) && (decimal?)x != default(decimal)},
                { typeof(double?), (object x) => (double?)x != default(double?) && (double?)x != default(double) },
                { typeof(int?), (object x) => (int?)x != default(int?) && (int?)x != default(int) },
                { typeof(decimal), (object x) => (decimal)x != default(decimal)},
                { typeof(double), (object x) => (double)x != default(double) },
                { typeof(int), (object x) => (int)x != default(int) },
                { typeof(string), (object x) => !string.IsNullOrWhiteSpace(x.ToString()) },
            };

            return nonEmptyChecker.ContainsKey(type) ? nonEmptyChecker[type](value) : true;
        }
    }
}
