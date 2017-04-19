using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.UI.CommonReward.View;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Game.Model;
using Common.Util;
using Common.Localization;

namespace  Logic.UI.Tips.View
{
	public class CommonRewardTipsView : MonoBehaviour 
	{
		public const string PREFAB_PATH = "ui/tips/common_reward_tips_view";
		
		public static CommonRewardTipsView Open(List<GameResData> dataList,bool rewardEnable = false, System.Action clickRewardCallback = null)
		{
			CommonRewardTipsView view = UIMgr.instance.Open<CommonRewardTipsView>(PREFAB_PATH, EUISortingLayer.Tips);
			view._clickRewardCallback = clickRewardCallback;
			view.SetRewardButtonEnable(rewardEnable);
			view.SetDataList(dataList);
			return view;
		}

		#region ui component
		public GameObject rewardIconPrefab;
		public Transform rewardIconRoot;
		public Button getRewardBtn;
		#endregion

		private System.Action _clickRewardCallback;

		void Start()
		{
//			List<GameResData> rewardList = new List<GameResData>();
//			rewardList.Add(new GameResData(Logic.Enums.BaseResType.Account_Exp,0,100,0));
//			rewardList.Add(new GameResData(Logic.Enums.BaseResType.Account_Exp,0,100,0));
//			rewardList.Add(new GameResData(Logic.Enums.BaseResType.Account_Exp,0,100,0));
//			rewardList.Add(new GameResData(Logic.Enums.BaseResType.Account_Exp,0,100,0));
//			rewardList.Add(new GameResData(Logic.Enums.BaseResType.Account_Exp,0,100,0));
//			rewardList.Add(new GameResData(Logic.Enums.BaseResType.Account_Exp,0,100,0));
//			rewardList.Add(new GameResData(Logic.Enums.BaseResType.Account_Exp,0,100,0));
//			SetDataList(rewardList);
		}

		private void Init()
		{


		}
		public void SetDataList(List<GameResData> dataList)
		{
			if(dataList == null)
			{
				return;
			}
			TransformUtil.ClearChildren(rewardIconRoot,true);
			int count = dataList.Count;
			int spaceHorizon = 130;
			int spaceVertical = 120;
			int columnMax = 4;
			int totalRow = count/columnMax + (count%columnMax == 0 ? 0 : 1) ;
			//
			GridLayoutGroup layout = rewardIconRoot.GetComponent<GridLayoutGroup>();

			float height = layout.cellSize.y+layout.spacing.y;
			RectTransform rectTran = rewardIconRoot.transform as RectTransform;
			Vector2 rewardRootSizeDelta = rectTran.sizeDelta;
			rewardRootSizeDelta = rectTran.sizeDelta = new Vector2(rewardRootSizeDelta.x, height*totalRow-layout.spacing.y+50);
				
			if(totalRow == 1)
			{
				if(layout!=null)
					layout.enabled = false;
			}else
			{
				if(layout!=null)
					layout.enabled = true;
			}
			rewardIconPrefab.SetActive(true);

			float xPadding = rewardRootSizeDelta.x/2;
			float yPadding = -100;
			for(int i = 0;i<count;i++)
			{
				GameObject clone = Instantiate<GameObject>(rewardIconPrefab);

				CommonRewardIcon icon = CommonRewardIcon.Create(clone.transform);
				RectTransform iconTran = icon.transform as RectTransform;
				iconTran.SetAsFirstSibling();
//				iconTran.anchorMax = iconTran.anchorMin = new Vector2(0.5f,0.5f);
				GameResData resData = dataList[i];
				icon.SetGameResData(resData);
				Transform tran = clone.transform;
				tran.SetParent(rewardIconRoot,false);
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

				tran.localPosition = new Vector3(x+xPadding,y+yPadding);
				Text countText = tran.Find("text_count").GetComponent<Text>();
				countText.text = string.Format(Localization.Get("common.x_count"),resData.count);

				icon.ShowRewardCount(true);
			}
			rewardIconPrefab.SetActive(false);
		}
		private void SetRewardButtonEnable(bool enable)
		{
			getRewardBtn.enabled = enable;
			getRewardBtn.GetComponent<Image>().SetGray(!enable);
			Text text = getRewardBtn.GetComponentInChildren<Text>();
			if(text!= null)
			{
				text.color = enable ? Color.white : Color.gray;
				 Outline outline = text.GetComponent<Outline>();
				if(outline!=null)
					outline.enabled = false;
			}
		}
		public void OnClickCloseBtnHandler()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		public void OnClickGetRewardBtnHandler()
		{
			if(_clickRewardCallback!= null)
			{
				_clickRewardCallback();
			}

			UIMgr.instance.Close(PREFAB_PATH);
		}

	}

}
