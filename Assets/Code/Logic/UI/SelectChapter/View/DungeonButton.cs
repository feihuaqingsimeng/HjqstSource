using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Logic.Dungeon.Model;
using Logic.Player.Model;
using Logic.Character;
using Logic.Game.Model;
using Common.Animators;
using Common.ResMgr;
using Logic.Item.Model;
using Logic.Equipment.Model;
using Logic.Hero.Model;
using Logic.Enums;

namespace Logic.UI.SelectChapter.View
{
	public class DungeonButton : MonoBehaviour
	{
		private DungeonInfo _dungeonInfo;
		public DungeonInfo DungeonInfo
		{
			get
			{
				return _dungeonInfo;
			}
		}

		public Image img;

		public Text nameText;
		public Image bossMarkImage;
		public GameObject[] stars;
		public GameObject rewardItemFrame;
		public GameObject iconRoot;

		private Sprite _normalEnabledSprite;
		private Sprite _eliteEnabledSprite;
		private Sprite _bossEnabledSprite;
		private Sprite _normalDisabledSprite;
		private Sprite _eliteDisabledSprite;
		private Sprite _bossDisabledSprite;
		private CommonReward.View.CommonRewardIcon _commonRewardIcon;

		void Awake()
		{
			_normalEnabledSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_bg_checkpoint_02_01");
			_eliteEnabledSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_bg_checkpoint_03_01");
			_bossEnabledSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_bg_checkpoint_04_01");
			_normalDisabledSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_bg_checkpoint_02_02");
			_eliteDisabledSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_bg_checkpoint_03_02");
			_bossDisabledSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_bg_checkpoint_04_02");
		}

		public void SetDungeonInfo (DungeonInfo dungeonInfo)
		{
			_dungeonInfo = dungeonInfo;
			nameText.text = Localization.Get(_dungeonInfo.dungeonData.order_name);

			bossMarkImage.gameObject.SetActive(_dungeonInfo.dungeonData.dungeonBossType == DungeonBossType.Boss);

			nameText.gameObject.SetActive(!_dungeonInfo.isLock);
			img.SetSprite( _dungeonInfo.isLock ? GetDisabledSprite() : GetEnabledSprite());

			for (int i = 0; i < stars.Length; i++)
			{
				bool shouldShow = (i < dungeonInfo.star);
				stars[i].SetActive(shouldShow);
			}

//			if (_dungeonInfo.dungeonData.showFirstPassRewardGameResData != null && _dungeonInfo.star <= 0)
//			{
//				if (_commonRewardIcon == null)
//				{
//					_commonRewardIcon = CommonReward.View.CommonRewardIcon.Create(iconRoot.transform);
//				}
//				_commonRewardIcon.SetGameResData(_dungeonInfo.dungeonData.showFirstPassRewardGameResData);
//				//_commonRewardIcon.SetDesButtonEnable(false);
//				rewardItemFrame.SetActive(true);
//			}
//			else
//			{
//				rewardItemFrame.SetActive(false);
//			}

			if (_dungeonInfo.dungeonData.showFirstPassRewardGameResData != null)
			{
				if (_commonRewardIcon == null)
				{
					_commonRewardIcon = CommonReward.View.CommonRewardIcon.Create(iconRoot.transform);
				}
				_commonRewardIcon.SetGameResData(_dungeonInfo.dungeonData.showFirstPassRewardGameResData);
				//_commonRewardIcon.SetDesButtonEnable(false);
				rewardItemFrame.SetActive(true);
			}
			else
			{
				rewardItemFrame.SetActive(false);
			}
		}

		private Sprite GetEnabledSprite ()
		{
			switch (_dungeonInfo.dungeonData.dungeonBossType)
			{
				case DungeonBossType.Normal:
					return _normalEnabledSprite;
				case DungeonBossType.Elite:
					return _eliteEnabledSprite;
				case DungeonBossType.Boss:
					return _bossEnabledSprite;
			}
			return _normalEnabledSprite;
		}

		private Sprite GetDisabledSprite ()
		{
			switch (_dungeonInfo.dungeonData.dungeonBossType)
			{
				case DungeonBossType.Normal:
					return _normalDisabledSprite;
				case DungeonBossType.Elite:
					return _eliteDisabledSprite;
				case DungeonBossType.Boss:
					return _bossDisabledSprite;
			}
			return _normalDisabledSprite;
		}

		#region ui handlers
		public void ClickHandler ()
		{
//			if (!_dungeonInfo.isLock)
//			{
//				Logic.UI.DungeonDetail.View.DungeonDetailView dungeonDetialView = UIMgr.instance.Open<Logic.UI.DungeonDetail.View.DungeonDetailView>(Logic.UI.DungeonDetail.View.DungeonDetailView.PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
//				dungeonDetialView.SetDungeonInfo(_dungeonInfo);
//			}
//			else
//			{
//				Tips.View.CommonAutoDestroyTipsView.Open(Localization.Get("ui.select_chapter_view.dungeon_is_locked"));
//			}
			Logic.UI.DungeonDetail.View.DungeonDetailView dungeonDetialView = UIMgr.instance.Open<Logic.UI.DungeonDetail.View.DungeonDetailView>(Logic.UI.DungeonDetail.View.DungeonDetailView.PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
			dungeonDetialView.SetDungeonInfo(_dungeonInfo);
		}
		#endregion ui handlers
	}
}
