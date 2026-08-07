//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Text;

namespace OpenADK.Library.Impl
{
    class Sif1xElementSorter<T> : ElementSorter<T> where T: Element
    {
        public Sif1xElementSorter( SifVersion version )
            : base( version )
        {
           
        }


        /**
         *  Determines whether Element <i>o1</i>comes before or after Element <i>o2</i>
         *  given the ElementDef sequence number of the two objects.
         */
        public override int Compare( T o1, T o2 )
        {
            Element parent1 = o1.Parent;
            Element parent2 = o2.Parent;
            if (parent1 == parent2 || parent1 == null || parent2 == null)
            {
                // When both elements share the same parent, handle fields that "leaked"
                // out of a collapsed container (e.g. ItemTitle stored in Transaction.fFields
                // but defined under ItemInfo). Their ElementDef.Parent differs from the
                // actual parent's ElementDef, so we compute effective sequence at the
                // shared parent level.
                if (parent1 != null)
                {
                    bool o1Leaked = o1.ElementDef.Parent != null &&
                                    o1.ElementDef.Parent != parent1.ElementDef;
                    bool o2Leaked = o2.ElementDef.Parent != null &&
                                    o2.ElementDef.Parent != parent2.ElementDef;

                    if (o1Leaked || o2Leaked)
                    {
                        // Both from same collapsed container → compare their inner seqs
                        if (o1Leaked && o2Leaked &&
                            o1.ElementDef.Parent.Name == o2.ElementDef.Parent.Name)
                        {
                            return compareSequences(o1.ElementDef.GetSequence(fVersion),
                                                    o2.ElementDef.GetSequence(fVersion));
                        }
                        int eff1 = GetEffectiveSeq(o1, parent1.ElementDef);
                        int eff2 = GetEffectiveSeq(o2, parent2.ElementDef);
                        if (eff1 != eff2) return compareSequences(eff1, eff2);
                        // Tied at container level: compare inner seqs if both leaked
                        if (o1Leaked && o2Leaked)
                            return compareSequences(o1.ElementDef.GetSequence(fVersion),
                                                    o2.ElementDef.GetSequence(fVersion));
                    }
                }
                return base.Compare(o1, o2);
            }

            // One of these elements has a parent that was collapsed and it is now
            // being compared with it's uncles and aunts, rather than its siblings
            // The logic is simple: Determine which element is the niece or nephew. That
            // element will use it's parent sequence to compare with the relative.
            if (parent1.Parent == parent2)
            {
                // o2 leaked from the same collapsed container as parent1
                if (o2.ElementDef.Parent != null &&
                    o2.ElementDef.Parent.Name == parent1.ElementDef.Name)
                {
                    return compareSequences(o1.ElementDef.GetSequence(fVersion),
                                           o2.ElementDef.GetSequence(fVersion));
                }
                int cmp1 = parent1.ElementDef.GetSequence(fVersion);
                int cmp2 = o2.ElementDef.GetSequence(fVersion);
                return compareSequences(cmp1, cmp2);
            }
            else if (parent2.Parent == parent1)
            {
                // o1 leaked from the same collapsed container as parent2
                if (o1.ElementDef.Parent != null &&
                    o1.ElementDef.Parent.Name == parent2.ElementDef.Name)
                {
                    return compareSequences(o1.ElementDef.GetSequence(fVersion),
                                           o2.ElementDef.GetSequence(fVersion));
                }
                int cmp1 = o1.ElementDef.GetSequence(fVersion);
                int cmp2 = parent2.ElementDef.GetSequence(fVersion);
                return compareSequences(cmp1, cmp2);
            }
            else if (parent1.Parent == parent2.Parent)
            {
                int cmp1 = parent1.ElementDef.GetSequence(fVersion);
                int cmp2 = parent2.ElementDef.GetSequence(fVersion);
                return compareSequences(cmp1, cmp2);
            }
            // Indeterminate. Do the safe thing and exit gracefully
            return base.Compare(o1, o2);
        }

        private int GetEffectiveSeq(Element e, IElementDef parentDef)
        {
            if (e.ElementDef.Parent != null && e.ElementDef.Parent != parentDef)
            {
                String containerTag = e.ElementDef.Parent.Tag(fVersion);
                IElementDef container = e.ElementDef.Dtd.LookupElementDef(parentDef, containerTag);
                if (container != null) return container.GetSequence(fVersion);
            }
            return e.ElementDef.GetSequence(fVersion);
        }

    }
}
