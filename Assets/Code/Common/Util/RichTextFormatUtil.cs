using UnityEngine;
using System.Collections.Generic;

namespace Common.Util
{
	public static class RichTextFormatUtil
	{
		public static List<int> GetRTFStringRealCharIndexList (string inputString)
		{
			List<int> realCharIndexList = new List<int>();
			int i = 0;
			string remainString = inputString;
			while(i < inputString.Length)
			{
				remainString = inputString.Substring(i);
//				Debugger.Log(remainString);
				if (remainString.StartsWith("<b>"))
				{
					i += 3;
				}
				else if (remainString.StartsWith("<i>"))
				{
					i += 3;
				}
				else if (remainString.StartsWith("<size"))
				{
					i += inputString.Substring(i).IndexOf(">") + 1;
				}
				else if (remainString.StartsWith("<color"))
				{
					i += inputString.Substring(i).IndexOf(">") + 1;
				}
				else if (remainString.StartsWith("<material"))
				{
					i += inputString.Substring(i).IndexOf(">") + 1;
				}
				else if (remainString.StartsWith("<quad"))
				{
					i += inputString.Substring(i).IndexOf(">") + 1;
				}
				else if (remainString.StartsWith("</b>"))
				{
					i += 4;
				}
				else if (remainString.StartsWith("</i>"))
				{
					i += 4;
				}
				else if (remainString.StartsWith("</size>"))
				{
					i += 7;
				}
				else if (remainString.StartsWith("</color>"))
				{
					i += 8;
				}
				else if (remainString.StartsWith("</material>"))
				{
					i += 11;
				}
				else
				{
//					Debugger.Log(inputString[i].ToString());
					realCharIndexList.Add(i);
					i++;
				}
			}
			return realCharIndexList;
		}

		private static List<string> GetMismatchedRTFTagList (string inputString)
		{
			List<string> tagList = new List<string>();
			int i = 0;
			string remainString = inputString;
			while(i < inputString.Length)
			{
				remainString = inputString.Substring(i);
				if (remainString.StartsWith("<b>"))
				{
					i += 3;
					tagList.Add("<b>");
				}
				else if (remainString.StartsWith("<i>"))
				{
					i += 3;
					tagList.Add("<i>");
				}
				else if (remainString.StartsWith("<size"))
				{
					i += inputString.Substring(i).IndexOf(">") + 1;
					tagList.Add("<size>");
				}
				else if (remainString.StartsWith("<color"))
				{
					i += inputString.Substring(i).IndexOf(">") + 1;
					tagList.Add("<color>");
				}
				else if (remainString.StartsWith("<material"))
				{
					i += inputString.Substring(i).IndexOf(">") + 1;
					tagList.Add("<material>");
				}
				else if (remainString.StartsWith("<quad"))
				{
					i += inputString.Substring(i).IndexOf(">") + 1;
//					tagList.Add("");
				}
				else if (remainString.StartsWith("</b>"))
				{
					i += 4;
					tagList.Remove("<b>");
				}
				else if (remainString.StartsWith("</i>"))
				{
					i += 4;
					tagList.Remove("<i>");
				}
				else if (remainString.StartsWith("</size>"))
				{
					i += 7;
					tagList.Remove("<size>");
				}
				else if (remainString.StartsWith("</color>"))
				{
					i += 8;
					tagList.Remove("<color>");
				}
				else if (remainString.StartsWith("</material>"))
				{
					i += 11;
					tagList.Remove("<material>");
				}
				else
				{
					i++;
				}
			}
			return tagList;
		}

		public static string GetRTFSubString (string inputString, int endIndex)
		{
			string subString = inputString.Substring(0, endIndex + 1);
			List<string> mismatchedRTFTagList = GetMismatchedRTFTagList(subString);
			mismatchedRTFTagList.Reverse();
			for (int i = 0, mismatchedRTFTagCount = mismatchedRTFTagList.Count; i < mismatchedRTFTagCount; i++)
			{
				if (mismatchedRTFTagList[i] == "<b>")
				{
					subString += "</b>";
				}
				else if (mismatchedRTFTagList[i] == "<i>")
				{
					subString += "</i>";
				}
				else if (mismatchedRTFTagList[i] == "<size>")
				{
					subString += "</size>";
				}
				else if (mismatchedRTFTagList[i] == "<color>")
				{
					subString += "</color>";
				}
				else if (mismatchedRTFTagList[i] == "<material>")
				{
					subString += "</material>";
				}
			}
			return subString;
		}
	}
}