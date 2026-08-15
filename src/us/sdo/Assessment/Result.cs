using System;
using System.Runtime.Serialization;
using OpenADK.Library;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;

namespace OpenADK.Library.us.Assessment;

/// <summary>
/// Represents the <c>result</c> element of a <c>studentResultSet</c> in the SIF Assessment DTD.
/// </summary>
[Serializable]
public class Result : SifElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    public Result() : base( AssessmentDTD.STUDENTRESULTSET_RESULT ) {}
}
