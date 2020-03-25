using System;
using System.Security.Cryptography;
using System.Text;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    public class KCCDHelper
    {
        private const string separator = "-";
        public static string GetMd5Sum(string[] str)
        {
            string res = string.Join(string.Empty, str);
            return GetMd5Sum(res);
        }

        // Create an md5 sum string of this string
        public static string GetMd5Sum(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            string result;
            using (MD5 md5 = new MD5Cng())
            {
                result = BitConverter.ToString(md5.ComputeHash(bytes)).Replace(separator, string.Empty);
            }

            return result;
        }
    }
}
