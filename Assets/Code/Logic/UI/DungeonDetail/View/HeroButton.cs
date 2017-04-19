using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using Common.Localization;
using Logic.Hero.Model;
using Logic.Equipment.Model;

namespace Logic.UI.DungeonDetail.View
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

		private EquipmentInfo _equipInfo;
		public EquipmentInfo EquipInfo{
			get{
				return _equipInfo;
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
			heroIcon.SetSprite( ResMgr.instance.Load<Sprite>(_heroInfo.HeadIcon));
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
		public void SetEquipmentInfo(EquipmentInfo equipInfo){
			if(equipInfo == null)
				return ;
			_equipInfo = equipInfo;
//			heroIcon.sprite = ResMgr.instance.Load<Sprite>(ResPath.GetCharacterHeadIconPath(_equipInfo.HeadIcon));
//			heroNameText.text = Localization.Get(_equipInfo.equipmentData.name);
//			
//			int starCount = starImages.Length;
//			for (int i = 0; i < starCount; i++)
//			{
//				if (i < _equipInfo.advanceLevel)
//				{
//					starImages[i].gameObject.SetActive(true);
//				}
//				else
//				{
//					starImages[i].gameObject.SetActive(false);
//				}
//			}

		}
		public void SetSelect(bool isSelect){
			_isSelected = isSelect;
			selectedMarkGameObject.SetActive(isSelect);
		}

	}
}