/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Collections.Generic;

namespace MoleSql.Mapper
{
    abstract class AggregateAsyncReader
    {
        internal abstract object Read(object sequence);
    }

    sealed class AggregateAsyncReader<T, S> : AggregateAsyncReader
    {
        readonly Func<IAsyncEnumerable<T>, S> aggregator;
        
        internal AggregateAsyncReader(Func<IAsyncEnumerable<T>, S> aggregator)
        {
            this.aggregator = aggregator;
        }
        
        internal override object Read(object o)
        {
            if (!(o is IAsyncEnumerable<T> sequence))
                throw new InvalidOperationException($"The pass objet was not an '{typeof(IAsyncEnumerable<T>)}'.");
            return aggregator(sequence);
        }
    }
}
