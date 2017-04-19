using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using Common.Localization;
using Logic.Hero.Model;

namespace Logic.UI.HeroCombine.View
{
	public class HeroButton : MonoBehaviour
	{
		public delegate bool HeroButtonSelect(Logic.UI.HeroCombine.View.HeroButton herobutton,bool isSelect);

		private HeroButtonSelect _heroButtonSelectDelegate ;
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
        public HeroButtonSelect HeroButtonSelectDelegate {
            get {
                return _heroButtonSelectDelegate;
            }
            set {
                _heroButtonSelectDelegate = value;
            }
        }
		#region UI components
		public Image heroIcon;
		public GameObject selectedMarkGameObject;
		public Text heroNameText;
		public Text text_level;
		public Text text_strengthenLevel;
		public Image[] starImages;
		#endregion

		void Awake ()
		{
			Init();
			
		}
        void Start() {
            
        }
		private void Init ()
		{
			SetSelect(false);
		}

		public void SetHeroInfo (HeroInfo heroInfo)
		{
            if (heroInfo == null)
                return;
			_heroInfo = heroInfo;
			heroIcon.SetSprite(ResMgr.instance.Load<Sprite>(_heroInfo.HeadIcon));
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
			text_level.text = _heroInfo.level.ToString();
			if(_heroInfo.strengthenLevel>0)
				text_strengthenLevel.text = string.Format("+{0}",_heroInfo.strengthenLevel.ToString());
			else
				text_strengthenLevel.text = string.Empty;
		}
		
		public void SetSelect(bool isSelect){
			_isSelected = isSelect;
			selectedMarkGameObject.SetActive(isSelect);
		}

		public void HideMark(){
			selectedMarkGameObject.SetActive(false);
		}

		public void ShowMark(){
			selectedMarkGameObject.SetActive(true);
		}

		public void HideLevel(){
			text_level.gameObject.SetActive(false);
			text_strengthenLevel.gameObject.SetActive(false);
		}
		public void ShowLevel(){
			text_level.gameObject.SetActive(true);
			text_strengthenLevel.gameObject.SetActive(true);
		}
	}
}