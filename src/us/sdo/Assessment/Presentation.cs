using System;
using System.Runtime.Serialization;
using OpenADK.Library;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;

namespace OpenADK.Library.us.Assessment;

/// <summary>
/// Represents the <c>presentation</c> element of an <c>assessmentItem</c> in the SIF Assessment DTD.
/// </summary>
[Serializable]
public class Presentation : SifElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Presentation"/> class.
    /// </summary>
	public Presentation() : base( AssessmentDTD.ASSESSMENTITEM_PRESENTATION ) {}
}
