namespace Pixel.Util
{
    public static class StringUtil
    {
        public static string Truncate(this string str, int totalLength, string truncationIndicator = "")
        {
            if (string.IsNullOrEmpty(str) || str.Length < totalLength)
            {
                return str;
            }
            
            return str.Substring(0, totalLength - truncationIndicator.Length) + truncationIndicator;
        }
    }
}