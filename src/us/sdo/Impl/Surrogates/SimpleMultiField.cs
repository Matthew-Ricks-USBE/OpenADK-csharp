//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;

namespace OpenADK.Library.Impl.Surrogates
{
    /// <summary>
    /// Represents a single field that maps to multiple underlying element definitions which should remain synchronized.
    /// </summary>
    internal sealed class SimpleMultiField : SimpleField
    {
        private readonly IElementDef[] elementDefs;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The SIFElement that is the parent of this field</param>
        /// <param name="defs">The metadata definition of these fields</param>
        /// <exception cref="ArgumentNullException">Thrown if the value passed in is null</exception>
        public SimpleMultiField(SifElement parent, IElementDef[] defs) : base(defs[0], parent)
        {
            elementDefs = defs;
        }

        public override SifSimpleType SifValue
        {
            get => base.SifValue;
            set
            {
                var val = value is SifString ? value : new SifString(value.ToString());
                foreach (var def in elementDefs)
                {
                    ((SifElement)Parent).SetField(def, val);
                }
            }
        }

        public override string TextValue
        {
            get => base.TextValue;
            set
            {
                var val = new SifString(value);
                foreach (var def in elementDefs)
                {
                    ((SifElement)Parent).SetField(def, val);
                }
            }
        }
    }
}
