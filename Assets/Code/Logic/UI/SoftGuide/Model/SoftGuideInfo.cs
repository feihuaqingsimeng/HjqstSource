using UnityEngine;
using System.Collections;
using Logic.FunctionOpen.Model;

namespace Logic.UI.SoftGuide.Model
{
	public class SoftGuideInfo  
	{
		public int id;
		public FunctionData data;
		public bool isTipOver;

		public bool hasFirstView;
		public string firstViewPath = string.Empty;
		public string firstButtonPath = string.Empty;

		public bool hasSecondView;
		public string SecondViewPath = string.Empty;
		public string SecondButtonPath = string.Empty;

		public SoftGuideInfo (int id,bool isTipOver)
		{
			this.id = id;
			data = FunctionData.GetFunctionDataByID(id);
			this.isTipOver = isTipOver;
			if(string.IsNullOrEmpty(data.show_main_position))
			{
				hasFirstView = false;
			}else
			{
				hasFirstView = true;
				string[] mainPos = data.show_main_position.Split('#');
				firstViewPath = mainPos[0];
				firstButtonPath = mainPos[1];
			}

			if(string.IsNullOrEmpty(data.show_sheet_position))
			{
				hasSecondView = false;
			}else
			{
				hasSecondView = true;
				string[] mainPos = data.show_sheet_position.Split('#');
				SecondViewPath = mainPos[0];
				SecondButtonPath = mainPos[1];
			}
		}
	}
}

