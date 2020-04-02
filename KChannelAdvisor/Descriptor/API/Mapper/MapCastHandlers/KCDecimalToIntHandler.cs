namespace KChannelAdvisor.Descriptor.API.Mapper.MapCastHandlers
{
    internal class KCDecimalToIntHandler : KCMapCastHandler
    {
        public override object Handle(object request)
        {
            if (request is KCMapCastDTO dto && dto.Field is decimal && dto.Property.PropertyType == typeof(int?))
            {
                return (int)(decimal)dto.Field;
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
