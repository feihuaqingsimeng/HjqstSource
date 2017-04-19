using UnityEngine;
using System.Collections;
using Common.ResMgr;
using Logic.Map.View;
using Logic.Game.Controller;
using Logic.Game;
using Common.Components.Shadow;
namespace Logic.Map.Controller
{
    public class MapController : SingletonMono<MapController>
    {
        private MapView mapView;
        private float _delay;
        public SpriteRenderer mapMaskSpriteRenderer;
        private bool _load = false;
        public Transform effectsParent;
        private GameObject _comboMapEffect;
        //当前第几关卡
        public uint level { get; set; }
        void Awake()
        {
            instance = this;
            fighting = false;
            ShowMapMask(false);
        }

        public bool fighting
        {
            set
            {
                this.enabled = value;
                if (value)
                    LoadMaskAndEffect();
            }
        }

        private void LoadMaskAndEffect()
        {
            if (!_load)
            {
                _load = true;
                mapMaskSpriteRenderer.sprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/mask1");
            }
            if (_comboMapEffect) return;
            GameObject go = ResMgr.instance.Load<GameObject>("effects/prefabs/beijing_effect_lianji");
            if (go)
            {
                _comboMapEffect = GameObject.Instantiate(go);
                _comboMapEffect.transform.SetParent(transform);
                _comboMapEffect.transform.localPosition = new Vector3(0f, 0f, -15f);
                Common.Util.TransformUtil.SwitchLayer(_comboMapEffect.transform, (int)Logic.Enums.LayerType.Scene);
                _comboMapEffect.SetActive(false);
            }
        }

        public void CreateMap(string path)
        {
            if (mapView)
                UnityEngine.Object.Destroy(mapView.gameObject);
            ShowMapMask(false);
            GameObject prefab = ResMgr.instance.Load<GameObject>("map/" + path);
            GameObject mapGO = UnityEngine.Object.Instantiate(prefab) as GameObject;
            mapView = mapGO.GetComponent<MapView>();
            mapView.alphaChangeHandler += SetMapMaskAlpha;
            mapView.transform.SetParent(this.transform, false);
        }

        public void ClearMap()
        {
            if (mapView)
            {
                mapView.alphaChangeHandler -= SetMapMaskAlpha;
                UnityEngine.Object.Destroy(mapView.gameObject);
            }
            mapView = null;
            if (_comboMapEffect)
                UnityEngine.Object.Destroy(_comboMapEffect);
            _comboMapEffect = null;
            ClearShadow();
        }

        public void ClearShadow()
        {
            DynamicShadow.instance.ClearTargets();
        }

        public void AddTarget(Transform trans, Vector2 size)
        {
            DynamicShadow.instance.AddTarget(trans, size);
        }

        public void RemoveTarget(Transform trans)
        {
            DynamicShadow.instance.RemoveTarget(trans);
        }

        public void MoveForward()
        {
            if (!mapView) return;
            StartCoroutine(MoveForwardCoroutine());
        }

        private IEnumerator MoveForwardCoroutine()
        {
            yield return new WaitForSeconds(0.25f);//等待跑步动画播放时间，避免地图拖动人物移动错觉
            mapView.MoveForward();
        }

#if UNITY_EDITOR
        public void MoveBack()
        {
            if (!mapView) return;
            mapView.MoveBack();
        }
#endif
        public void ShowComboMapEffect(bool show)
        {
            if (_comboMapEffect)
                _comboMapEffect.SetActive(show);
        }

        private void ShowMapMask(bool show)
        {
            mapMaskSpriteRenderer.gameObject.SetActive(show);
        }

        private void SetMapMaskAlpha(float alpha)
        {
            Color color = mapMaskSpriteRenderer.color;
            color.a = alpha;
            mapMaskSpriteRenderer.color = color;
        }

        public void ShowMap(bool show, float duration)
        {
            StopCoroutine("MapTirggerCoroutine");
            ShowMapMask(!show);
            ShowComboMapEffect(!show);
            mapView.ShowMap(show, duration, 1f);
        }

        public void MapTrigger(float delay, float alpha)
        {
            //Debugger.Log("alpha:{0}", alpha);
            if (!mapView) return;
            _delay = delay;
            StopCoroutine("MapTirggerCoroutine");
            StartCoroutine("MapTirggerCoroutine");
            ShowMapMask(true);
            mapView.ShowMap(false, 0f, alpha);
        }

        private IEnumerator MapTirggerCoroutine()
        {
            yield return new WaitForSeconds(_delay);
            if (mapView)
            {
                ShowMapMask(false);
                mapView.ShowMap(true, 0f, 1f);
            }
        }

        public void Move(Transform trans, Vector3 endPos, System.Action callback = null, float duration = 0.15f)
        {
            //Debugger.LogError("speed:" + speed.ToString());
            StartCoroutine(MoveCoroutine(trans, endPos, callback, duration));
        }

        private IEnumerator MoveCoroutine(Transform trans, Vector3 endPos, System.Action callback = null, float duration = 0.15f)
        {
            Vector3 curPos = trans.localPosition;
            Vector3 normal = (endPos - curPos).normalized;
            float distance = Vector3.Distance(curPos, endPos);
            float delta = GameSetting.instance.deltaTimeFight;
            float speed = distance / (duration / delta);
            //Debugger.LogError("distance:" + distance + "  speed" + speed + "   duration:" + duration + "  delta:" + delta);
            normal *= speed;
            float time = 0;
            normal.y = 0;
            if (curPos.x - endPos.x <= 0)
            {
                while (curPos.x < endPos.x)
                {
                    time += delta;
                    yield return new WaitForSeconds(delta);
                    curPos += normal;
                    if (trans)
                        trans.localPosition = curPos;
                    else
                        yield break;
                }
                //Debugger.LogError("from left:"+time);
            }
            else
            {
                while (curPos.x > endPos.x)
                {
                    time += delta;
                    yield return new WaitForSeconds(delta);
                    curPos += normal;
                    if (trans)
                        trans.localPosition = curPos;
                    else
                        yield break;
                }
                //Debugger.LogError("from right:" + time);
            }
            if (trans)
                trans.localPosition = endPos;
            if (callback != null)
                callback();
        }


#if UNITY_EDITOR
        public bool showMapButton;
        void OnGUI()
        {
            if (showMapButton)
            {
                if (GUI.Button(new Rect(0, 200, 100, 20), "前进"))
                {
                    MoveForward();
                    //GameSetting.instance.pause = true;
                }

                if (GUI.Button(new Rect(0, 240, 100, 20), "后退"))
                {
                    MoveBack();
                    //GameSetting.instance.pause = false;
                }
            }
        }
#endif
    }
}