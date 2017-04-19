using UnityEngine;
using UnityEngine.UI;
using Logic.Game.Model;
using Logic.UI.CommonReward.View;
using Common.ResMgr;
using Common.Localization;
using Logic.Item.Model;

namespace Logic.UI.DailyDungeon.View
{
	public class DailyDungeonRewardCard : MonoBehaviour
	{
//		private bool _hasDraw = false;
//		public bool HasDraw
//		{
//			get
//			{
//				return _hasDraw;
//			}
//		}
//
//		private Image _image;
//
//		private Sprite _frontSprite;
//		public Sprite FrontSprite
//		{
//			get
//			{
//				if (_frontSprite == null)
//					_frontSprite = ResMgr.instance.LoadSprite("sprite/main_ui/bg_common_05");
//				return _frontSprite;
//
//			}
//		}
//
//		private Sprite _backSprite;
//		public Sprite BackSprite
//		{
//			get
//			{
//				if (_backSprite == null)
//					_backSprite = ResMgr.instance.LoadSprite("sprite/main_ui/daily_dungeon_reward_card_back");
//				return _backSprite;
//			}
//		}
//
//		public CommonRewardIcon commonRewardIcon;
//
//		void Awake ()
//		{
//			_image = GetComponent<Image>();
//			_image.sprite = BackSprite;
//			commonRewardIcon.transform.localScale = new Vector3(0, 1, 1);
//		}
//
//		public void TurnOver (GameResData gameResData)
//		{
//			commonRewardIcon.SetGameResData(gameResData);
//			LTDescr ltDescr = LeanTween.scaleX(gameObject, 0, 0.25f);
//			ltDescr.setOnComplete(OnScaleDownComplete);
//			_hasDraw = true;
//		}
//
//		private void OnScaleDownComplete ()
//		{
//			_image.sprite = FrontSprite;
//			LeanTween.scaleX(gameObject, 1, 0.25f);
//			LeanTween.scaleX(commonRewardIcon.gameObject, 1, 0.25f);
//		}\

		public Text nameText;
		public CommonRewardIcon commonRewardIcon;

		public void SetGameResData (GameResData gameResData)
		{
			ItemData itemData = null;
			if (gameResData.type == Logic.Enums.BaseResType.Item)
			{
				itemData = ItemData.GetItemDataByID(gameResData.id);
			}
			else
			{
				itemData = ItemData.GetBasicResItemByType(gameResData.type);
			}
			commonRewardIcon.SetGameResData(gameResData);
			nameText.text = UI.UIUtil.FormatStringWithinQualityColor((Enums.RoleQuality)(itemData.itemQuality) ,Localization.Get(itemData.name));
		}
	}
}