//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;

namespace OpenADK.Library.Tools.XPath.Compiler
{
    /// <summary/>
    public class AdkNodeNameTest : AdkNodeTest
    {
        private String fName;

        /// <summary/>
        public AdkNodeNameTest( String nodeName )
        {
            fName = nodeName;
        }

        /// <summary/>
        public String NodeName
        {
            get { return fName; }
        }

        /// <summary/>
        public override String ToString()
        {
            return fName;
        }
    }
}
