/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MoleSql.Helpers
{
    [SuppressMessage("Microsoft.Performance", "CA1812", Justification="Is created via reflection.")]
    sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public TKey Key { get; }
        readonly IEnumerable<TElement> elements;
        
        internal Grouping(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            this.elements = elements;
        }

        public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
