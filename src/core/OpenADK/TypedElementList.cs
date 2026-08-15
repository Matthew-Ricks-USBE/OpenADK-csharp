//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenADK.Library
{
    /// <summary/>
    public class TypedElementList<T> :
        IList<T>
        where T : SifElement
    {
        private IList<T> fList;

        internal TypedElementList( IList<T> items )
        {
            fList = items;
        }

        internal TypedElementList()
        {
            fList = new List<T>();
        }

        /// <summary/>
        public T [] ToArray()
        {
            T [] returnValue = new T[fList.Count];
            fList.CopyTo( returnValue, 0 );
            return returnValue;
        }

        #region IList<T> Members

        /// <summary/>
        public int IndexOf( T item )
        {
            return fList.IndexOf( item );
        }

        void IList<T>.Insert( int index,
                              T item )
        {
            _throwNotSupported();
        }

        void IList<T>.RemoveAt( int index )
        {
            _throwNotSupported();
        }

        /// <summary/>
        public T this[ int index ]
        {
            get { return fList[index]; }
            set { _throwNotSupported(); }
        }

        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add( T item )
        {
            _throwNotSupported();
        }

        void ICollection<T>.Clear()
        {
            _throwNotSupported();
        }

        /// <summary/>
        public bool Contains( T item )
        {
            return fList.Contains( item );
        }

        /// <summary/>
        public void CopyTo( T [] array,
                            int arrayIndex )
        {
            fList.CopyTo( array, arrayIndex );
        }

        /// <summary/>
        public int Count
        {
            get { return fList.Count; }
        }

        /// <summary/>
        public bool IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<T>.Remove( T item )
        {
            _throwNotSupported();
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary/>
        public IEnumerator<T> GetEnumerator()
        {
            return fList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fList.GetEnumerator();
        }

        #endregion

        private void _throwNotSupported()
        {
            throw new NotSupportedException
                ( this.GetType().FullName + " is a read-only list. NO changes are permitted." );
        }
    }
}
