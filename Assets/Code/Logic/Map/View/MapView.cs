using UnityEngine;
using System.Collections;
using Logic.Net.Controller;
using Logic.Map.Controller;
using System.Collections.Generic;
using Common.GameTime.Controller;
using Logic.Enums;
namespace Logic.Map.View
{
    public class MapView : MonoBehaviour
    {
        #region unity object
        public GameObject aheadGO;
        public GameObject betweenGO;
        public GameObject groundGO;
        public GameObject behindGO;
        #endregion
        //private Vector3 fgPos;
        //private Vector3 bgPos;
        public float aheadTime = 0.533f;
        public float betweenTime = 0.533f;
        public float groundTime = 0.533f;
        public float behindTime = 0.533f;
        public float aheadOffsetX = 0f;
        public float betweenOffsetX = 0f;
        public float groundOffsetX = 0f;
        public float behindOffsetX = 0f;
        private List<SpriteRenderer> renderers;
        private bool _show;
        private float _duration;
        private float _alpha;

        public delegate void AlphaChangeHandler(float alpha);
        public AlphaChangeHandler alphaChangeHandler;
        void Start()
        {
            SpriteRenderer[] rs = gameObject.GetComponentsInChildren<SpriteRenderer>();
            renderers = new List<SpriteRenderer>();
            for (int i = 0, count = rs.Length; i < count; i++)
            {
                SpriteRenderer r = rs[i];
                if (r.gameObject.layer == (int)LayerType.SceneFront)
                    renderers.Add(r);
            }
        }

        public void MoveForward()
        {
            Vector3 aheadEndPos = aheadGO.transform.localPosition;
            aheadEndPos.x -= aheadOffsetX;
            MapController.instance.Move(aheadGO.transform, aheadEndPos, null, aheadTime);

            Vector3 betweenEndPos = betweenGO.transform.localPosition;
            betweenEndPos.x -= betweenOffsetX;
            MapController.instance.Move(betweenGO.transform, betweenEndPos, null, betweenTime);

            Vector3 groundEndPos = groundGO.transform.localPosition;
            groundEndPos.x -= groundOffsetX;
            MapController.instance.Move(groundGO.transform, groundEndPos, () =>
            {
#if UNITY_EDITOR
                if (!MapController.instance.showMapButton)
#endif
                    DataMessageHandler.DataMessage_FinishRun(CharacterType.Player);
            }, groundTime);

            Vector3 behindEndPos = behindGO.transform.localPosition;
            behindEndPos.x -= behindOffsetX;
            MapController.instance.Move(behindGO.transform, behindEndPos, null, behindTime);
        }

#if UNITY_EDITOR
        public void MoveBack()
        {
            Vector3 aheadEndPos = aheadGO.transform.localPosition;
            aheadEndPos.x += aheadOffsetX;
            MapController.instance.Move(aheadGO.transform, aheadEndPos, null, aheadTime);

            Vector3 betweenEndPos = betweenGO.transform.localPosition;
            betweenEndPos.x += betweenOffsetX;
            MapController.instance.Move(betweenGO.transform, betweenEndPos, null, betweenTime);

            Vector3 groundEndPos = groundGO.transform.localPosition;
            groundEndPos.x += groundOffsetX;
            MapController.instance.Move(groundGO.transform, groundEndPos, null, groundTime);

            Vector3 behindEndPos = behindGO.transform.localPosition;
            behindEndPos.x += behindOffsetX;
            MapController.instance.Move(behindGO.transform, behindEndPos, null, behindTime);
        }
#endif

        public void ShowMap(bool show, float duration, float alpha)
        {
            _show = show;
            _duration = duration;
            _alpha = alpha;
            StopCoroutine("ShowMapCoroutine");
            StartCoroutine("ShowMapCoroutine");
        }

        private IEnumerator ShowMapCoroutine()
        {
            if (_duration > 0)
            {
                float time = Time.realtimeSinceStartup;
                float currentTime = time;
                while (Time.realtimeSinceStartup - time < _duration)
                {
                    if (!TimeController.instance.playerPause)
                    {
                        if (_show)
                        {
                            for (int i = 0, count = renderers.Count; i < count; i++)
                            {
                                SpriteRenderer sr = renderers[i];
                                if (!sr) continue;
                                Color color = sr.color;
                                color.a += (Game.GameSetting.instance.deltaTimeFight / _duration) * _alpha;
                                if (alphaChangeHandler != null)
                                    alphaChangeHandler(1f - color.a);
                                sr.color = color;
                                /*Renderer r = renderers[i];
                                if (r is SpriteRenderer)
                                {
                                    SpriteRenderer sr = r as SpriteRenderer;
                                    Color color = sr.color;
                                    color.a += (Game.GameSetting.instance.deltaTimeFight / _duration) * _alpha;
                                    if (alphaChangeHandler != null)
                                        alphaChangeHandler(1f-color.a);
                                    sr.color = color;
                                }
                                else
                                {
#if UNITY_EDITOR
                                    Color color = r.material.color;
#else
                            Color color = r.sharedMaterial.color;                        
#endif
                                    color.a += (Game.GameSetting.instance.deltaTimeFight / _duration) * _alpha;
                                    if (alphaChangeHandler != null)
                                        alphaChangeHandler(1f - color.a);
#if UNITY_EDITOR
                                    r.material.color = color;
#else
                            r.sharedMaterial.color = color;
#endif
                                }*/
                            }
                        }
                        else
                        {
                            for (int i = 0, count = renderers.Count; i < count; i++)
                            {
                                SpriteRenderer sr = renderers[i];
                                if (!sr) continue;
                                Color color = sr.color;
                                color.a -= (Game.GameSetting.instance.deltaTimeFight / _duration) * _alpha;
                                if (alphaChangeHandler != null)
                                    alphaChangeHandler(1f - color.a);
                                sr.color = color;
                                /*Renderer r = renderers[i];
                                if (r is SpriteRenderer)
                                {
                                    SpriteRenderer sr = r as SpriteRenderer;
                                    Color color = sr.color;
                                    color.a -= (Game.GameSetting.instance.deltaTimeFight / _duration) * _alpha;
                                    if (alphaChangeHandler != null)
                                        alphaChangeHandler(1f - color.a);
                                    sr.color = color;
                                }
                                else
                                {
#if UNITY_EDITOR
                                    Color color = r.material.color;
#else
                            Color color = r.sharedMaterial.color;                        
#endif
                                    color.a -= (Game.GameSetting.instance.deltaTimeFight / _duration) * _alpha;
                                    if (alphaChangeHandler != null)
                                        alphaChangeHandler(1f - color.a);
#if UNITY_EDITOR
                                    r.material.color = color;
#else
                            r.sharedMaterial.color = color;
#endif
                                }*/
                            }
                        }
                    }
                    yield return null;
                    if (TimeController.instance.playerPause)
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
            }
            else
            {
                if (_show)
                {
                    for (int i = 0, count = renderers.Count; i < count; i++)
                    {
                        SpriteRenderer sr = renderers[i];
                        if (!sr) continue;
                        Color color = sr.color;
                        color.a = _alpha;
                        sr.color = color;
                        /*Renderer r = renderers[i];
                        if (r is SpriteRenderer)
                        {
                            SpriteRenderer sr = r as SpriteRenderer;
                            Color color = sr.color;
                            color.a = _alpha;
                            if (alphaChangeHandler != null)
                                alphaChangeHandler(1f - color.a);
                            sr.color = color;
                        }
                        else
                        {
#if UNITY_EDITOR
                            Color color = r.material.color;
#else
                        Color color = r.sharedMaterial.color;                        
#endif
                            color.a = _alpha;
                            if (alphaChangeHandler != null)
                                alphaChangeHandler(1f - color.a);
#if UNITY_EDITOR
                            r.material.color = color;
#else
                        r.sharedMaterial.color = color;
#endif
                        }*/
					}
					if (alphaChangeHandler != null)
						alphaChangeHandler(1f - _alpha);
                }
                else
                {
                    for (int i = 0, count = renderers.Count; i < count; i++)
                    {
                        SpriteRenderer sr = renderers[i];
                        if (!sr) continue;
                        Color color = sr.color;
                        color.a = _alpha;
                        sr.color = color;
                        /*Renderer r = renderers[i];
                        if (r is SpriteRenderer)
                        {
                            SpriteRenderer sr = r as SpriteRenderer;
                            Color color = sr.color;
                            color.a = _alpha;
                            if (alphaChangeHandler != null)
                                alphaChangeHandler(1f - color.a);
                            sr.color = color;
                        }
                        else
                        {
                            Debugger.Log(r.gameObject.name);
#if UNITY_EDITOR
                            Color color = r.material.color;
#else
                        Color color = r.sharedMaterial.color;                        
#endif
                            color.a = _alpha;
                            if (alphaChangeHandler != null)
                                alphaChangeHandler(1f - color.a);
#if UNITY_EDITOR
                            r.material.color = color;
#else
                        r.sharedMaterial.color = color;
#endif
                        }*/
					}
					if (alphaChangeHandler != null)
						alphaChangeHandler(1f - _alpha);
                }
            }
        }
    }
}
