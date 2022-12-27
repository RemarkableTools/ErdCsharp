using System;
using System.Collections.Generic;
using System.Text;

namespace ErdCsharp.Domain.Helper
{
    public static class DataCoder
    {
        public static string DecodeData(string encodedData)
        {
            if (encodedData is null) return null;

            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedData));
        }

        public static string EncodeData(string data)
        {
            if (data is null) return null;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
    }
}
