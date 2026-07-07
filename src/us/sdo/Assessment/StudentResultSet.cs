using System;
using System.Runtime.Serialization;
using OpenADK.Library;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;

namespace OpenADK.Library.us.Assessment {

[Serializable]
public class StudentResultSet : SifElement
{
	public StudentResultSet() : base( AssessmentDTD.STUDENTRESULTSET ) {}
}}
