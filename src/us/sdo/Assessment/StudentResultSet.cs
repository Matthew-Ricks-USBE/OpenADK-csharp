using System;
using System.Runtime.Serialization;
using OpenADK.Library;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;

namespace OpenADK.Library.us.Assessment;

/// <summary>
/// Represents the <c>studentResultSet</c> element in the SIF Assessment DTD.
/// </summary>
[Serializable]
public class StudentResultSet : SifElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StudentResultSet"/> class.
    /// </summary>
    public StudentResultSet() : base( AssessmentDTD.STUDENTRESULTSET ) {}
}
