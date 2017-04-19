using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Logic.UI.CommonReward.View;
using Logic.UI.Pvp.Model;
using Common.Localization;
using Common.Util;
using Logic.Game.Model;

namespace  Logic.UI.Pvp.View
{
	public class PvpRewardRankButton : MonoBehaviour 
	{
		
		#region ui component
		public Text rankNameText;
		public Transform rewardIconPrefab;
		public Transform rewardIconRoot;
		#endregion

		private PvpArenaPrizeData _prizeData;

		public void SetArenaPrizeData(PvpArenaPrizeData data)
		{
			_prizeData = data;
			SetTitleName();
			SetReward();
		}
		private void SetTitleName()
		{
			if(_prizeData.endInterval == -1)
			{
				rankNameText.text = string.Format(Localization.Get("ui.pvp_reward_rank_view.rankName3"),_prizeData.beginInterval);
			}else if(_prizeData.beginInterval == _prizeData.endInterval)
			{
				rankNameText.text = string.Format(Localization.Get("ui.pvp_reward_rank_view.rankName2"),_prizeData.beginInterval);
			}else
			{
				rankNameText.text = string.Format(Localization.Get("ui.pvp_reward_rank_view.rankName1"),_prizeData.beginInterval,_prizeData.endInterval);
			}
		}
		private void SetReward()
		{
			TransformUtil.ClearChildren(rewardIconRoot,true);

			rewardIconPrefab.gameObject.SetActive(true);
			for(int i = 0,count = _prizeData.bonusList.Count;i<count;i++)
			{
				GameResData resData = _prizeData.bonusList[i];
				Transform icon = Instantiate<Transform>(rewardIconPrefab);
				icon.SetParent(rewardIconRoot,false);
				icon.Find("text_count").GetComponent<Text>().text = string.Format(Localization.Get("common.x_count"),resData.count);
				CommonRewardIcon rewardIcon = CommonRewardIcon.Create(icon);
				rewardIcon.transform.SetAsFirstSibling();
				rewardIcon.SetGameResData(resData);
				rewardIcon.HideCount();
			}
			rewardIconPrefab.gameObject.SetActive(false);
		}
	}

}
