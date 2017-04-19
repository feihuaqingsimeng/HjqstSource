using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Common.ResMgr;
using Logic.UI.WorldTree.Model;

namespace Logic.UI.WorldTree.View
{
	public class WorldTreeDungeonButton : MonoBehaviour
	{
		private WorldTreeDungeonInfo _worldTreeDungeonInfo;
		public WorldTreeDungeonInfo WorldTreeDungeonInfo
		{
			get
			{
				return _worldTreeDungeonInfo;
			}
		}

		#region UI components
		public Text dungeonNameText;
		public Image rewardIconImage;
		public Image bgImage;
		public Sprite bgNormalSprite;
		public Sprite bgUnlockedSprite;
		#endregion UI components

		void Awake ()
		{
			bgNormalSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_bg_checkpoint_02_01");
			bgUnlockedSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_bg_checkpoint_02_02");
		}

		void Start ()
		{
//			if (_worldTreeDungeonInfo.worldTreeDungeonStatus == Logic.Enums.WorldTreeDungeonStatus.Unlocked)
//			{
//				bgImage.sprite = bgUnlockedSprite;
//			}
//			else if (_worldTreeDungeonInfo.worldTreeDungeonStatus == Logic.Enums.WorldTreeDungeonStatus.Passed)
//			{
//				bgImage.sprite = bgNormalSprite;
//			}
//			else if (_worldTreeDungeonInfo.worldTreeDungeonStatus == Logic.Enums.WorldTreeDungeonStatus.Locked)
//			{
//				bgImage.sprite = bgNormalSprite;
//				UIUtil.SetGray(gameObject, true);
//			}

			bgImage.SetSprite(bgNormalSprite);
			if (_worldTreeDungeonInfo.worldTreeDungeonStatus == Logic.Enums.WorldTreeDungeonStatus.Passed)
			{
				rewardIconImage.gameObject.SetActive(false);
				UIUtil.SetGray(gameObject, true);
			}
			else
			{
				if (_worldTreeDungeonInfo.dungeonData.worldTreeRewardIconPath == string.Empty)
					rewardIconImage.gameObject.SetActive(false);
				else
					rewardIconImage.gameObject.SetActive(true);
				UIUtil.SetGray(gameObject, false);
			}
			bgImage.SetNativeSize();
		}

		public void SetWorldTreeDungeonInfo (WorldTreeDungeonInfo worldTreeDungeonInfo)
		{
			_worldTreeDungeonInfo = worldTreeDungeonInfo;
			dungeonNameText.text = string.Format(Localization.Get("ui.world_tree_preview_view.dungeon_button_title"), _worldTreeDungeonInfo.orderNumber.ToString());

			if (worldTreeDungeonInfo.dungeonData.worldTreeRewardIconPath == string.Empty)
			{
				rewardIconImage.gameObject.SetActive(false);
			}
			else
			{
				rewardIconImage.SetSprite(ResMgr.instance.Load<Sprite>(worldTreeDungeonInfo.dungeonData.worldTreeRewardIconPath));
				//rewardIconImage.SetNativeSize();
				rewardIconImage.gameObject.SetActive(true);
			}
		}		
	}
}