namespace KChannelAdvisor.Descriptor.API.LoggerProvider
{
    public interface KCILoggerProvider
    {
        void ClearLoggingIds();

        void SetNonChildEntityId(string id);

        void SetParentAndEntityIds(string parentId, string childId);

        void SetRequestId(int id);

        void Error(string msg);

        void Information(string msg);
    }
}
