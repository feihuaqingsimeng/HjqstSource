using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Logic.UI.Pvp.Model;
using Common.Util;
using Common.UI.Components;
namespace Logic.UI.Pvp.View
{
	public class PvpRewardRankView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/pvp/pvp_reward_rank_view";
		public static PvpRewardRankView Open()
		{
			PvpRewardRankView view = UIMgr.instance.Open<PvpRewardRankView>(PREFAB_PATH,EUISortingLayer.MainUI,UIOpenMode.Overlay);
			return view;
		}

		#region ui component
		public ScrollContent scrollContent;
		#endregion
		private List<PvpArenaPrizeData> _prizeDataList ;
		void Start()
		{
			_prizeDataList = PvpArenaPrizeData.arenaPrizeDataDictionary.GetValues();
			_prizeDataList.Reverse();
			scrollContent.Init(_prizeDataList.Count,true);
		}

		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		public void OnResetScrollItemHandler(GameObject go,int index)
		{
			PvpRewardRankButton rewardButton = go.GetComponent<PvpRewardRankButton>();
			if(rewardButton!= null)
			{
				rewardButton.SetArenaPrizeData(_prizeDataList[index]);
			}
		}
	}

}
