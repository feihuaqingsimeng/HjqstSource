using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
namespace Common.Util
{
	public class BlackListWordUtil
	{
		private static readonly Regex regex_b = new Regex(@"\b",RegexOptions.None);
		private static readonly Regex reg_en = new Regex(@"[a-za-z]+",RegexOptions.None);
		private static readonly Regex reg_num = new Regex(@"^[\-\.\s\d]+$",RegexOptions.None);

		private static List<Regex> _regexWordList= new List<Regex>();
		private static List<Regex> GetRegexList()
		{
			if(_regexWordList.Count == 0)
			{
				int defaultLength = 200;
				int num = blockWords.Length/defaultLength + (blockWords.Length%defaultLength == 0 ? 0 : 1);//数组太长了，导致正则表达式太长
				for(int i = 0;i<num;i++)
				{
					string pattern = GetPattern(blockWords,i*defaultLength,defaultLength);
					if(!string.IsNullOrEmpty(pattern))
					{
						Regex rex = new Regex(pattern,RegexOptions.IgnoreCase);
						_regexWordList.Add(rex);
					}

				}
			}
			return _regexWordList;
		}

		private static string[] _blockWords;

		private static string[] blockWords
		{
			get
			{
				if(_blockWords == null)
				{
					_blockWords = GetBlockWords();
				}
				return _blockWords;
			}
			
		}

		/// <summary>
		/// 检查输入内容是否包含脏字
		/// </summary>
		public static bool HasBlockWords(string s)
		{
			for(int i = 0,count = blockWords.Length;i<count;i++)
			{
				if(s.Contains(blockWords[i]))
					return true;
			}
			return false;
//			for(int i = 0,count = GetRegexList().Count;i<count;i++)
//			{
//				if(_regexWordList[i].Match(s).Success)
//					return true;
//			}
            //return false;
		}
		///脏词替换成*号
	
		public static string WordsFilter(string s)
		{
			for(int i = 0,count = blockWords.Length;i<count;i++)
			{
				s = s.Replace(blockWords[i],"***");
			}
//			for(int i = 0,count = GetRegexList().Count;i<count;i++)
//			{
//				if(_regexWordList[i].Match(s).Success)
//					s = _regexWordList[i].Replace(s,"***");
//            }
            return s;
		}

		private static string GetPattern(string[] blockWords,int start,int length)
		{
			StringBuilder patt = new StringBuilder();
			string s;
			string word ;
			for(int i = start,count = blockWords.Length;i<count && i< start+length;i++)
			{
				word = blockWords[i];
				if(word.Length == 0)
					continue;
				if(word.Length == 1)
				{
					patt.AppendFormat("|({0})",word);
				}else if(reg_num.IsMatch(word))
				{
					patt.AppendFormat("|({0})",word);
				}else if(reg_en.IsMatch(word))
				{
					s = regex_b.Replace(word,@"(?:[^a-za-z]{0,3})");
					patt.AppendFormat("|({0})",s);
				}else
				{
					s = regex_b.Replace(word,@"(?:[^\u4e00-\u9fa5]{0,3})");
					patt.AppendFormat("|({0})",s);
				}

			}
			if(patt.Length>0)
			{
				patt.Remove(0,1);
			}
			return patt.ToString();
		}

		private static string[] GetBlockWords()
		{
			string[] s = new string[]{"fuck"};
			TextAsset text = ResMgr.ResMgr.instance.Load<TextAsset>("config/lawlessWordList");
			if(text != null)
			{
				s = text.text.Split(CSVUtil.SYMBOL_LINE,System.StringSplitOptions.RemoveEmptyEntries);
			}
			return s;
		}
	}
}

