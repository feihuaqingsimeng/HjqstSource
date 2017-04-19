using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.ResMgr;
using Common.Util;
using LuaInterface;

namespace Logic.UI
{
    [NoToLua]
    public enum EUISortingLayer
    {
        MainUI = 0,
        Tips = 1,
        Notice = 2,
        Select = 3,
        FlyWord = 4,
		Tutorial = 5,
        Loading = 6,
		TopTips = 7,
    }

    [NoToLua]
    public enum UIOpenMode
    {
        Replace = 0,
        Overlay = 1,
    }

    public class UIMgr : SingletonMono<UIMgr>
    {
        [NoToLua]
        public const string UI_MAX_LAYER_CHANGE = "UI_MAX_LAYER_CHANGE";
        [NoToLua]
        public const int SORT_ORDER_INTERVAL = 100;
        [NoToLua]
        private Dictionary<string, GameObject> _uiDic;
        [NoToLua]
        private Dictionary<EUISortingLayer, List<string>> _layerDic;
        [NoToLua]
        private Dictionary<GameObject, EUISortingLayer> _goLayerDic;
        [NoToLua]
        private Dictionary<EUISortingLayer, Dictionary<string, UIOpenMode>> _layerUIOpenModeDic;

        [NoToLua]
        private string _lastOpenedUIPath;
        [NoToLua]
        public string LastOpenedUIPath
        {
            get
            {
                return _lastOpenedUIPath;
            }
        }

        [NoToLua]
        public delegate void OnUIViewOpenDelegate(string uiPath);
        [NoToLua]
        public OnUIViewOpenDelegate onUIViewOpenDelegate;
        public Vector2 designResolution;
        [NoToLua]
        public Camera uiCamera;
        [NoToLua]
        public Transform ui2DRoot;
        public Transform ui3DRoot;
        [NoToLua]
        public Canvas basicCanvas;

        [NoToLua]
        void Awake()
        {
            instance = this;
            _uiDic = new Dictionary<string, GameObject>();
            _layerDic = new Dictionary<EUISortingLayer, List<string>>();
            _goLayerDic = new Dictionary<GameObject, EUISortingLayer>();
            _layerUIOpenModeDic = new Dictionary<EUISortingLayer, Dictionary<string, UIOpenMode>>();
        }

        public Transform GetParticularUITransform(string uiViewPrefabPath, string subPath)
        {
            Transform particularUITransform = null;
            GameObject viewGameObject = null;
            if (_uiDic.TryGetValue(uiViewPrefabPath, out viewGameObject))
            {
                particularUITransform = viewGameObject.transform.FindChild(subPath);
            }
            return particularUITransform;
        }

        [NoToLua]
        private int GetTopSortingOrderAtLayer(EUISortingLayer uiSortingLayer)
        {
            int result = 0;
            List<string> layerUIPathList = null;
            if (_layerDic.TryGetValue(uiSortingLayer, out layerUIPathList))
            {
                if (layerUIPathList.Count > 0)
                {
                    Canvas canvas = _uiDic[layerUIPathList.Last()].GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        result += canvas.sortingOrder;
                    }
                }
            }
            result += SORT_ORDER_INTERVAL;
            return result;
        }

        [NoToLua]
        private float GetTopPlaneDistance(EUISortingLayer uiSortingLayer)
        {
            float topPlaneDistance = 0;
            List<string> layerUIPathList = null;
            if (_layerDic.TryGetValue(uiSortingLayer, out layerUIPathList))
            {
                if (layerUIPathList.Count > 0)
                {
                    Canvas canvas = _uiDic[layerUIPathList.Last()].GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        if (topPlaneDistance > canvas.planeDistance)
                            topPlaneDistance = canvas.planeDistance;
                    }
                }
                else
                {
                    topPlaneDistance = (int)uiSortingLayer * -1000;
                }

            }
            else
            {
                topPlaneDistance = (int)uiSortingLayer * -1000;
            }
            topPlaneDistance -= 10;
            return topPlaneDistance;
        }

        [NoToLua]
        private void SetLayerAndOrder(GameObject target, EUISortingLayer uiSortingLayer, int sortingOrder, float planeDistance)
        {
            Canvas[] canvases = target.GetComponentsInChildren<Canvas>(true);
            Canvas canvas = null;
            for (int index = 0, count = canvases.Length; index < count; index++)
            {
                canvas = canvases[index];
                canvas.sortingLayerName = uiSortingLayer.ToString();
                canvas.sortingOrder = sortingOrder + canvas.sortingOrder;
            }

            Common.Components.SortingOrderChanger[] sortingOrderChangers = target.GetComponentsInChildren<Common.Components.SortingOrderChanger>(true);
            Common.Components.SortingOrderChanger sortingOrderChanger = null;
            for (int index = 0, count = sortingOrderChangers.Length; index < count; index++)
            {
                sortingOrderChanger = sortingOrderChangers[index];
                sortingOrderChanger.sortingOrder += sortingOrder;
            }

            Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
            Renderer renderer = null;
            for (int index = 0, count = renderers.Length; index < count; index++)
            {
                renderer = renderers[index];
                renderer.sortingLayerName = uiSortingLayer.ToString();
                renderer.sortingOrder = sortingOrder + renderer.sortingOrder;
            }

            Canvas targetCanvas = target.GetComponent<Canvas>();
            if (targetCanvas != null)
            {
                targetCanvas.overrideSorting = true;
                targetCanvas.sortingLayerName = uiSortingLayer.ToString();
                targetCanvas.sortingOrder = sortingOrder;
                //				targetCanvas.transform.localPosition = new Vector3(targetCanvas.transform.localPosition.x, targetCanvas.transform.localPosition.y, z);
                targetCanvas.planeDistance = planeDistance;
            }
        }

        [NoToLua]
        private void RecalulateMainUIlayer()
        {
            List<string> mainUILayerUIPathList = null;
            Dictionary<string, UIOpenMode> mainUILayerUIOpenModeDic = null;

            if (!_layerDic.TryGetValue(EUISortingLayer.MainUI, out mainUILayerUIPathList))
                return;
            if (mainUILayerUIPathList.Count <= 0)
                return;
            if (!_layerUIOpenModeDic.TryGetValue(EUISortingLayer.MainUI, out mainUILayerUIOpenModeDic))
                return;

            int layerTypeValue = (int)Enums.LayerType.UI;
            UIOpenMode uiOpenMode = UIOpenMode.Overlay;
            string uiPath = null;
            for (int index = mainUILayerUIPathList.Count - 1; index >= 0; index--)
            {
                uiPath = mainUILayerUIPathList[index];
                if (_uiDic[uiPath].gameObject.layer != layerTypeValue)
                {
                    TransformUtil.SwitchLayer(_uiDic[uiPath].transform, layerTypeValue);
                }
                if (mainUILayerUIOpenModeDic.TryGetValue(uiPath, out uiOpenMode))
                {
                    if (uiOpenMode == UIOpenMode.Replace)
                    {
                        layerTypeValue = (int)Enums.LayerType.UIHide;
                    }
                }
            }
            _lastOpenedUIPath = mainUILayerUIPathList.Last<string>();
            if (onUIViewOpenDelegate != null)
                onUIViewOpenDelegate(_lastOpenedUIPath);
        }
        public GameObject Open(string uiPath, int uiSortingLayer, int uiOpenMode)
        {
            return Open(uiPath, (EUISortingLayer)uiSortingLayer, (UIOpenMode)uiOpenMode);
        }
        [NoToLua]
        public GameObject Open(string uiPath, EUISortingLayer uiSortingLayer = EUISortingLayer.MainUI, UIOpenMode uiOpenMode = UIOpenMode.Replace)
        {
            GameObject result = null;

            if (!_uiDic.TryGetValue(uiPath, out result))
            {
                GameObject prefab = ResMgr.instance.Load<GameObject>(uiPath);
                if (prefab == null)
                    return null;
                result = GameObject.Instantiate(prefab);
                result.transform.SetParent(ui2DRoot, false);
                result.name = uiPath;

                RectTransform rectTransform = result.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector3.one;
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.localScale = Vector3.one;
                }
            }
            SetLayerAndOrder(result, uiSortingLayer, GetTopSortingOrderAtLayer(uiSortingLayer), GetTopPlaneDistance(uiSortingLayer));

            _uiDic.AddOrReplace(uiPath, result);
            _goLayerDic.AddOrReplace(result, uiSortingLayer);

            if (!_layerDic.ContainsKey(uiSortingLayer))
                _layerDic[uiSortingLayer] = new List<string>();
            _layerDic[uiSortingLayer].Remove(uiPath);
            _layerDic[uiSortingLayer].Add(uiPath);

            if (!_layerUIOpenModeDic.ContainsKey(uiSortingLayer))
                _layerUIOpenModeDic[uiSortingLayer] = new Dictionary<string, UIOpenMode>();
            _layerUIOpenModeDic[uiSortingLayer].AddOrReplace(uiPath, uiOpenMode);

            Observers.Facade.Instance.SendNotification(uiPath, result, "open");
			Observers.Facade.Instance.SendNotification("OpenUIView", uiPath, "open");

            if (uiSortingLayer == EUISortingLayer.MainUI)
                RecalulateMainUIlayer();
            return result;
        }

        [NoToLua]
        public T Open<T>(string uiPath, EUISortingLayer uiSortingLayer = EUISortingLayer.MainUI, UIOpenMode uiOpenMode = UIOpenMode.Replace) where T : MonoBehaviour
        {
            GameObject go = Open(uiPath, uiSortingLayer, uiOpenMode);
            return go.GetComponent<T>();
        }

        public bool IsOpening(string uiPath)
        {
            return _uiDic.ContainsKey(uiPath);
        }

        // try 3D exhibition
        public GameObject OpenExhibition(string uiPath)
        {
            GameObject result = null;

            // hide other UI panels
            List<GameObject> uiGOList = _uiDic.GetValues();
            for (int i = 0, uiGOCount = uiGOList.Count; i < uiGOCount; i++)
            {
                TransformUtil.SwitchLayer(uiGOList[i].transform, (int)Enums.LayerType.UIHide);
            }

            result = Open(uiPath);
            return result;
        }

        [NoToLua]
        public T OpenExhibition<T>(string uiPath)
        {
            GameObject go = OpenExhibition(uiPath);
            return go.GetComponent<T>();
        }

        public void CloseExhibition(string uiPath)
        {
            // remove UI 3D scene
            TransformUtil.ClearChildren(ui3DRoot, true);

            Close(uiPath, true);

            // show other UI panels
            List<GameObject> uiGOList = _uiDic.GetValues();
            for (int i = 0, uiGOCount = uiGOList.Count; i < uiGOCount; i++)
            {
                TransformUtil.SwitchLayer(uiGOList[i].transform, (int)Enums.LayerType.UI);
            }
        }
        // try 3D exhibition

        [NoToLua]
        public T Get<T>(string uiPath) where T : MonoBehaviour
        {
            if (!_uiDic.ContainsKey(uiPath))
                return default(T);
            return _uiDic[uiPath].GetComponent<T>();
        }

        public GameObject Get(string uiPath)
        {
            if (!_uiDic.ContainsKey(uiPath))
                return null;
            return _uiDic[uiPath];
        }

		public int GetOpenUICount(EUISortingLayer layer = EUISortingLayer.MainUI)
		{
			int count = 0;
			foreach(var value in _uiDic)
			{
				if(_goLayerDic[value.Value] == layer)
				{
					count++;
				}
			}
			return count;
		}

        public void CloseAll()
        {
			Dictionary<string, GameObject> uiDic = new Dictionary<string, GameObject>(_uiDic);
			foreach (var kv in uiDic)
            {
                UnityEngine.Object.Destroy(kv.Value);
                LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("uimanager.CloseView", kv.Key);
                Observers.Facade.Instance.SendNotification(kv.Key, null, "close");
                Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", kv.Key, "OnViewClose"));
				Observers.Facade.Instance.SendNotification("CloseUIView", kv.Key, "close");
            }
            _uiDic.Clear();
            _layerDic.Clear();
            _goLayerDic.Clear();
            _layerUIOpenModeDic.Clear();
        }

        public void Close(string uiPath)
        {
            Close(uiPath, false);
        }

		public void CloseImmediate (string uiPath)
		{
			Close(uiPath, true);
		}

        //暂时不公开
        [NoToLua]
        private void Close(string uiPath, bool immediate)
        {
            GameObject go;
            if (!_uiDic.TryGetValue(uiPath, out go))
                return;
            _uiDic.Remove(uiPath);

            if (_goLayerDic.ContainsKey(go))
            {
                _goLayerDic.Remove(go);
            }
            foreach (var kv in _layerDic)
            {
                if (kv.Value.Contains(uiPath))
                {
                    kv.Value.Remove(uiPath);
                    break;
                }
            }

            EUISortingLayer uiSortingLayer = EUISortingLayer.MainUI;
            if (_goLayerDic.TryGetValue(go, out uiSortingLayer))
            {
                Dictionary<string, UIOpenMode> openModeDic = null;
                if (_layerUIOpenModeDic.TryGetValue(uiSortingLayer, out openModeDic))
                {
                    openModeDic.TryDelete(uiPath);
                }
            }

            if (immediate)
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
            else
            {
                UnityEngine.Object.Destroy(go);
            }

            if (uiSortingLayer == EUISortingLayer.MainUI)
                RecalulateMainUIlayer();
            if (LuaInterface.LuaScriptMgr.Instance != null)
                LuaInterface.LuaScriptMgr.Instance.CallLuaFunction("uimanager.CloseView", uiPath);
            Observers.Facade.Instance.SendNotification(uiPath, null, "close");
            Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", uiPath, "OnViewClose"));
			Observers.Facade.Instance.SendNotification("CloseUIView", uiPath, "close");
        }

        public void Close(EUISortingLayer uiSortingLayer)
        {
            List<string> uiPathList;
            if (_layerDic.TryGetValue(uiSortingLayer, out uiPathList))
            {
                List<string> uiPathListCopy = new List<string>(uiPathList);
                for (int index = 0, count = uiPathListCopy.Count; index < count; index++)
                {
                    Close(uiPathListCopy[index]);
                }
            }
        }

		public void CloseLayerBelow (EUISortingLayer uiSortingLayer)
		{
			for (int i = (int)EUISortingLayer.MainUI; i <= (int)uiSortingLayer; i++)
			{
				Close((EUISortingLayer)i);
			}
		}
    }
}