namespace KChannelAdvisor.Descriptor.API.Mapper.MapCastHandlers
{
    class KCToBoolHandler : KCMapCastHandler
    {
        public override object Handle(object request)
        {
            if (request is KCMapCastDTO dto && dto.Property.PropertyType == typeof(bool?))
            {
                var value = dto.Field as bool?;
                return value.GetValueOrDefault();
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
