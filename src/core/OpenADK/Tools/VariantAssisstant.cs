//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Text;
using OpenADK.Library.Impl;
using OpenADK.Util;

namespace OpenADK.Library.Tools
{
    /// <summary/>
    public class VariantAssisstant
    {
        private readonly IDtd fDtd;
        private readonly ISifObjectFactory fObjects;

        /// <summary/>
        public VariantAssisstant(IDtd dtd, ISifObjectFactory objects)
        {
            fDtd = dtd ?? throw new ArgumentNullException(nameof(dtd));
            fObjects = objects ?? throw new ArgumentNullException(nameof(objects));
        }

        /// <summary/>
        public static readonly string README = "THIS CLASS HAS NOT BEEN TESTED.  USE AT YOUR OWN RISK.";

        /// <summary/>
        public Type GetSifElementType(string library, string sdoName)
        {
            string sdoAssembly = fDtd.SDOAssembly;

            string variantString = sdoAssembly.Substring(sdoAssembly.Length - 2).ToLower();

            string className = "OpenADK.Library." + variantString + "." + library + "." + sdoName + ", " + sdoAssembly;

            Type type = Type.GetType(className);

            return type;
        }


        /// <summary/>
        public Type GetSifElementType(string sdoName)
        {
            string sdoAssembly = fDtd.SDOAssembly;

            string variantString = sdoAssembly.Substring(sdoAssembly.Length - 2).ToLower();

            foreach (string library in ((ISifDtd)fDtd).LoadedLibraryNames)
            {
                string className = "OpenADK.Library." + variantString + "." + library + "." + sdoName + ", " + sdoAssembly;

                Type type = Type.GetType(className);

                if (type != null)
                    return type;
            }

            return null;
        }



        /// <summary/>
        public SifElement GetSifElement(string sdoName)
        {
            string sdoAssembly = fDtd.SDOAssembly;

            string variantString = sdoAssembly.Substring(sdoAssembly.Length - 2).ToLower();

            foreach (string library in ((ISifDtd)fDtd).LoadedLibraryNames)
            {
                string className = "OpenADK.Library." + variantString + "." + library + "." + sdoName + ", " + sdoAssembly;

                Type type = Type.GetType(className);

                if (type != null)
                    return fObjects.Create(type);
            }

            return null;
        }


        /// <summary/>
        public SifElement GetSifElement(string library, string sdoName)
        {
            string sdoAssembly = fDtd.SDOAssembly;

            string variantString = sdoAssembly.Substring(sdoAssembly.Length - 2).ToLower();

            string className = "OpenADK.Library." + variantString + "." + library + "." + sdoName + ", " + sdoAssembly;

            Type type = Type.GetType(className);

            if (type != null)
                return fObjects.Create(type);
            else
                return null;
        }
    }
}
