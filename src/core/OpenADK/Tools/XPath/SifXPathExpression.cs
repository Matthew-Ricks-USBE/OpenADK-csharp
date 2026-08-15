//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Xml.XPath;

namespace OpenADK.Library.Tools.XPath
{
    /// <summary/>
    public class SifXPathExpression
    {
        private string fExpression;
        private XPathExpression fCompiledExpression;

        /// <summary/>
        public SifXPathExpression( String expression )
        {
            fExpression = expression;
        }

        /// <summary/>
        public INodePointer CreatePath( SifXPathContext context )
        {
            return context.CreatePath( this );
        }

        /// <summary/>
        public object GetValue( SifXPathContext context )
        {
            return context.GetValue( this );
        }

        internal XPathExpression CompiledExpression
        {
            get { return fCompiledExpression; }
            set { fCompiledExpression = value; }
        }

        /// <summary/>
        public String Expression
        {
            get { return fExpression; }
        }
    }
}
