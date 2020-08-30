using System.Collections.Generic;
using System.Linq;

namespace Pixel.Extensions
{
    public static class IndexExtension
    {
        public static IEnumerable<(T item, int index)> LoopIndex<T>(IEnumerable<T> self) =>
            self.Select((item, index) => (item, index));
    }
}