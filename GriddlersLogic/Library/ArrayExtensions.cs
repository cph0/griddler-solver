using System.Collections.Generic;

namespace Griddlers.Library
{
    public static class ArrayExtensions
    {
        public static IEnumerable<T> Skip<T>(this T[] arr, int count)
        {
            for (int Pos = count; Pos < arr.Length; Pos++)
                yield return arr[Pos];
        }

        public static IEnumerable<T> Reverse<T>(this T[] arr, int? start = null)
        {
            int Start = start.GetValueOrDefault(arr.Length - 1);
            for (int Pos = Start; Pos >= 0; Pos--)
                yield return arr[Pos];
        }
    }
}
