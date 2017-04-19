using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.UI.CommonReward.View;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Game.Model;
using Common.Util;
using Common.Localization;

namespace  Logic.UI.Pvp.View
{
	public class PvpGainRewardView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/pvp/pvp_gain_reward_view";
		
		public static PvpGainRewardView Open(List<GameResData> dataList,bool combineList = false)
		{
			PvpGainRewardView view = UIMgr.instance.Open<PvpGainRewardView>(PREFAB_PATH,EUISortingLayer.Tips);
			view.SetDataList(dataList,combineList);
			return view;
		}

		#region ui component
		public Transform rewardIconPrefab;
		public Transform rewardIconRoot;
		#endregion

		void Start()
		{
			Init();
		}

		private void Init()
		{
		
		}
		public void SetDataList(List<GameResData> dataList,bool combineList)
		{
			if(dataList == null)
			{
				return;
			}
			if(combineList)
				dataList = UIUtil.CombineGameResList(dataList);

			TransformUtil.ClearChildren(rewardIconRoot,true);
			int count = dataList.Count;
			int spaceHorizon = 130;
			int spaceVertical = 120;
			int columnMax = 4;
			int totalRow = count/columnMax + (count%columnMax == 0 ? 0 : 1) ;
			rewardIconPrefab.gameObject.SetActive(true);
			for(int i = 0;i<count;i++)
			{
				Transform iconTran = Instantiate<Transform>(rewardIconPrefab);
				GameResData resData = dataList[i];

				iconTran.SetParent(rewardIconRoot,false);
				float x = 0;
				float y = 0;
				int row = i/columnMax;
				int col = i%columnMax;
				if(totalRow == 1)//单行时居中
				{
					x = count%2 == 0 ?((i-count/2)*spaceHorizon+spaceHorizon/2):((i-count/2)*spaceHorizon);
				}else{//左对齐
					x = columnMax%2 == 0 ? ((col-columnMax/2)*spaceHorizon+spaceHorizon/2):((col-columnMax/2)*spaceHorizon);
					y = totalRow%2 == 0 ? ((totalRow/2-row)*spaceVertical-spaceVertical/2):((totalRow/2-row)*spaceVertical);
				}

				iconTran.localPosition = new Vector3(x,y);
				Text countText = iconTran.Find("text_count").GetComponent<Text>();
				countText.text = string.Format(Localization.Get("common.x_count"),resData.count);
				CommonRewardIcon rewardIcon = CommonRewardIcon.Create(iconTran);
				rewardIcon.SetGameResData(resData);
				rewardIcon.HideCount();
			}
			rewardIconPrefab.gameObject.SetActive(false);
		}
		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

	}

}
