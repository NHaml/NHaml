using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace NHaml.Utils
{
    [SuppressMessage( "Microsoft.Naming", "CA1710" )]
    public sealed class StringSet : Collection<string>
    {
        public StringSet()
        {
        }

        public StringSet( IEnumerable<string> items )
        {
            var i = 0;

            foreach( var item in items )
            {
                base.InsertItem( i++, item );
            }
        }

        protected override void InsertItem( int index, string item )
        {
            if( !Contains( item ) )
            {
                base.InsertItem( index, item );
            }
        }

        protected override void SetItem( int index, string item )
        {
        }
    }
}