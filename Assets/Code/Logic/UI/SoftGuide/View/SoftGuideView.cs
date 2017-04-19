using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.ResMgr;
using UnityEngine.EventSystems;
using Logic.UI.SoftGuide.Model;
using Logic.FunctionOpen.Model;

namespace Logic.UI.SoftGuide.View
{
	public class SoftGuideView : MonoBehaviour
	{

		public enum StatusType
		{
			FirstView = 1,
			SecondView = 2,
			Lock = 3,
		}

		public const string PREFAB_PATH = "ui/soft_guide/soft_guide_view";

		public int id;
		public StatusType status;//1 一级界面 2 二级界面 3 锁住 4 隐藏光圈
		public GameObject goLock;
		public GameObject goEffectTip;
		public GameObject goLockSmall;
		public static SoftGuideView Create(Transform parent,int id,StatusType status)
		{
			GameObject go = Instantiate<GameObject>(ResMgr.instance.Load<GameObject>(PREFAB_PATH));
			go.transform.SetParent(parent,false);
			SoftGuideView view = go.GetComponent<SoftGuideView>();
			view.id = id;
			view.AddListener(parent);
			view.SetStatus(status);

			return view;
		}
		public void SetStatus(StatusType status)
		{
			this.status = status;
			Refresh();
		}
		private void Refresh()
		{
			FunctionData data = FunctionData.GetFunctionDataByID(id);
			//蛋疼的策划特殊处理
			SoftGuideInfo info = new SoftGuideInfo(id,false);
			if("ui/pve_embattle/pve_embattle_view_lua".Equals( info.firstViewPath))
			{
				goLock.SetActive(false);
				goLockSmall.SetActive(status == StatusType.Lock);
			}else
			{
				goLock.SetActive(status == StatusType.Lock);
				goLockSmall.SetActive(false);
			}

			if (data.is_show_light)
			{
				goEffectTip.SetActive(status != StatusType.Lock);
			}else
			{
				goEffectTip.SetActive(false);
			}
			//0 不处理 1 是否显示隐藏，2显隐and是否有动画（2为主界面专用）
			if (data.show_animation_status == 1)
			{
				transform.parent.gameObject.SetActive(status != StatusType.Lock);
			}
		}
		public RectTransform rectTransform
		{
			get
			{
				return transform as RectTransform;
			}
		}
		public void AddListener(Transform parent)
		{
			Button[] b = parent.GetComponentsInChildren<Button>(true);
			if(b!= null && b.Length > 0)
			{
				b[0].onClick.AddListener(OnButtonClick);
			}
			Toggle[] t = parent.GetComponentsInChildren<Toggle>(true);
			if(t!= null && t.Length > 0)
			{
				t[0].onValueChanged.AddListener(OnToggleClick);
			}
		}
		private void OnButtonClick()
		{

			SoftGuideProxy.instance.OnClickSoftGuideView(this);
		}
		private void OnToggleClick(bool value)
		{
			if(value)
				SoftGuideProxy.instance.OnClickSoftGuideView(this);
        }
	}
}

