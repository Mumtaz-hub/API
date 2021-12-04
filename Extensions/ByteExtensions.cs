using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Extensions
{
    public static class ByteExtensions
    {
        public static string GetString(this byte[] byteArray)
        {
            return Encoding.UTF8.GetString(byteArray);
        }

        public static Stream ToStream(this byte[] byteArray)
        {
            return new MemoryStream(byteArray);
        }

        public static bool IsEqual(this byte[] sourceBytes, byte[] destinationBytes)
        {
            return sourceBytes.SequenceEqual(destinationBytes);
        }
    }
}
