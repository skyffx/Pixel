using System.Collections.Generic;
using System.IO;
using Pixel.TinyJson;

namespace Pixel.Common
{
    public static class Favorites
    {
        private static string _json;
        private static List<string> _favorites = new List<string>();
        
        private static void ReadJson()
        {
            if (File.Exists("favorites.json"))
            {
                _json = File.ReadAllText("favorites.json");
                _favorites = _json.FromJson<List<string>>();
            }
        }

        private static void WriteJson()
        {
            _json = _favorites.ToJson();
            File.WriteAllText("favorites.json", _json);
        }
        
        public static int CountFavorites()
        {
            ReadJson();
            return _favorites.Count;
        }

        public static List<string> ReadFavorites()
        {
            return _favorites;
        }

        public static void WriteFavorites(List<string> favorites)
        {
            _favorites = favorites;
            WriteJson();
        }

        public static void AddFavorite(string id)
        {
            _favorites.Add(id);
            WriteJson();
        }

        public static void RemoveFavorite(string id)
        {
            _favorites.Remove(id);
            WriteJson();
        }
    }
}