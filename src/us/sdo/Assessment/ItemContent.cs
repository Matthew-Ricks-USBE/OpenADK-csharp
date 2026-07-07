using System;
using System.Runtime.Serialization;
using OpenADK.Library;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;

namespace OpenADK.Library.us.Assessment {

[Serializable]
public class ItemContent : SifElement
{
	public ItemContent() : base( AssessmentDTD.ASSESSMENTITEM_ITEMCONTENT ) {}
}}
