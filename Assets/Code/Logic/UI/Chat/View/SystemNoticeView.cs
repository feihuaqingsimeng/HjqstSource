using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.UI.Chat.Model;
using Common.ResMgr;

namespace Logic.UI.Chat.View
{
    public class SystemNoticeView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/chat/system_board_notice_view";
        public static SystemNoticeView Create(Transform parent)
        {
            SystemNoticeView view = Instantiate<GameObject>(ResMgr.instance.Load<GameObject>(PREFAB_PATH)).GetComponent<SystemNoticeView>();
            view.transform.SetParent(parent, false);
			view.transform.localPosition = Vector3.zero;
            return view;
        }


        #region ui component
        public Text noticeText;
        public GameObject root;
        #endregion
		
        Queue<string> SystemNoticeQueue = new Queue<string>();
        private RectTransform _noticeRt;
        private int _showState;
        private Vector2 _sizeDelta;
		void Awake()
		{
			root.SetActive(false);
		}

        void Start()
        {

            _noticeRt = noticeText.rectTransform;
            _sizeDelta = _noticeRt.sizeDelta;
            noticeText.text = string.Empty;
            root.SetActive(SystemNoticeProxy.instance.isStartNotice);
			if(SystemNoticeProxy.instance.isStartNotice)
			{
				UpdateSystemNotice(SystemNoticeProxy.instance.positionPercent,SystemNoticeProxy.instance.currentString);
			}
            SystemNoticeProxy.instance.onUpdateSystemNoticeDelegate += UpdateSystemNotice;
            SystemNoticeProxy.instance.onUpdateStartNoticeDelegate += UpdateStartNotice;
            SystemNoticeProxy.instance.onUpdateEndNoticeDelegate += UpdateEndNotice;
        }
        void OnDestroy()
        {
            SystemNoticeProxy.instance.onUpdateSystemNoticeDelegate -= UpdateSystemNotice;
            SystemNoticeProxy.instance.onUpdateStartNoticeDelegate -= UpdateStartNotice;
            SystemNoticeProxy.instance.onUpdateEndNoticeDelegate -= UpdateEndNotice;
			
        }
        private void UpdateStartNotice(string str)
        {
			if(!root.activeSelf)
			{
				RectTransform rootRt = root.GetComponent<RectTransform>();
				rootRt.localPosition = new Vector3(0,rootRt.sizeDelta.y,0);
				LeanTween.moveLocalY(root,0,0.21f).onComplete = ()=>{
					LeanTween.moveLocalY(root,2,0.05f).setLoopPingPong(1);
				};
			}
            root.SetActive(true);
        }
        private void UpdateEndNotice()
        {
            root.SetActive(false);
        }
		private float length = 0;
        private void UpdateSystemNotice(float t, string s)
        {
			if(noticeText.text != s)
			{
				noticeText.text = s;
				length = noticeText.preferredWidth;
				length = length <= _sizeDelta.x ? 0 : length-_sizeDelta.x;
				SystemNoticeProxy.instance.IgnoreMoveOutTime(length == 0);
			}
			if(t < 0 )
			{
				_noticeRt.localPosition = new Vector3(length * t, 0, 0);
			}else
			{
				_noticeRt.localPosition = new Vector3(_sizeDelta.x * t, 0, 0);
				
			}

        }
		public void ClickCloseBtnHandler()
		{
			SystemNoticeProxy.instance.Clear();
		}

    }
}

