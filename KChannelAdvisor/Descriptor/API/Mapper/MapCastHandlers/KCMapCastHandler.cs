namespace KChannelAdvisor.Descriptor.API.Mapper.MapCastHandlers
{
    internal abstract class KCMapCastHandler : KCIHandler
    {
        private KCIHandler _nextHandler;

        public KCIHandler SetNext(KCIHandler handler)
        {
            _nextHandler = handler;

            return handler;
        }

        public virtual object Handle(object request)
        {
            if (_nextHandler != null)
            {
                return _nextHandler.Handle(request);
            }
            else
            {
                return null;
            }
        }
    }
}
