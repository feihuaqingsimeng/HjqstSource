using UnityEngine;
using System.Collections;
using Logic.UI.Pvp.Model;
using System.Collections.Generic;
using UnityEngine.UI;
using Common.Util;
using Common.UI.Components;
namespace  Logic.UI.Pvp.View
{
	public class PvpTopHundredRankView : MonoBehaviour 
	{
		
		public const string PREFAB_PATH = "ui/pvp/pvp_top_hundred_rank_view";

		public static PvpTopHundredRankView Open(List<PvpFighterInfo> fighterList)
		{
			PvpTopHundredRankView view = UIMgr.instance.Open<PvpTopHundredRankView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			view.SetTopHundredRanking(fighterList);
			return view;
		}

		private List<PvpFighterInfo> _fighterList;

		#region ui component
		public Text textTitle;
		public ScrollContent scrollContent;
		#endregion


		void Awake()
		{
			Init();
		}

		private void Init()
		{

		}
		public void SetTopHundredRanking(List<PvpFighterInfo> fighterList)
		{
			_fighterList = fighterList;
			scrollContent.Init(_fighterList.Count,true);
		}

		#region ui event handler
		public void OnResetScrollItemHandler(GameObject go,int index)
		{
			PvpTopHundredFighterButton fighterButton = go.GetComponent<PvpTopHundredFighterButton>();
			if(fighterButton!= null)
			{
				fighterButton.SetPvpFighterInfo(_fighterList[index]);
			}
		}
		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion
		
	}
}

