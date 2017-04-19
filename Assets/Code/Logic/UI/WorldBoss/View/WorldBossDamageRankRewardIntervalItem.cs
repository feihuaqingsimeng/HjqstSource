using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Logic.WorldBoss.Model;
using Logic.UI.CommonReward.View;
using Logic.Game.Model;
using Common.ResMgr;

namespace Logic.UI.WorldBoss.View
{
	public class WorldBossDamageRankRewardIntervalItem : MonoBehaviour
	{
		public Image top3TitleImage;
//		public Sprite top1TitleSprite;
//		public Sprite top2TitleSprite;
//		public Sprite top3TitleSprite;
		public Text intervalText;

		public Transform rewardItemRoot;

		public void SetInfo (WorldBossData worldBossData)
		{
			if (worldBossData.rankMin <= 3)
			{
				if (worldBossData.rankMin == 1)
				{
					top3TitleImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_1st"));
				}
				else if (worldBossData.rankMin == 2)
				{
					top3TitleImage.SetSprite( ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_2st"));
				}
				else if (worldBossData.rankMin == 3)
				{
					top3TitleImage.SetSprite(ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_3st"));
				}
				top3TitleImage.SetNativeSize();
				top3TitleImage.gameObject.SetActive(true);
				intervalText.gameObject.SetActive(false);
			}
			else
			{
				top3TitleImage.gameObject.SetActive(false);
				intervalText.gameObject.SetActive(true);
				if (worldBossData.rankMax > 0)
				{
					intervalText.text = string.Format(Localization.Get("ui.world_boss_reward.interval_1"), worldBossData.rankMin, worldBossData.rankMax);
				}
				else
				{
					intervalText.text = string.Format(Localization.Get("ui.world_boss_reward.interval_2"), worldBossData.rankMin);
				}
			}

			List<GameResData> rewardShowList = worldBossData.rewardShowList;
			int rewardShowItemCount = rewardShowList.Count;
			for (int i = 0; i < rewardShowItemCount; i++)
			{
				CommonRewardIcon commonRewardIcon = CommonRewardIcon.Create(rewardItemRoot);
				commonRewardIcon.SetGameResData(rewardShowList[i]);
			}
		}
	}
}