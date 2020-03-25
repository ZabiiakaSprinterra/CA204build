namespace KChannelAdvisor.Descriptor.API.Entity
{
    public class KCErrorResponse
    {
        public KCError Error { get; set; }

        public class KCError
        {
            public string Code { get; set; }
            public string Message { get; set; }
        }
    }
}
