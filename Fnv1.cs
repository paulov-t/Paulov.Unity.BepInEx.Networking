using System;
using System.Text;

namespace Paulov.UnityBepInExNetworking
{
    public static class Fnv1a
    {
        public static int Hash(byte[] data)
        {
            if (data.Length == 0)
            {
                return 5381;
            }
            uint hash = 5381u;
            for (int i = 0; i < data.Length; i++)
            {
                hash = (hash * 33) ^ data[i];
            }
            return (int)hash;
        }

        public static int HashString(string span)
        {
            if (span == null)
                throw new ArgumentNullException("span");

            return Hash(Encoding.UTF8.GetBytes(span));
        }
    }
}
