using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using Logic.Game.Model;
using Logic.Equipment.Model;
using Logic.UI.Description.View;
using Logic.Audio.Controller;
using Logic.Enums;

namespace Logic.UI.CommonEquipment.View
{
    public class CommonEquipmentIcon : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/common/common_equipment_icon";
        public const string PREFAB_SMALL_PATH = "ui/common/common_equipment_small_icon";

        public static CommonEquipmentIcon Create(Transform parent)
        {
            GameObject go = ResMgr.instance.Load<GameObject>(PREFAB_PATH);
            go = GameObject.Instantiate<GameObject>(go);
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = Vector3.zero;
            }
            return go.GetComponent<CommonEquipmentIcon>();
        }
        public static CommonEquipmentIcon CreateSmallIcon(Transform parent)
        {
            GameObject go = GameObject.Instantiate<GameObject>(ResMgr.instance.Load<GameObject>(PREFAB_SMALL_PATH));
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = Vector3.zero;
            }
            return go.GetComponent<CommonEquipmentIcon>();
        }
        private EquipmentInfo _equipmentInfo;
        public EquipmentInfo EquipmentInfo
        {
            get
            {
                return _equipmentInfo;
            }
        }

        public bool isSelect;


        public delegate void OnClickHandler(CommonEquipmentIcon icon);
        public OnClickHandler onClickHandler;

        #region UI components
        public RectTransform rectTransform;
        public Image equipmentIconImage;
        public Image equipmentRoleTypeIcon;
        public GameObject selectMark;
        public Text text_strengthen_level;
        #endregion UI components

        private Transform starRoot;
        private EquipmentDesButton _equipmentDesButton;
        void Awake()
        {
            SetSelect(false);
            _equipmentDesButton = EquipmentDesButton.Get(gameObject);
        }

        private void RefreshEquipmentIconImage()
        {
			equipmentIconImage.SetSprite(ResMgr.instance.Load<Sprite>(ResPath.GetEquipmentIconPath(_equipmentInfo.equipmentData.icon)));
        }

        public void Refresh()
        {
			Image frame = transform.GetComponent<Image>();
			if(frame!= null)
			{
				frame.SetSprite(UIUtil.GetRoleQualityFrameSprite((RoleQuality)_equipmentInfo.equipmentData.quality));
			}
            RefreshEquipmentIconImage();
            equipmentRoleTypeIcon.SetSprite( UIUtil.GetRoleTypeSmallIconSprite(_equipmentInfo.equipmentData.equipmentRoleType));
			text_strengthen_level.text = "";
            if (starRoot == null)
            {
                starRoot = rectTransform.Find("star_root");
            }
            if (starRoot != null)
            {
                for (int i = 0, count = starRoot.childCount; i < count; i++)
                {
                    starRoot.GetChild(i).gameObject.SetActive(i < _equipmentInfo.star);
                }
            }
            Transform strengthenLv = rectTransform.Find("text_strengthen_level");
            if (strengthenLv != null)
            {
				if (_equipmentInfo.strengthenLevel > 0)
					strengthenLv.GetComponent<Text>().text = string.Format("+{0}", _equipmentInfo.strengthenLevel);
                else
                    strengthenLv.GetComponent<Text>().text = string.Empty;
            }
        }

        public void SetEquipmentInfo(EquipmentInfo equipmentInfo)
        {
            _equipmentInfo = equipmentInfo;
            Refresh();
            _equipmentDesButton.SetEquipInfo(_equipmentInfo);
        }

        public void SetGameResData(GameResData gameResData)
        {
            SetEquipmentInfo(new EquipmentInfo(0, gameResData.id, 0));
        }
        public EquipmentDesButton GetEquipmentDesButton()
        {
            return _equipmentDesButton;
        }
        public void ButtonEnable(bool value)
        {
            Button btn = transform.GetComponent<Button>();
            if (btn != null)
            {
                btn.enabled = value;
            }
        }
        public void SetSelect(bool select)
        {
            isSelect = select;
            selectMark.SetActive(select);
        }

        public void ShowSelect()
        {
            selectMark.SetActive(true);
        }
        public void HideSelect()
        {
            selectMark.SetActive(false);
        }
        public void SetLevelColor(Color color)
        {
            text_strengthen_level.color = color;
        }
        #region ui event handlers
        public void ClickHandler(CommonEquipmentIcon icon)
        {
            if (onClickHandler != null)
            {
                onClickHandler(icon);
                AudioController.instance.PlayAudio(AudioController.SELECT, false);
            }
        }
        #endregion ui event handlers
    }
}