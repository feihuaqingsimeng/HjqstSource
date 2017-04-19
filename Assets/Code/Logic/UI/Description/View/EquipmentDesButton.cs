using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.UI.Description.View;
using Logic.Equipment.Model;
using Logic.Enums;
using Logic.Audio.Controller;
using LuaInterface;


namespace Logic.UI.Description.View
{
	public class EquipmentDesButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerClickHandler
    {


        public static EquipmentDesButton Get(GameObject go)
        {
            EquipmentDesButton btn = go.GetComponent<EquipmentDesButton>();
            if (btn == null)
                btn = go.AddComponent<EquipmentDesButton>();
            return btn;
        }

        private CommonEquipDesView _equipView;
		private LuaTable equipDesBtnLua ;
        private EquipmentInfo _equipInfo;
        private float _showDelay;
        private ShowDescriptionType _type;
        public void SetEquipInfo(EquipmentInfo data, ShowDescriptionType type = ShowDescriptionType.longPress, float showDelay = 0.15f)
        {
            _equipInfo = data;
            _showDelay = showDelay;
            _type = type;
        }
        public void SetEquipInfo(int equipId, ShowDescriptionType type = ShowDescriptionType.longPress, float showDelay = 0.15f)
        {
            EquipmentInfo info = new EquipmentInfo(0, equipId, 0);
            SetEquipInfo(info, type, showDelay);
        }
        public void SetType(ShowDescriptionType type)
        {
            _type = type;
        }
        public void SetDelay(float showDelay)
        {
            _showDelay = showDelay;
        }
        void Awake()
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_type != ShowDescriptionType.click)
            {
                if (_equipInfo == null)
                    return;
                Debugger.Log("id:" + _equipInfo.instanceID + ",modelid:" + _equipInfo.equipmentData.id + ",equipType:" + _equipInfo.equipmentData.equipmentType + ",roleType:" + _equipInfo.equipmentData.equipmentRoleType);
                StartCoroutine(showTipsCoroutine(_showDelay));
            }


        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_type != ShowDescriptionType.click)
            {
                StopAllCoroutines();
                if (_equipView != null)
                    _equipView.Close();
                _equipView = null;
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_type == ShowDescriptionType.click)
            {
                StartCoroutine(showTipsCoroutine(0));
                AudioController.instance.PlayAudio(AudioController.CLICK, false);
            }
        }
        private IEnumerator showTipsCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            RectTransform rectTran = transform as RectTransform;
            Vector2 sizeDelta = new Vector2(100, 100);
            if (rectTran != null)
                sizeDelta = (transform as RectTransform).sizeDelta;

            _equipView = CommonEquipDesView.Open(_equipInfo, transform.position, sizeDelta);
        }
    }
}

