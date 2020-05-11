using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Honeybee.Core.Extensions
{
    public static class ListExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return new ObservableCollection<T>(source);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer, T value)
        {
            return source.FirstOrDefault(item => comparer.Equals(item, value));
        }
    }
}
