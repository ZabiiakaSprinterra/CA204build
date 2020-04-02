namespace KChannelAdvisor.Descriptor.API.Mapper
{
    internal interface KCIHandler
    {
        KCIHandler SetNext(KCIHandler handler);

        object Handle(object request);
    }
}
