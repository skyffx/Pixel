using System.Collections.Generic;

namespace Pixel.Common
{
    public static class Dictionary
    {
        private static Dictionary<int, bool> _map = new Dictionary<int, bool>();
        
        public static void Put(int key, bool value)
        {
            _map[key] = value;
        }

        private static bool Get(int key, bool defaultValue)
        {
            try
            {
                return _map[key];
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
        }

        public static bool Get(int key)
        {
            return Get(key, true);
        }

        public static void Clear()
        {
            _map.Clear();
        }
    }
}