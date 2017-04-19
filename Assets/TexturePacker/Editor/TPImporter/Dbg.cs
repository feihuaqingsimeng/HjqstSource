using System;
using UnityEngine;
namespace TPImporter
{
	public class Dbg
	{
		private static bool DEBUG = false;
		public static void Log(string msg)
		{
			if (Dbg.DEBUG)
			{
				Debug.Log(msg);
			}
		}
	}
}
