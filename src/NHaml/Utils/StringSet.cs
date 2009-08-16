using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace NHaml.Utils
{
    [SuppressMessage( "Microsoft.Naming", "CA1710" )]
    public  class Set<T> : Collection<T>
    {
        public Set()
        {
        }

        public Set( IEnumerable<T> items )
        {
            var i = 0;

            foreach( var item in items )
            {
                base.InsertItem( i++, item );
            }
        }

        protected override void InsertItem( int index, T item )
        {
            if( !Contains( item ) )
            {
                base.InsertItem( index, item );
            }
        }

        protected override void SetItem( int index, T item )
        {
        }
    }
}