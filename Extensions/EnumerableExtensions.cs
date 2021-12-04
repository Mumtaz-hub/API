using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToUnion<T>(this IEnumerable<T> items, T newElement) => items.Union(newElement.ToUnarySequence());

        public static IEnumerable<T> ToUnarySequence<T>(this T item)
        {
            yield return item;
        }

        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> sequence, T exception)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            return default(T) == null ?
                sequence.Except(x => x == null) : sequence.Except(x => exception.Equals(x));
        }


        public static IEnumerable<T> Except<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return sequence.Where(x => !predicate.Invoke(x));
        }


        public static IEnumerable<T> Except<T>(this IEnumerable<T> sequence, Func<T, int, bool> predicate)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return sequence.Where((x, i) => !predicate.Invoke(x, i));
        }

        /// <summary>
        /// Returns all elements in a sequence that precede the first occurence of an element that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> BeforeFirst<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return sequence.TakeWhile(x => !predicate.Invoke(x));
        }

        /// <summary>
        /// Returns all elements in a sequence that follow the first occurence of an element that satisfies a specified condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> AfterFirst<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return sequence.SkipWhile(x => !predicate.Invoke(x)).Skip(1);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (!enumerable.NotNull())
                return;
            foreach (var item in enumerable) action(item);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// 
        /// <returns>
        /// false if the enumerable sequence contains any elements; otherwise, true.
        /// </returns>
        public static bool None<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// 
        /// <returns>
        /// false if any elements in the enumerable sequence pass the test in the specified predicate; otherwise, true.
        /// </returns>
        public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return enumerable == null || !enumerable.Any(predicate);
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.None();
        }

        public static bool HasValues<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.IsNullOrEmpty();
        }

        public static bool ContainsAll<T>(this IEnumerable<T> collection, IEnumerable<T> otherCollection)
        {
            return collection != null && collection.ContainsAll(otherCollection.ToArray());
        }

        public static bool ContainsAll<T>(this IEnumerable<T> collection, params T[] otherCollection)
        {
            return collection != null && otherCollection.All(collection.Contains);
        }

        public static bool ContainsNone<T>(this IEnumerable<T> collection, IEnumerable<T> otherCollection)
        {
            return otherCollection == null || collection.ContainsNone(otherCollection.ToArray());
        }

        public static bool ContainsNone<T>(this IEnumerable<T> collection, params T[] otherCollection)
        {
            return collection != null && otherCollection.None(collection.Contains);
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> collection)
        {
            return collection.EmptyIfNull().GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key);
        }

        public static IEnumerable<IGrouping<TKey, T>> GetDuplicates<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector)
        {
            return collection.EmptyIfNull().GroupBy(keySelector).Where(x => x.Count() > 1);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}