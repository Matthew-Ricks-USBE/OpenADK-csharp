//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using OpenADK.Library.Tools.XPath;

namespace OpenADK.Library.Impl.Surrogates
{
    /// <summary/>
    public abstract class SurrogateElementPointer<T> : FauxElementPointer
        where T : Element

    {
        /// <summary/>
        protected T fElement;

        /// <summary/>
        protected SurrogateElementPointer( INodePointer parent, String fauxName )
            : base( parent, fauxName, false )
        {
        }

        /// <summary/>
        protected SurrogateElementPointer( INodePointer parent, String fauxName, T pointedNode, bool isAttribute )
            : base( parent, fauxName, isAttribute )
        {
            fElement = pointedNode;
        }


        /// <summary/>
        protected void setElement( T node )
        {
            fElement = node;
        }

        /// <summary/>
        protected T getElement()
        {
            return fElement;
        }

        /// <summary/>
        public override object GetBaseValue()
        {
            return fElement;
        }

        /// <summary/>
        public override Object Node
        {
            get { return fElement; }
        }
    }
}
