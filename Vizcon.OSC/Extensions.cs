using System;
using System.Collections.Generic;
using System.Linq;

namespace Vizcon.OSC
{
    internal static class Extensions
    {
        public static int FirstIndexAfter<T>(this IEnumerable<T> items, int start, Func<T, bool> predicate)
        {
            _ = typeof(T);

            if (items != null)
            {
                if (predicate != null)
                {
                }
                else
                    throw new ArgumentNullException(nameof(predicate));
                if (start < items.Count())
                {
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(start));
                int retVal = 0;
                foreach (var item in items)
                {
                    if (retVal >= start && predicate(item)) return retVal;
                    retVal++;
                }
                return -1;
            }

            throw new ArgumentNullException(nameof(items));
        }

        public static List<List<T>> Split<T>(this IEnumerable<T> data, Func<T, bool> predicate)
        {
            var output = new List<List<T>>();
            var curr = new List<T>();
            output.Add(curr);
            foreach (var x in data)
            {
                if (predicate(x))
                {
                    curr = new List<T>();
                    output.Add(curr);
                }
                else
                    curr.Add(x);
            }

            return output;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
