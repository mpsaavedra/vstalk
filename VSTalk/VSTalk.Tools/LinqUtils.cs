using System;
using System.Collections.Generic;
using System.Linq;

namespace VSTalk.Tools
{
    public static class LinqUtils
    {
        public static IEnumerable<TInput> Each<TInput>(this IEnumerable<TInput> items,
            Action<TInput> action)
            where TInput : class
        {
            if (items == null) return null;
            foreach (var input in items)
            {
                action(input);
            }
            return items;
        }

        public static TInput Do<TInput>(this TInput o, Action<TInput> action)
            where TInput : class
        {
            if (o == null) return null;
            action(o);
            return o;
        }

        public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
            where TResult : class
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o);
        }
    }
}
