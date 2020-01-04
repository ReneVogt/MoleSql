/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * Original source code taken from Matt Warren (https://github.com/mattwar/iqtoolkit).
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace MoleSql.Mapper
{
    abstract class AggregateReader
    {
        internal abstract object Read(IEnumerable sequence);
    }

    sealed class AggregateReader<T, S> : AggregateReader
    {
        readonly Func<IEnumerable<T>, S> aggregator;
        
        internal AggregateReader(Func<IEnumerable<T>, S> aggregator)
        {
            this.aggregator = aggregator;
        }
        
        internal override object Read(IEnumerable sequence)
        {
            return aggregator((IEnumerable<T>)sequence);
        }
    }
}
