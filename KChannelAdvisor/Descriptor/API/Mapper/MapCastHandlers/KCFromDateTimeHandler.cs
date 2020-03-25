using System;
using System.Globalization;

namespace KChannelAdvisor.Descriptor.API.Mapper.MapCastHandlers
{
    internal class KCFromDateTimeHandler : KCMapCastHandler
    {
        public override object Handle(object request)
        {
            if (request is KCMapCastDTO dto && dto.Field is DateTime)
            {
                return ((DateTime)dto.Field).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
