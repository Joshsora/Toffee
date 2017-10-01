using System;

namespace Toffee.Util
{
    public static class StringUtil
    {
        private const string Alphanumeric = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Random(int length)
        {
            Random random = new Random();
            string str = "";
            for (int i = 0; i < length; i++)
                str += Alphanumeric[random.Next(Alphanumeric.Length)];
            return str;
        }
    }
}
