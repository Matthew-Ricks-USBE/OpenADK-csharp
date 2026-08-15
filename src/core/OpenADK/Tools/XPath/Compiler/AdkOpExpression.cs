//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

namespace OpenADK.Library.Tools.XPath.Compiler
{
    /// <summary/>
    public abstract class AdkOpExpression : AdkExpression
    {
        /// <summary/>
        protected AdkExpression[] fArgs;


        /// <summary/>
        protected AdkOpExpression( params AdkExpression[] args )
        {
            fArgs = args;
        }

        /// <summary/>
        public AdkExpression[] Arguments
        {
            get { return fArgs; }
        }

        /// <summary/>
        protected override bool ComputeContextDependent()
        {
            if ( fArgs != null )
            {
                foreach ( AdkExpression expr in fArgs )
                {
                    if ( expr.IsContextDependent() )
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
