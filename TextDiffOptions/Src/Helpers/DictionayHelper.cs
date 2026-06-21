
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Comparer.Helper; // UniversalComparer

namespace Dictionary.Helper
{
    /// <summary>
    /// Sortable dictionary: extends Dictionary with a Sort method
    ///  that returns an array of values ordered by a sort expression
    /// </summary>
    [Serializable]
    public class SortableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public SortableDictionary() { }

        protected SortableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Returns all values as an array sorted by <paramref name="sortExpression"/>
        /// </summary>
        public TValue[] Sort(string sortExpression = "")
        {
            int count = Count;
            TValue[] array = new TValue[count];
            int idx = 0;
            foreach (var kvp in this)
                array[idx++] = kvp.Value;

            if (string.IsNullOrEmpty(sortExpression))
                return array;

            var comparer = new UniversalComparer<TValue>(sortExpression);
            Array.Sort(array, comparer);
            return array;
        }
    }
}