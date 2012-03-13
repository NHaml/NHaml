using System;
using System.Collections.Generic;
using System.Configuration;

namespace NHaml4.Configuration
{
    public class ConfigurationCollection<T> : ConfigurationElementCollection, IEnumerable<T>
      where T : KeyedConfigurationElement, new()
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override Object GetElementKey( ConfigurationElement element )
        {
            return ((T)element).Key;
        }

        public T this[int index]
        {
            get { return (T)BaseGet( index ); }
            set
            {
                if( BaseGet( index ) != null )
                {
                    BaseRemoveAt( index );
                }

                BaseAdd( index, value );
            }
        }

        public new T this[string name]
        {
            get { return (T)BaseGet( name ); }
        }

        public new IEnumerator<T> GetEnumerator()
        {
            var enumerator = base.GetEnumerator();

            while( enumerator.MoveNext() )
            {
                yield return (T)enumerator.Current;
            }
        }

        public int IndexOf( T element )
        {
            return BaseIndexOf( element );
        }

        public void Add( T element )
        {
            BaseAdd( element );
        }

        protected override void BaseAdd( ConfigurationElement element )
        {
            BaseAdd( element, false );
        }

        public void Remove( T element )
        {
            if( BaseIndexOf( element ) >= 0 )
            {
                BaseRemove( element.Key );
            }
        }

        public void RemoveAt( int index )
        {
            BaseRemoveAt( index );
        }

        public void Remove( string name )
        {
            BaseRemove( name );
        }

        public void Clear()
        {
            BaseClear();
        }
    }
}