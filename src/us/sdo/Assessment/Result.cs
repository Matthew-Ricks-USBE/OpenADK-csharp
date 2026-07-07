using System;
using System.Runtime.Serialization;
using OpenADK.Library;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;

namespace OpenADK.Library.us.Assessment {

[Serializable]
public class Result : SifElement
{
	public Result() : base( AssessmentDTD.STUDENTRESULTSET_RESULT ) {}
}}
