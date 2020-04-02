namespace KChannelAdvisor.Descriptor.API.Mapper.MapCastHandlers
{
    internal class KCToStringHandler : KCMapCastHandler
    {
        public override object Handle(object request)
        {
            if (request is KCMapCastDTO dto && dto.Property.PropertyType == typeof(string))
            {
                return dto.Field.ToString();
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
