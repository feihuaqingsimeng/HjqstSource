using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.UI.Description.View;
using Logic.Item.Model;
using Logic.Enums;
using Logic.Audio.Controller;


namespace Logic.UI.Description.View
{
    public class ItemDesButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {


        public static ItemDesButton Get(GameObject go)
        {
            ItemDesButton btn = go.GetComponent<ItemDesButton>();
            if (btn == null)
                btn = go.AddComponent<ItemDesButton>();
            return btn;
        }

        private CommonItemDesView _itemView;

        private ItemInfo _itemInfo;
        private float _showDelay;
        private ShowDescriptionType _type;
        public void SetItemInfo(ItemInfo data, ShowDescriptionType type = ShowDescriptionType.longPress, float showDelay = 0.15f)
        {
            _itemInfo = data;
            _showDelay = showDelay;
            _type = type;
        }
        public void SetItemInfo(int itemDataId, ShowDescriptionType type = ShowDescriptionType.longPress, float showDelay = 0.15f)
        {
            ItemInfo info = new ItemInfo(0, itemDataId, 0);
            SetItemInfo(info, type, showDelay);
        }
        public void SetType(ShowDescriptionType type)
        {
            _type = type;
        }
        void Awake()
        {

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_type != ShowDescriptionType.click)
            {
                if (_itemInfo == null)
                    return;

                StartCoroutine(showTipsCoroutine(_showDelay));
            }


        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_type != ShowDescriptionType.click)
            {
                StopAllCoroutines();
                if (_itemView != null)
                    _itemView.Close();
                _itemView = null;
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
            Debugger.Log("itemid:" + _itemInfo.itemData.id);
            yield return new WaitForSeconds(delay);
            RectTransform rectTran = transform as RectTransform;
            Vector2 sizeDelta = new Vector2(100, 100);
            if (rectTran != null)
                sizeDelta = (transform as RectTransform).sizeDelta;

            _itemView = CommonItemDesView.Open(_itemInfo, transform.position, sizeDelta);
        }
    }
}

