using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldGen.Generator.Data
{
    /// <summary>
    /// A class containing useful extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Pick a random element from a set.
        /// </summary>
        /// <typeparam name="T">The type contained within the set.</typeparam>
        /// <param name="rnd">The random class.</param>
        /// <param name="set">The set of items to choose from.</param>
        /// <returns>A random element from <paramref name="set"/>, as chosen by <paramref name="rnd"/>.</returns>
        public static T PickRandom<T>(this Random rnd, IEnumerable<T> set)
        {
            var array = set.ToArray();

            var r = rnd.Next(array.Length);

            return array[r];
        }

        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// From: http://stackoverflow.com/questions/1577822/passing-a-single-item-as-ienumerablet
        /// </summary>
        /// <typeparam name="T"> Type of the wrapped object.</typeparam>
        /// <param name="item"> The object to wrap.</param>
        /// <returns>
        /// An IEnumerable&lt;T&gt; consisting of a single item.
        /// </returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
