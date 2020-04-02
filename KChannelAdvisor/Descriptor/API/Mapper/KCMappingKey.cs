using System;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    public class KCMappingKey : IEquatable<KCMappingKey>
    {
        public object ParentEntityID { get; }
        public string ViewName { get; }

        public KCMappingKey(string viewname, object entityID)
        {
            ViewName = viewname;
            ParentEntityID = entityID;
        }

        public bool Equals(KCMappingKey other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ParentEntityID, other.ParentEntityID) && string.Equals(ViewName, other.ViewName);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KCMappingKey)obj);
        }

        public override int GetHashCode()
        {
            return KCCDHelper.GetMd5Sum(new[] { ParentEntityID.ToString(), ViewName }).GetHashCode();
        }
    }
}
