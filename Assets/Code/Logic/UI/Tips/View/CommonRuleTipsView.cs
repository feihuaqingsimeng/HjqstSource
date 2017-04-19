using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace Logic.UI.Tips.View
{
	public class CommonRuleTipsView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/tips/common_rule_tips_view";
		
		public static CommonRuleTipsView Open(string title,string content)
		{
			CommonRuleTipsView view = UIMgr.instance.Open<CommonRuleTipsView>(PREFAB_PATH,EUISortingLayer.Tips);
			view.SetData(title,content);
			return view;
		}
		
		#region ui component
		public Text textTitle;
		public Text textContent;
		#endregion

		private void SetData(string title,string content)
		{
			textTitle.text = title;
			textContent.text = content;
		}

		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}

