namespace KChannelAdvisor.Descriptor.Extensions
{
    public static class KCStringExtensions
    {
        public static bool Contains(this string str, char[] separators)
        {
            return str.IndexOfAny(separators) != -1;
        }
    }
}
