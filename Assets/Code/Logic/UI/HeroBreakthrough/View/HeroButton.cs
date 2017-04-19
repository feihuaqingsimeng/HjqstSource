using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using Common.Localization;
using Logic.Hero.Model;

namespace Logic.UI.HeroBreakthrough.View
{
	public class HeroButton : MonoBehaviour
	{
		private HeroInfo _heroInfo;
		public HeroInfo HeroInfo
		{
			get
			{
				return _heroInfo;
			}
		}

		private bool _isSelected = false;
		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
		}

		#region UI components
		public Image heroIcon;
		public GameObject selectedMarkGameObject;
		public Text levelText;
		public Text strengthenLevelText;
		public Text heroNameText;
		public Image[] starImages;
		#endregion

		void Awake ()
		{
			Init();
		}

		private void Init ()
		{
			SetAsUnselected();
		}

		public void SetHeroInfo (HeroInfo heroInfo)
		{
			_heroInfo = heroInfo;
			heroIcon.SetSprite(ResMgr.instance.Load<Sprite>(_heroInfo.HeadIcon));
			if (levelText != null)
			{
				levelText.text = _heroInfo.level.ToString();
			}
			if (strengthenLevelText != null)
			{
				if (_heroInfo.strengthenLevel <= 0)
				{
					strengthenLevelText.enabled = false;
				}
				else
				{
					strengthenLevelText.text = string.Format(Localization.Get("common.strengthen_level"), _heroInfo.strengthenLevel);
					strengthenLevelText.enabled = true;
				}
			}
			heroNameText.text = Localization.Get(_heroInfo.heroData.name);

			int starCount = starImages.Length;
			for (int i = 0; i < starCount; i++)
			{
				if (i < _heroInfo.advanceLevel)
				{
					starImages[i].gameObject.SetActive(true);
				}
				else
				{
					starImages[i].gameObject.SetActive(false);
				}
			}
		}

		public void SetAsSelected ()
		{
			_isSelected = true;
			selectedMarkGameObject.SetActive(_isSelected);
		}

		public void SetAsUnselected ()
		{
			_isSelected = false;
			selectedMarkGameObject.SetActive(_isSelected);
		}
	}
}