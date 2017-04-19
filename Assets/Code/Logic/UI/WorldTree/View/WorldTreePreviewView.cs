using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Logic.Game.Model;
using Logic.Dungeon.Model;
using Logic.UI.WorldTree.Model;
using Logic.Hero.Model;
using Common.ResMgr;
using Common.Util;
using Logic.UI.CommonHeroIcon.View;
using Logic.Enums;
using Logic.UI.CommonReward.View;
using Logic.UI.WorldTree.Controller;
using Logic.UI.Tips.View;
using Logic.FunctionOpen.Model;
using Logic.UI.Description.View;

namespace Logic.UI.WorldTree.View
{
	public class WorldTreePreviewView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/world_tree/world_tree_preview_view";

		private Dictionary<int, WorldTreeDungeonButton> _worldTreeDungeonButtonDictionary = new Dictionary<int, WorldTreeDungeonButton>();
		private WorldTreeDungeonInfo _selectedWorldTreeDungeonInfo = null;

		public GameObject core;
		public CanvasGroup worldTreeContentCanvasGroup;

		#region UI components
		public ScrollRect bgScrollRect;
		public Transform cloudRoot;

		public Text floorNoText;
		public Text dungeonStatusText;
		public Text monstersText;
		public Text monstersDescriptionsText;
		public Text inspectMonstersText;
		public Text passRewardText;

		public Transform monsterRoot;
		public Transform rewardRoot;

		public ScrollRect worldTreeDungeonsScrollRect;
		public Transform worldTreeDungeonButtonsRoot;
		public WorldTreeDungeonButton worldTreeDungeonButtonPrefab;
		public Image selectedDungeonIndicatorImage;
		public Text embattleText;
		public Text startChallengeText;
		public Image costWorldTreeFruitIconImage;

		public GameObject startChallengeGameObject;
		public GameObject passedGameObject;
		public GameObject lockedGameObject;
		#endregion UI components

		void Awake ()
		{
			worldTreeContentCanvasGroup.alpha = 0;

			CommonTopBar.View.CommonTopBarView view = CommonTopBar.View.CommonTopBarView.CreateNewAndAttachTo(core.transform);
			view.SetAsCommonStyle(Localization.Get("ui.world_tree_preview_view.title"),OnClickBackButtonHandler, false, true, true, true, false, false, true);

			monstersText.text = Localization.Get("ui.world_tree_preview_view.monsters");
			inspectMonstersText.text = Localization.Get("ui.world_tree_preview_view.inpect_monster");
			passRewardText.text = Localization.Get("ui.world_tree_preview_view.pass_reward");
			embattleText.text = Localization.Get("ui.world_tree_preview_view.embattle");
			startChallengeText.text = Localization.Get("ui.world_tree_preview_view.start_challenge");
			costWorldTreeFruitIconImage.SetSprite(ResMgr.instance.Load<Sprite>(UIUtil.GetBaseResIconPath(BaseResType.TowerAction)));

			worldTreeDungeonButtonPrefab.gameObject.SetActive(false);

			WorldTreeController.instance.CLIENT2LOBBY_WORLD_TREE_FRUIT_SYN_REQ();
			WorldTreeController.instance.CLIENT2LOBBY_WORLD_TREE_REQ();

			WorldTreeProxy.instance.onWorldTreeDungeonInfosUpdateDelegate += OnWorldTreeDungeonInfosUpdateHandler;
			WorldTreeProxy.instance.onWorldTreeFruitUpdateDelegate += OnWorldTreeFruitUpdateHandler;
		}

		void Start ()
		{
			LTDescr ltDescr = LeanTween.moveLocalX(cloudRoot.gameObject, -1136, 200f);
			ltDescr.setFrom(1136);
			ltDescr.tweenType = LeanTweenType.linear;
			ltDescr.setRepeat(-1);
		}

		void OnDestroy ()
		{
			WorldTreeProxy.instance.onWorldTreeDungeonInfosUpdateDelegate -= OnWorldTreeDungeonInfosUpdateHandler;
			WorldTreeProxy.instance.onWorldTreeFruitUpdateDelegate -= OnWorldTreeFruitUpdateHandler;
		}

		private void RegenerateWorldDungeonButton ()
		{
			_worldTreeDungeonButtonDictionary.Clear();
			worldTreeDungeonButtonPrefab.gameObject.SetActive(false);
			List<WorldTreeDungeonInfo> allWorldDungeonInfoList = WorldTreeProxy.instance.GetAllWorldTreeDungeonInfoList();
			int allWorldDungeonInfoCount = allWorldDungeonInfoList.Count;
			List<Vector2> worldTreeDungeonButtonPosTemplateList = GlobalData.GetGlobalData().worldTreeDungeonPosList;
			WorldTreeDungeonInfo worldTreeDungeonInfo = null;
			Vector2 worldTreeDungeonButtonPos;
			for (int i = 0; i < allWorldDungeonInfoCount; i++)
			{
				worldTreeDungeonInfo = allWorldDungeonInfoList[i];
				WorldTreeDungeonButton worldTreeDungeonButton = GameObject.Instantiate<WorldTreeDungeonButton>(worldTreeDungeonButtonPrefab);
				worldTreeDungeonButton.SetWorldTreeDungeonInfo(worldTreeDungeonInfo);
				worldTreeDungeonButton.transform.SetParent(worldTreeDungeonButtonsRoot, false);

				worldTreeDungeonButtonPos = worldTreeDungeonButtonPosTemplateList[i % worldTreeDungeonButtonPosTemplateList.Count];
				worldTreeDungeonButtonPos += new Vector2(-120 + (i % 2) * 250, (i / worldTreeDungeonButtonPosTemplateList.Count) * 640);
				worldTreeDungeonButton.GetComponent<RectTransform>().anchoredPosition = worldTreeDungeonButtonPos;
				worldTreeDungeonButton.gameObject.SetActive(true);
				_worldTreeDungeonButtonDictionary.Add(worldTreeDungeonInfo.dungeonID, worldTreeDungeonButton);
			}
			float contentIdealPosY = -_worldTreeDungeonButtonDictionary[WorldTreeProxy.instance.UnlockedWorldTreeDungeonInfo.dungeonID].GetComponent<RectTransform>().anchoredPosition.y + worldTreeDungeonsScrollRect.viewport.rect.size.y * 0.5f;
			contentIdealPosY = Mathf.Clamp(contentIdealPosY, -(worldTreeDungeonsScrollRect.content.rect.size.y - worldTreeDungeonsScrollRect.viewport.rect.y), 0);
			worldTreeDungeonsScrollRect.content.anchoredPosition = new Vector2(0, contentIdealPosY);
		}

		public void RefreshMonsterIcons()
		{
			TransformUtil.ClearChildren(monsterRoot, true);
			List<HeroInfo> monsters = _selectedWorldTreeDungeonInfo.dungeonData.heroPresentList;	
			HeroInfo info = null;
			for(int i = 0,count = monsters.Count;i<count;i++)
			{
				info = monsters[i];
				CommonHeroIcon.View.CommonHeroIcon heroIcon =CommonHeroIcon.View.CommonHeroIcon.Create(monsterRoot);
//				heroIcon.transform.localScale = new Vector3(0.68f,0.68f,1);
				RoleDesButton.Get(heroIcon.gameObject).SetRoleInfo(info,ShowDescriptionType.click);
				heroIcon.SetHeroInfo(info);
				heroIcon.HideLevel();
			}
		}
		
		public void RefreshRewardIcons()
		{
			TransformUtil.ClearChildren(rewardRoot,true);
			List<GameResData> rewards = _selectedWorldTreeDungeonInfo.dungeonData.eachLootPresent;
			GameResData resData = null;
			int exp = 0;
			for(int i = 0,count = rewards.Count;i<count;i++)
			{
				resData = rewards[i];
				CommonRewardIcon icon = CommonRewardIcon.Create(rewardRoot);
				icon.SetGameResData(resData);
				icon.SetDesButtonType(ShowDescriptionType.click);
			}
		}

		private void ResetSelectedWorldTreeDungeonInfo (WorldTreeDungeonInfo worldTreeDungeonInfo)
		{
			_selectedWorldTreeDungeonInfo = worldTreeDungeonInfo;
			selectedDungeonIndicatorImage.transform.localPosition = _worldTreeDungeonButtonDictionary[_selectedWorldTreeDungeonInfo.dungeonID].transform.localPosition + new Vector3(0, 105, 0);
			string dungeonStatusString = string.Empty;
			switch (worldTreeDungeonInfo.worldTreeDungeonStatus)
			{
				case WorldTreeDungeonStatus.Locked:
//					dungeonStatusString = Localization.Get("ui.world_tree_preview_view.locked");
					{
						startChallengeGameObject.SetActive(false);
						passedGameObject.SetActive(false);
						lockedGameObject.SetActive(true);
					}
					break;
				case WorldTreeDungeonStatus.Unlocked:
					{
						if (WorldTreeProxy.instance.FailedTimes > 0)
						{
							dungeonStatusString = string.Format(Localization.Get("ui.world_tree_preview_view.unlocked_and_challenge_failed"), WorldTreeProxy.instance.FailedTimes, WorldTreeProxy.instance.GetWorldTreeChallengeFailedWeakenValue() * 100);
						}
	//					else
	//					{
	//						dungeonStatusString = Localization.Get("ui.world_tree_preview_view.unlocked");
	//					}
						startChallengeGameObject.SetActive(true);
						passedGameObject.SetActive(false);
						lockedGameObject.SetActive(false);
					}
					break;
				case WorldTreeDungeonStatus.Passed:
//					dungeonStatusString = Localization.Get("ui.world_tree_preview_view.passed");
					{
						startChallengeGameObject.SetActive(false);
						passedGameObject.SetActive(true);
						lockedGameObject.SetActive(false);
					}
					break;
				default:
					break;
			}
			dungeonStatusText.text = dungeonStatusString;
			floorNoText.text = string.Format(Localization.Get("ui.world_tree_preview_view.floor_NO"), _selectedWorldTreeDungeonInfo.orderNumber);
			monstersDescriptionsText.text = Localization.Get(_selectedWorldTreeDungeonInfo.dungeonData.description);
			RefreshMonsterIcons();
			RefreshRewardIcons();
		}

		private void OnWorldTreeCanvasGroupAlphaUpdate (float alpha)
		{
			worldTreeContentCanvasGroup.alpha = alpha;
		}

		#region proxy callback handlers
		private void OnWorldTreeDungeonInfosUpdateHandler ()
		{
			RegenerateWorldDungeonButton();
			ClickWorldTreeDungeonButtonHandler(_worldTreeDungeonButtonDictionary.GetValue(WorldTreeProxy.instance.UnlockedWorldTreeDungeonInfo.dungeonID));
			LTDescr ltDescr = LeanTween.value(gameObject, 0, 1, 0.6f);
			ltDescr.setIgnoreTimeScale(true);
			ltDescr.setOnUpdate(OnWorldTreeCanvasGroupAlphaUpdate);

			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", PREFAB_PATH, "OnViewReady"));
		}
		
		private void OnWorldTreeFruitUpdateHandler ()
		{
		}
		#endregion proxy callback handlers

		#region UI event handlers
		public void OnScrollValueChanged (Vector2 val)
		{
			Debug.Log("========== Pos:" + val);
			bgScrollRect.verticalNormalizedPosition = val.y;
		}

		private void OnClickConfirmBuyWorldTreeFruitHandler ()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_Action);
		}

		private void OnClickBackButtonHandler ()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickWorldTreeDungeonButtonHandler (WorldTreeDungeonButton worldTreeDungeonButton)
		{
			ResetSelectedWorldTreeDungeonInfo(worldTreeDungeonButton.WorldTreeDungeonInfo);
		}

		public void ClickEmbattleHandler ()
		{
			FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
		}

		public void ClickStartChallengeHandler ()
		{
			if (WorldTreeProxy.instance.WorldTreeFruit <= 0)
			{
				ConfirmTipsView.Open(Localization.Get("ui.world_tree_preview_view.tips.out_of_fruit_description"), OnClickConfirmBuyWorldTreeFruitHandler);
				return;
			}

			if (_selectedWorldTreeDungeonInfo.worldTreeDungeonStatus == WorldTreeDungeonStatus.Locked)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.world_tree_preview_view.tips.dungeon_locked"));
				return;
			}
			if (_selectedWorldTreeDungeonInfo.worldTreeDungeonStatus == WorldTreeDungeonStatus.Passed)
			{
				CommonAutoDestroyTipsView.Open(Localization.Get("ui.world_tree_preview_view.tips.dungeon_passed"));
				return;
			}
			WorldTreeController.instance.CLIENT2LOBBY_WORLD_TREE_CHALLENGE_REQ(_selectedWorldTreeDungeonInfo.dungeonID);
		}
		#endregion UI event handlers
	}
}