namespace KChannelAdvisor.Descriptor.API.Mapper.MapCastHandlers
{
    internal class KCBasicHandler : KCMapCastHandler
    {
        public override object Handle(object request)
        {
            if (request is KCMapCastDTO dto)
            {
                return dto.Field;
            }
            else
            {
                return base.Handle(request);
            }
        }
    }
}
