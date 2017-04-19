using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.SignIn.Model;
using Common.Util;
using Logic.UI.CommonReward.View;
using Logic.Game.Model;
using Common.Localization;
using Logic.Enums;

namespace Logic.UI.SignIn.View
{
	public class SignInButton : MonoBehaviour 
	{
		public SignInInfo info;


		public Transform rewardRoot;
		public GameObject signGO;
		public Text textVip;
		public Text textVipMultiple;
		public GameObject vipGO;
		public GameObject notSignGO;
		public Text textDay;
		private CommonRewardIcon _rewardIcon;

		public void Set(SignInInfo info)
		{
			this.info = info;
			Refresh();
		}

		private void Refresh()
		{
			if(_rewardIcon == null)
			{
				_rewardIcon = CommonRewardIcon.Create(rewardRoot);
				_rewardIcon.transform.SetAsFirstSibling();

			}
			GameResData data = info.signData.awardItemList[0];
			_rewardIcon.SetGameResData(data);
			_rewardIcon.SetDesButtonType(ShowDescriptionType.click);
			float scale = 1;
			bool isGrayMark =false;
			bool isSignMark = false;
			if (SignInProxy.instance.curSignInId == info.id)//today
			{
				if(!SignInProxy.instance.isSignInToday)//未签到
				{
					scale = 1.1f;
				}else
				{
					isSignMark = true;
				}
			}else
			{
				isSignMark = info.isSign;
				isGrayMark = !info.isSign;
			}
			_rewardIcon.transform.localScale = new Vector3(scale,scale,scale);
			signGO.SetActive(isSignMark);
			notSignGO.SetActive(isGrayMark);
			UIUtil.SetGray(_rewardIcon.gameObject,isGrayMark);
			textDay.text = info.id.ToString();
			vipGO.SetActive(info.signData.vip_lv > 0);
			textVip.text = string.Format(Localization.Get("ui.signInView.Vip"),info.signData.vip_lv);
			textVipMultiple.text = string.Format(Localization.Get("ui.signInView.VipMultiple"),info.signData.vip_multiple);
		}
	}

}
