using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.FunctionOpen.Model;
using Observers.Interfaces;
using Common.Components;
using Logic.Game.Model;
using Logic.Enums;
using Logic.UI.SoftGuide.View;

namespace Logic.UI.SoftGuide.Model
{
    public class SoftGuideProxy : SingletonMono<SoftGuideProxy>
    {
        void Awake()
        {
            instance = this;
        }



        public System.Action FunctionOpenDelegate;

        private Dictionary<int, SoftGuideInfo> _guideInfoDictionary = new Dictionary<int, SoftGuideInfo>();//未开放数据

        private List<SoftGuideInfo> _currentGuideList = new List<SoftGuideInfo>();//光圈提示
        private Dictionary<int, bool> _functionOpenDic = new Dictionary<int, bool>();//功能开放tip提示(bool 是否已提示)

		
        private Dictionary<string, System.Func<INotification, bool>> _funcResisterDic = new Dictionary<string, System.Func<INotification, bool>>();
        private bool _isInit = false;
        public void Init()
        {
            if (_isInit)
                return;
            _isInit = true;
            _funcResisterDic.Clear();
            _guideInfoDictionary.Clear();
            _currentGuideList.Clear();
            _functionOpenDic.Clear();
            Dictionary<int, FunctionData> dataDic = FunctionData.FunctionDataDictionary;
            SoftGuideInfo info;
            foreach (var value in dataDic)
            {
                if (!FunctionOpenProxy.instance.IsFunctionOpen((FunctionOpenType)value.Key))
                {
                    info = new SoftGuideInfo(value.Key, false);

                    if (info.hasFirstView)
                    {
                        RegisterObserver(info.firstViewPath);
                        _guideInfoDictionary.Add(value.Key, info);
                    }
                    if (info.hasSecondView)
                    {
                        RegisterObserver(info.SecondViewPath);
                    }
                    if (!string.IsNullOrEmpty(info.data.show_new_des))
                    {
                        _functionOpenDic.Add(value.Key, false);
                    }
                }

            }
            Observers.Facade.Instance.RegisterObserver("UpdateSoftGuideByLua", UpdateSoftGuideByLua);
        }
        private void RegisterObserver(string path)
        {
            if (!_funcResisterDic.ContainsKey(path))
            {
                Observers.Facade.Instance.RegisterObserver(path, Observer_ViewOpen_handler);
                _funcResisterDic.Add(path, Observer_ViewOpen_handler);
            }

        }

        public bool UpdateSoftGuideByLua(INotification note)
        {
            UpdateSoftGuide();
            return true;
        }

        public void UpdateSoftGuide()
        {
            RemoveTipOverInCurrentList();
            int lv = GameProxy.instance.AccountLevel;
            SoftGuideInfo info;
            bool isOpen = false;
            foreach (var value in _guideInfoDictionary)
            {
                info = value.Value;
                if (!info.isTipOver && FunctionOpenProxy.instance.IsFunctionOpen((FunctionOpenType)info.id))
                {
                    AddNewInfoInCurrentList(info);
                    isOpen = true;
					if(info.data.show_animation_status == 2)
						FunctionOpenProxy.instance.AddFunctionAnimationTipId(info.id);
                }
            }

            //总列表中移除当前开放的提示
            for (int i = 0, count = _currentGuideList.Count; i < count; i++)
            {
                info = _currentGuideList[i];

                _guideInfoDictionary.Remove(info.id);

            }
            UpdateEffectTips();
            if (isOpen && FunctionOpenDelegate != null)
            {
                FunctionOpenDelegate();
            }
            //提示功能开放界面
            List<int> openList = new List<int>();
            foreach (var value in _functionOpenDic)
            {
                if (!value.Value && FunctionOpenProxy.instance.IsFunctionOpen((FunctionOpenType)value.Key))
                {
					FunctionOpenProxy.instance.AddFunctionViewTipId(value.Key);
                    openList.Add(value.Key);
                }
            }
            for (int i = 0, count = openList.Count; i < count; i++)
            {
                _functionOpenDic[openList[i]] = true;
            }
        }
        private void UpdateEffectTips()
        {
            SoftGuideInfo info;
            GameObject go;
            for (int i = 0, count = _currentGuideList.Count; i < count; i++)
            {
                info = _currentGuideList[i];
                go = UIMgr.instance.Get(info.firstViewPath);
                if (go != null && !info.isTipOver)
                {
					CreateSoftEffectTip(go.transform.Find(info.firstButtonPath) as RectTransform, info.id, SoftGuideView.StatusType.FirstView);
                }
                go = UIMgr.instance.Get(info.SecondViewPath);
                if (go != null && !info.isTipOver)
                {
					CreateSoftEffectTip(go.transform.Find(info.SecondButtonPath) as RectTransform, info.id, SoftGuideView.StatusType.SecondView);
                }
            }

        }
        private void RemoveTipOverInCurrentList()
        {
            List<SoftGuideInfo> removeList = new List<SoftGuideInfo>();
            SoftGuideInfo info;

            for (int i = 0, count = _currentGuideList.Count; i < count; i++)
            {
                info = _currentGuideList[i];
                if (info.isTipOver)
                    removeList.Add(info);
            }
            for (int i = 0, count2 = removeList.Count; i < count2; i++)
            {
                _currentGuideList.Remove(removeList[i]);
            }
        }
        private void AddNewInfoInCurrentList(SoftGuideInfo info)
        {
            SoftGuideInfo curInfo;
            bool contain = false;
            for (int i = 0, count = _currentGuideList.Count; i < count; i++)
            {
                curInfo = _currentGuideList[i];
                if (curInfo.id == info.id)
                {
                    contain = true;
                    break;
                }
            }

            if (!contain)
            {
                if (info.data.id == (int)Logic.Enums.FunctionOpenType.MainView_Activity)
					LuaInterface.ToLuaPb.SetFromLua((int)Logic.Protocol.Model.MSG.ActivityListReq);
				if (info.data.id == (int)FunctionOpenType.MainView_Chat)
					LuaInterface.ToLuaPb.SetFromLua((int)Logic.Protocol.Model.MSG.ChatInfoReq);
                _currentGuideList.Add(info);
            }
        }
        public void OnClickSoftGuideView(SoftGuideView view)
        {
           // Debug.Log(view.id + "," + view.status);
            SoftGuideInfo info = null;
            for (int i = 0, count = _currentGuideList.Count; i < count; i++)
            {
                if (_currentGuideList[i].id == view.id)
                {
                    info = _currentGuideList[i];
                    break;
                }
            }
            if (info != null)
            {

				if (view.status == SoftGuideView.StatusType.FirstView)//一级界面
                {
                    if (!info.hasSecondView)
                    {
                        info.isTipOver = true;
                        bool hasSame = CheckFirstSame(info);
                        if (!hasSame)
                        {
                            RemoveSoftEffectTip(view.transform);
                        }
                    }

                }
				else if (view.status == SoftGuideView.StatusType.SecondView)//二级界面
                {
                    info.isTipOver = true;
                    bool hasSame = CheckSecondSame(info);
                    if (!hasSame)
                    {
                        RemoveSoftEffectTip(view.transform);
                    }
                    hasSame = CheckFirstSame(info);
                    if (!hasSame)
                    {
                        GameObject firstView = UIMgr.instance.Get(info.firstViewPath);
                        if (firstView != null)
                        {
                            RemoveSoftEffectTip(firstView.transform.Find(info.firstButtonPath));
                        }

                    }
                }
            }
        }
        /// <summary>
        /// 检查一级界面相同，ignoreSecond：是否忽略二级，true：不管二级界面，false:必须要有二级
        /// </summary>

        private bool CheckFirstSame(SoftGuideInfo info)
        {
            int count = _currentGuideList.Count;
            SoftGuideInfo curInfo;
            for (int i = 0; i < count; i++)
            {
                curInfo = _currentGuideList[i];
                if (!curInfo.isTipOver)
                {
                    if (curInfo.hasFirstView && info.firstViewPath.Equals(curInfo.firstViewPath) && info.firstButtonPath.Equals(curInfo.firstButtonPath))
                    {
                        if (curInfo.hasSecondView)
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }
        /// <summary>
        /// 检查二级界面相同
        /// </summary>
        private bool CheckSecondSame(SoftGuideInfo info)
        {
            int count = _currentGuideList.Count;
            SoftGuideInfo curInfo;
            for (int i = 0; i < count; i++)
            {
                curInfo = _currentGuideList[i];
                if (!curInfo.isTipOver)
                {
                    if (curInfo.hasSecondView && info.SecondViewPath.Equals(curInfo.SecondViewPath) && info.SecondButtonPath.Equals(curInfo.SecondButtonPath))
                    {
                        return true;
                    }

                }
            }
            return false;
        }
		private void CreateSoftEffectTip(RectTransform t, int id, SoftGuideView.StatusType status)
        {
            if (t == null)
                return;
            SoftGuideView[] views = t.GetComponentsInChildren<SoftGuideView>(true);

            if (views.Length == 0)
            {
                SoftGuideView view = SoftGuideView.Create(t, id, status);
                RectTransform rt = view.rectTransform;
                rt.sizeDelta = t.sizeDelta;
                Canvas canvas = view.GetComponentsInParent<Canvas>(true)[0];
                rt.anchoredPosition3D = new Vector3(0, 0, canvas.planeDistance);
                Renderer[] orderChange = view.GetComponentsInChildren<Renderer>(true);
                Renderer particleRender;
                for (int i = 0, count = orderChange.Length; i < count; i++)
                {
                    particleRender = orderChange[i];
                    particleRender.sortingLayerName = canvas.sortingLayerName;
                    particleRender.sortingOrder += canvas.sortingOrder;
                }
            }
            else
            {
                for (int i = 0, count = views.Length; i < count; i++)
                {
                    views[i].SetStatus(status);
                }
            }
        }
        private void RemoveSoftEffectTip(Transform t)
        {
            if (t == null)
                return;
            SoftGuideView[] view = t.GetComponentsInChildren<SoftGuideView>(true);
            if (view != null && view.Length > 0)
            {
                GameObject.DestroyImmediate(view[0].gameObject);
            }
        }
        private bool Observer_ViewOpen_handler(Observers.Interfaces.INotification note)
        {
            if ("open".Equals(note.Type))
            {
                
				StartCoroutine(HandleOpenViewCoroutine(note));
            }
            else if ("close".Equals(note.Type) && note.Name.Equals("ui/main/main_view"))
            {
                FunctionOpenView.Close();
            }
            return true;
        }
		//status 1 一级界面 2 二级界面 3 锁住 4 隐藏光圈
		private IEnumerator HandleOpenViewCoroutine(Observers.Interfaces.INotification note)
		{
			yield return null;
			SoftGuideInfo info;
			Transform tran = (note.Body as GameObject).transform;
			for (int i = 0, count = _currentGuideList.Count; i < count; i++)
			{
				info = _currentGuideList[i];
				if (note.Name.Equals(info.firstViewPath) && !info.isTipOver)
				{
					
					CreateSoftEffectTip(tran.Find(info.firstButtonPath) as RectTransform, info.id,SoftGuideView.StatusType.FirstView);
					
				}
				else if (note.Name.Equals(info.SecondViewPath) && !info.isTipOver)
				{
					CreateSoftEffectTip(tran.Find(info.SecondButtonPath) as RectTransform, info.id,SoftGuideView.StatusType.SecondView );
				}
			}
			foreach (var value in _guideInfoDictionary)
			{
				if (note.Name.Equals(value.Value.firstViewPath) && !value.Value.hasSecondView)
				{
					CreateSoftEffectTip(tran.Find(value.Value.firstButtonPath) as RectTransform, value.Value.id, SoftGuideView.StatusType.Lock);
					// Debugger.Log("[Soft Guide]lock button :{0},functionId:{1}", value.Value.firstButtonPath, value.Value.id);
				}
				else if (note.Name.Equals(value.Value.SecondViewPath))
				{
					CreateSoftEffectTip(tran.Find(value.Value.SecondButtonPath) as RectTransform, value.Value.id, SoftGuideView.StatusType.Lock);
					//Debugger.Log("[Soft Guide]lock button :{0},functionId:{1}", value.Value.SecondButtonPath, value.Value.id);
				}
			}
		}
    }

}
