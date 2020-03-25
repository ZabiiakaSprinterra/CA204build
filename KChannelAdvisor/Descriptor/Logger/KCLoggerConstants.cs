namespace KChannelAdvisor.Descriptor.Logger
{
    public class KCLoggerConstants
    {
        #region Properties
        public const string RequestId = "RequestID";
        public const string EntityType = "EntityType";
        public const string ActionName = "ActionName";
        public const string EntityId = "EntityId";
        public const string ParentEntityId = "ParentEntityId";
        public const string CreatedDateTime = "CreatedDateTime";
        #endregion

        #region Actions
        public const string Import = "Import";
        public const string Export = "Export";
        public const string Retrieval = "Retrieval";
        #endregion

        #region Values
        public const int DescriptionLength = 1000;
        #endregion
    }
}
