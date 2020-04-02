using System;

namespace KChannelAdvisor.Descriptor.API.Mapper.MapCastHandlers
{
    internal class KCDateTimeOffsetToDateTimeHandler : KCMapCastHandler
    {
        public override object Handle(object request)
        {
            if (request is KCMapCastDTO dto && dto.Field is DateTimeOffset && dto.Property.PropertyType == typeof(DateTime?))
            {
                return ((DateTimeOffset)dto.Field).DateTime;
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
