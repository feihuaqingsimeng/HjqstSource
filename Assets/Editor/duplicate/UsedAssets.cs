using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class UsedAssets {
	public static string[] GetAllAssets()
	{
		string[] tempAssets1 = Directory.GetFiles(Application.dataPath,"*.*",SearchOption.AllDirectories);
		string[] tempAssets2 = Array.FindAll(tempAssets1,name=>!name.EndsWith(".meta"));
		string[] allAssets = Array.FindAll(tempAssets2,name=>!name.EndsWith(".unity"));

		for(int i = 0 ;i<allAssets.Length;i++)
		{
			allAssets[i] = allAssets[i].Substring(allAssets[i].IndexOf("/Assets")+1);
			allAssets[i] = allAssets[i].Replace(@"\","/");
		}
		return allAssets;
	}
}
