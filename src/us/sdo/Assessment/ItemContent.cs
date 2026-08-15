using System;
using System.Runtime.Serialization;
using OpenADK.Library;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;

namespace OpenADK.Library.us.Assessment;

/// <summary>
/// Represents the <c>itemContent</c> element of an <c>assessmentItem</c> in the SIF Assessment DTD.
/// </summary>
[Serializable]
public class ItemContent : SifElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemContent"/> class.
    /// </summary>
	public ItemContent() : base( AssessmentDTD.ASSESSMENTITEM_ITEMCONTENT ) {}
}
