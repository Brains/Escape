using System;
using System.Collections.Generic;

namespace Tools
{
    static public class ListExtensions
    {
        //------------------------------------------------------------------
        public static T MinBy <T, C> (this IEnumerable<T> sequence, Func<T, C> keySelector)
        {
            bool first = true;
            T result = default (T);
            C minKey = default (C);
            IComparer<C> comparer = Comparer<C>.Default; //or you can pass this in as a parameter

            foreach (var item in sequence)
            {
                if (first)
                {
                    result = item;
                    minKey = keySelector.Invoke (item);
                    first = false;
                    continue;
                }

                C key = keySelector.Invoke (item);
                if (comparer.Compare (key, minKey) < 0)
                {
                    result = item;
                    minKey = key;
                }
            }

            return result;
        } 
    }
}