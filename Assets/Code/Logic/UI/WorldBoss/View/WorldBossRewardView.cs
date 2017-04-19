using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Common.Util;
using Logic.WorldBoss.Model;
using Logic.Game.Model;
using Logic.UI.CommonReward.View;

namespace Logic.UI.WorldBoss.View
{
	public class WorldBossRewardView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/world_boss/world_boss_reward_view";

		#region UI components
		public Text titleText;
		public Text killRewardTitleText;

		public Transform damageRankRewardIntervalItemRoot;
		public WorldBossDamageRankRewardIntervalItem damageRankRewardIntervalItemPrefab;

		public Transform killWorldBossRewardRoot;
		#endregion UI components

		void Awake ()
		{
			titleText.text = Localization.Get("ui.world_boss_reward.title");
			killRewardTitleText.text = Localization.Get("ui.world_boss_reward.kill_reward_title");

			damageRankRewardIntervalItemPrefab.gameObject.SetActive(false);
			TransformUtil.ClearChildren(damageRankRewardIntervalItemRoot, true);
			List<WorldBossData> damageRankWorldBossDataList = WorldBossData.GetDamageRankWorldBossDataList();
			int damageRankWorldBossDataCount = damageRankWorldBossDataList.Count;
			for (int i = 0; i < damageRankWorldBossDataCount; i++)
			{
				WorldBossDamageRankRewardIntervalItem damageRankRewardIntervalItem = GameObject.Instantiate<WorldBossDamageRankRewardIntervalItem>(damageRankRewardIntervalItemPrefab);
				damageRankRewardIntervalItem.SetInfo(damageRankWorldBossDataList[i]);
				damageRankRewardIntervalItem.gameObject.SetActive(true);
				damageRankRewardIntervalItem.transform.SetParent(damageRankRewardIntervalItemRoot, false);
			}

			TransformUtil.ClearChildren(killWorldBossRewardRoot, true);
			List<GameResData> killWorldBossRewardList = WorldBossData.GetKillWorldBossReward().rewardShowList;
			int killWorldBossRewardCount = killWorldBossRewardList.Count;
			for (int i = 0; i < killWorldBossRewardCount; i++)
			{
				CommonRewardIcon commonRewardIcon = CommonRewardIcon.Create(killWorldBossRewardRoot);
				commonRewardIcon.transform.localScale = new Vector3(86f / 108, 86f / 108, 1);
				commonRewardIcon.SetGameResData(killWorldBossRewardList[i]);
			}
		}

		#region UI event handlers
		public void ClickCloseHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#endregion UI event handlers
	}
}