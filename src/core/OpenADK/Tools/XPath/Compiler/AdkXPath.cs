//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

namespace OpenADK.Library.Tools.XPath.Compiler
{
    /// <summary/>
    public abstract class AdkXPath : AdkExpression
    {
        private AdkXPathStep[] fSteps;

        /// <summary/>
        protected AdkXPath( params AdkXPathStep[] steps )
        {
            fSteps = steps;
        }

        /// <summary/>
        public AdkXPathStep[] Steps
        {
            get { return fSteps; }
        }

        /// <summary/>
        protected override bool ComputeContextDependent()
        {
            if ( fSteps != null )
            {
                foreach ( AdkXPathStep step in fSteps )
                {
                    if ( step.IsContextDependent() )
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
