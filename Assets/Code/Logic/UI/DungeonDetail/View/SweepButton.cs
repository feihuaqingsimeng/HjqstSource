using UnityEngine;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.Enums;
using UnityEngine.UI;
using Common.Util;
using Logic.UI.CommonReward.View;
using Logic.VIP.Model;

namespace Logic.UI.DungeonDetail.View
{
	public class SweepButton : MonoBehaviour {


		#region ui component
		public Text textTitle;
		public Text textAccountLv;
		public Text textAccountName;
		public Text textAccountAdd;
		public Slider sliderAccountBar;

		public Transform rewardRoot;

//		public Transform rewardBasicRoot;
//		public Transform rewardBasicPrefab;
		public Text textMoney;
		public Text textMoneyVipTip;
		public GameObject bottomRootGO;
		public GameObject bottomLineGO;
		#endregion

		private List<GameResData> _dataList;
		private string _title;
		private int _accountLevel;
		private int _accountExp;
		private float _addExpPercent;
		public void SetSweepData(string title,List<GameResData> datas,int accountLevel,int accountExp,float addExpPercent)
		{
			_dataList = datas;
			_title = title;
			_accountLevel = accountLevel;
			_accountExp = accountExp;
			_addExpPercent = addExpPercent;
			Refresh();
		}

		private void Refresh()
		{
		
			textTitle.text = _title;
			TransformUtil.ClearChildren(rewardRoot,true);
			GameResData resData;
			int totalMoney = 0;
			int totalAccountExp = 0;
			for(int i = 0,count = _dataList.Count;i<count;i++)
			{
				resData = _dataList[i];
				if(resData.type != BaseResType.Account_Exp && resData.type != BaseResType.Hero_Exp && resData.type != BaseResType.Gold)
				{
					CommonRewardIcon icon = CommonRewardIcon.Create(rewardRoot);
					icon.SetGameResData(resData);
				}

				if(resData.type == BaseResType.Gold)
					totalMoney += resData.count;
				else if(resData.type == BaseResType.Account_Exp)
					totalAccountExp += resData.count;
			}
			if(rewardRoot.childCount == 0)
			{
				gameObject.GetComponent<LayoutElement>().preferredHeight = gameObject.GetComponent<LayoutElement>().preferredHeight-100;
			}
			//exp
			textAccountLv.text = _accountLevel.ToString();
//			textAccountName.text = GameProxy.instance.PlayerInfo.name;
			textAccountName.text = GameProxy.instance.AccountName;
			sliderAccountBar.transform.localPosition = textAccountName.transform.localPosition + new Vector3(textAccountName.preferredWidth+21,0,0);
			textAccountAdd.text = string.Format("+{0}%",(int)_addExpPercent);
			AccountExpData expData = AccountExpData.GetAccountExpDataByLv(_accountLevel);
			if(expData == null)
			{
				sliderAccountBar.value = 0;
			}else
			{
				sliderAccountBar.value = (_accountExp+0.0f)/ expData.exp;
			}

			//money
			textMoney.text = totalMoney.ToString();
			int vip = VIPProxy.instance.VIPLevel;
			if(vip>0)
				textMoneyVipTip.text = string.Format("(vip{0} +{1}%)",vip,VIPProxy.instance.VIPData.dungeonExtraGoldAmount);
			else 
				textMoneyVipTip.text = string.Empty;
		}

	}
}

