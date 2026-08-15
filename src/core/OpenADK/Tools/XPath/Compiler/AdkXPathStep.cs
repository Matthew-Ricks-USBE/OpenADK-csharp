//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Text;

namespace OpenADK.Library.Tools.XPath.Compiler
{
    /// <summary>
    /// Represents a single step in an XPath expression
    /// </summary>
    public class AdkXPathStep
    {
        private AdkNodeTest fNodeTest;
        private AdkAxisType fAxis;
        private AdkExpression[] fPredicates;


        /// <summary/>
        public AdkXPathStep( AdkAxisType axis, AdkNodeTest nodeTest, params AdkExpression[] predicates )
        {
            fAxis = axis;
            fNodeTest = nodeTest;
            fPredicates = predicates;
        }

        /// <summary/>
        public AdkNodeTest NodeTest
        {
            get { return fNodeTest; }
        }

        /// <summary/>
        public AdkExpression[] Predicates
        {
            get { return fPredicates; }
        }

        /// <summary/>
        public bool IsContextDependent()
        {
            if ( fPredicates != null )
            {
                foreach ( AdkExpression expr in fPredicates )
                {
                    if ( expr.IsContextDependent() )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary/>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            switch ( fAxis )
            {
                case AdkAxisType.Child:
                    buffer.Append( fNodeTest );
                    break;
                case AdkAxisType.Attribute:
                    buffer.Append( '@' );
                    buffer.Append( fNodeTest );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            AdkExpression[] predicates = Predicates;
            if ( predicates != null )
            {
                for ( int i = 0; i < predicates.Length; i++ )
                {
                    buffer.Append( '[' );
                    buffer.Append( predicates[i] );
                    buffer.Append( ']' );
                }
            }
            return buffer.ToString();
        }
    }
}
