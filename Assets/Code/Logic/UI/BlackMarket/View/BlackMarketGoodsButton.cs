using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Logic.UI.CommonReward.View;
using Logic.Game.Model;
using Logic.Enums;
using Logic.UI.BlackMarket.Model;
using Common.Localization;
using Logic.UI.CommonHeroIcon.View;
using Logic.UI.Description.View;

namespace Logic.UI.BlackMarket.View
{
	public class BlackMarketGoodsButton : MonoBehaviour 
	{
		public delegate void BlackMarketClickDelegate(BlackMarketGoodsButton info);
		public BlackMarketClickDelegate onBlackMarketClickDelegate; 

		#region ui component
		public Text remaindCountText;
		public Text limitLevelText;
		public GameObject maskGameObject;

		#endregion

		public BlackMarketInfo blackMarketInfo;

		private CommonRewardIcon _commonRewardIcon;
		private CommonHeroIcon.View.CommonHeroIcon _commonHeroIcon;

		public void SetData(BlackMarketInfo info,bool bigHero = false,ShowDescriptionType clickType = ShowDescriptionType.longPress)
		{
			blackMarketInfo = info;

			if(_commonRewardIcon != null)
			{
				GameObject.DestroyObject(_commonRewardIcon.gameObject);
			}
			if(_commonHeroIcon != null)
			{
				GameObject.DestroyObject(_commonHeroIcon.gameObject);
			}
			if(bigHero)
			{
				_commonHeroIcon = CommonHeroIcon.View.CommonHeroIcon.CreateBigIcon(transform);
				_commonHeroIcon.transform.SetAsFirstSibling();
				_commonHeroIcon.SetGameResData(info.itemData);
				RoleDesButton.Get(_commonHeroIcon.gameObject).SetRoleInfo(info.itemData,clickType);
				
			}else {
				_commonRewardIcon = CommonRewardIcon.Create(transform);
				_commonRewardIcon.transform.SetAsFirstSibling();
				_commonRewardIcon.SetGameResData(info.itemData);
				if(clickType == ShowDescriptionType.longPress)
					_commonRewardIcon.onClickHandler = OnClickRewardBtnHandler;
				_commonRewardIcon.SetDesButtonType(clickType);
			}
			remaindCountText.text = info.remaindCountString;
			if(maskGameObject != null)
			{
				maskGameObject.SetActive(GameProxy.instance.AccountLevel < blackMarketInfo.limitLv);
				limitLevelText.text = string.Format(Localization.Get("ui.black_market_view.limit_lv"),blackMarketInfo.limitLv);
			}

			GetComponent<Image>().enabled = false;
		}
		public Text GetRemaindCountText()
		{
			return remaindCountText;
		}
		public bool IsSelect
		{
			get
			{
				bool isSelect = false;
				if(_commonHeroIcon != null)
				{
					isSelect = _commonHeroIcon.IsSelect;
				}
				if(_commonRewardIcon != null)
				{
					isSelect = _commonRewardIcon.IsSelect;
				}
				return isSelect;
			}
		}
		public void SetSelect(bool isSelect)
		{
			if(_commonHeroIcon != null)
			{
				_commonHeroIcon.SetSelect(isSelect);
			}
			if(_commonRewardIcon != null)
			{
				_commonRewardIcon.SetSelect(isSelect);
			}
		}

		public void OnClickHeroBtnHandler(CommonHeroIcon.View.CommonHeroIcon icon)
		{
			if(onBlackMarketClickDelegate!= null)
			{
				onBlackMarketClickDelegate(this);
			}
		}
		public void OnClickRewardBtnHandler(GameObject go)
		{
			if(onBlackMarketClickDelegate!= null)
			{
				onBlackMarketClickDelegate(this);
			}
		}
	}

}
