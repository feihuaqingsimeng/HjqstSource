using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Cameras.Controller;
using UnityEngine.UI;
using Common.Util;
using Common.Localization;
using Logic.Enums;
using Logic.Skill.Model;
using Logic.Fight.Controller;
namespace Logic.UI.Buff.View
{
    public class BuffTipsView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/buff/buff_tips_view";

        #region ui components
        public Text tipsText;
        #endregion

        private Vector3 _worldPos;
        public Vector3 worldPos
        {
            set
            {
                _worldPos = value;
                ChangeThisPos(new Vector3(_worldPos.x, _worldPos.y * 0.5f, _worldPos.z));
            }
        }
        private float _offset;
        public float offset
        {
            set
            {
                _offset = value;
            }
            private get
            {
                return _offset;
            }

        }

        void OnDisable()
        {
            transform.localPosition = Vector3.one;
        }

        private void PlayMoveLocalYAnimation(float toY, float time, float delay)
        {
            LTDescr ltDescr = LeanTween.moveLocalY(gameObject, toY, time);
            ltDescr.tweenType = LeanTweenType.linear;
            ltDescr.setIgnoreTimeScale(true);
            ltDescr.setDelay(delay);
        }

        private void PlayAlphaAnimation(float toAlpha, float time, float delay)
        {
            LTDescr ltDescr = LeanTween.value(gameObject, 1, toAlpha, time);
            ltDescr.tweenType = LeanTweenType.easeInOutSine;
            ltDescr.setIgnoreTimeScale(true);
            ltDescr.setOnUpdate(OnUpdateAlpha);
            ltDescr.setOnComplete(OnAlphaAnimationComplemte);
            ltDescr.setDelay(delay);
        }

        private void OnUpdateAlpha(float alpha)
        {
            tipsText.CrossFadeAlpha(alpha, 0, true);
        }

        private void OnAlphaAnimationComplemte()
        {
            Logic.UI.Buff.Controller.BuffTipsController.instance.DespawnBuffTipsView(transform);
        }

        private void PlayScaleAnimation(float toSale, float time, float delay)
        {
            Vector2 toScaleVector = new Vector2(toSale, toSale);
            LTDescr ltDescr = LeanTween.scale(gameObject, toScaleVector, time);
            ltDescr.tweenType = LeanTweenType.easeInOutSine;
            ltDescr.setIgnoreTimeScale(true);
            ltDescr.setDelay(delay);
        }

        public void SetTips(string tips)
        {
            Debugger.Log("tips:" + tips);
            tipsText.text = tips;
            tipsText.gameObject.SetActive(true);
        }

        private void ChangeThisPos(Vector3 pos, bool isBool = false)
        {
            LeanTween.cancel(gameObject);

            Vector3 screenPos = CameraController.instance.mainCamera.WorldToScreenPoint(pos);
            screenPos.z = 0f;
            Vector3 uiPos = CameraController.instance.uiCamera.ScreenToWorldPoint(screenPos);
            uiPos = uiPos / UI.UIMgr.instance.basicCanvas.transform.localScale.x;
            if (offset != 0)
                uiPos.y += offset;
            this.transform.localPosition = (Vector2)uiPos;
            transform.localScale = Vector3.one;
            //transform.localScale *= 0.9f;
            //PlayMoveLocalYAnimation(transform.localPosition.y - 30, 0.05f, 0);
            //PlayScaleAnimation(2f, 0.05f, 0);
            //PlayScaleAnimation(0.8f, 0.1f, 0.07f);
            PlayMoveLocalYAnimation(transform.localPosition.y + 50, 0.3f, 0f);
            PlayAlphaAnimation(0f, 0.2f, 0.8f);
        }
    }
}