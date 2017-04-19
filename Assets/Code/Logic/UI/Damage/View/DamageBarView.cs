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
namespace Logic.UI.Damage.View
{
    public class DamageBarView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/damage/damage_bar_view";

        public enum DamageBarType
        {
            Invalid = 0,
            Dodge = 1,
            Damage = 2,
            Critical = 3,
            Treat = 4,
            Block = 5,
        }

        private DamageBarType _damageBarType = DamageBarType.Invalid;

        #region ui components
        public Text grayNumberText;
        public Text orangeNumberText;
        public Text greenNumberText;
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

        void OnDisable()
        {
            transform.localPosition = Vector3.one;
        }

        private void PlayMoveLocalYAnimation(float toY, float time, float delay)
        {
            LTDescr ltDescr = LeanTween.moveLocalY(gameObject, toY, time);
            ltDescr.tweenType = LeanTweenType.easeInOutSine;
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
            grayNumberText.CrossFadeAlpha(alpha, 0, true);
            orangeNumberText.CrossFadeAlpha(alpha, 0, true);
            greenNumberText.CrossFadeAlpha(alpha, 0, true);
        }

        private void OnAlphaAnimationComplemte()
        {
            Logic.Judge.Controller.JudgeController.instance.DespawnDamageBarView(transform);
        }

        private void PlayScaleAnimation(float toSale, float time, float delay)
        {
            Vector2 toScaleVector = new Vector2(toSale, toSale);
            LTDescr ltDescr = LeanTween.scale(gameObject, toScaleVector, time);
            ltDescr.tweenType = LeanTweenType.easeInOutSine;
            ltDescr.setIgnoreTimeScale(true);
            ltDescr.setDelay(delay);
        }

        public void SetDodge(SkillInfo skillInfo)
        {
            _damageBarType = DamageBarType.Dodge;
            switch (skillInfo.skillData.skillType)
            {
                case SkillType.Hit:      //普攻
                case SkillType.Skill:    //技能
                case SkillType.Aeon:
                    grayNumberText.text = Localization.Get("game.fight.miss");
                    grayNumberText.gameObject.SetActive(true);
                    orangeNumberText.gameObject.SetActive(false);
                    greenNumberText.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        public void SetBlock(SkillInfo skillInfo)
        {
            _damageBarType = DamageBarType.Block;
            switch (skillInfo.skillData.skillType)
            {
                case SkillType.Hit:      //普攻
                case SkillType.Skill:    //技能
                case SkillType.Aeon:
                    grayNumberText.text = Localization.Get("game.fight.block");
                    grayNumberText.gameObject.SetActive(true);
                    orangeNumberText.gameObject.SetActive(false);
                    greenNumberText.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        public void SetDemageValue(SkillInfo skillInfo, uint value, bool isContinuous)
        {
            _damageBarType = DamageBarType.Damage;
            if (skillInfo == null || isContinuous)//持续掉血
            {
                grayNumberText.gameObject.SetActive(false);
                orangeNumberText.gameObject.SetActive(true);
                greenNumberText.gameObject.SetActive(false);
                orangeNumberText.text = value.ToString();
            }
            else
            {
                switch (skillInfo.skillData.skillType)
                {
                    case SkillType.Hit://普攻
                    case SkillType.Skill://技能
                    case SkillType.Aeon:
                        grayNumberText.gameObject.SetActive(true);
                        orangeNumberText.gameObject.SetActive(false);
                        greenNumberText.gameObject.SetActive(false);
                        grayNumberText.text = value.ToString();
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetCriticalValue(uint value)
        {
            _damageBarType = DamageBarType.Critical;
            grayNumberText.gameObject.SetActive(false);
            orangeNumberText.gameObject.SetActive(true);
            greenNumberText.gameObject.SetActive(false);
            orangeNumberText.text = value.ToString();
        }

        public void SetTreatValue(uint value)
        {
            _damageBarType = DamageBarType.Treat;
            grayNumberText.gameObject.SetActive(false);
            orangeNumberText.gameObject.SetActive(false);
            greenNumberText.gameObject.SetActive(true);
            greenNumberText.text = "+" + value.ToString();
        }

        private void ChangeThisPos(Vector3 pos, bool isBool = false)
        {
            LeanTween.cancel(gameObject);

            Vector3 screenPos = CameraController.instance.mainCamera.WorldToScreenPoint(pos);
            screenPos.z = 0f;
            Vector3 uiPos = CameraController.instance.uiCamera.ScreenToWorldPoint(screenPos);
            uiPos = uiPos / UI.UIMgr.instance.basicCanvas.transform.localScale.x;
            this.transform.localPosition = (Vector2)uiPos;

            transform.localScale = Vector3.one;

            switch (_damageBarType)
            {
                case DamageBarType.Dodge:
                case DamageBarType.Damage:
                    {
                        transform.localScale *= 0.9f;
                        PlayMoveLocalYAnimation(transform.localPosition.y - 30, 0.05f, 0);
                        PlayScaleAnimation(2f, 0.05f, 0);
                        PlayScaleAnimation(0.8f, 0.1f, 0.07f);
                        PlayAlphaAnimation(0.9f, 0.5f, 0.07f);
                        PlayMoveLocalYAnimation(transform.localPosition.y + 40, 0.5f, 0.07f);
                        break;
                    }
				
				case DamageBarType.Block:
					{
						transform.localScale *= 0.9f;
						PlayMoveLocalYAnimation(transform.localPosition.y + 50, 0.05f, 0);
						PlayScaleAnimation(2f, 0.05f, 0);
						PlayScaleAnimation(0.8f, 0.1f, 0.07f);
						PlayAlphaAnimation(0.9f, 0.5f, 0.07f);
						PlayMoveLocalYAnimation(transform.localPosition.y + 70, 0.5f, 0.07f);
						break;
					}
                case DamageBarType.Critical:
                    {
                        PlayMoveLocalYAnimation(transform.localPosition.y - 10, 0.05f, 0);
                        PlayScaleAnimation(2.8f, 0.05f, 0);
                        PlayScaleAnimation(0.9f, 0.1f, 0.08f);
                        PlayAlphaAnimation(0.9f, 0.5f, 0.18f);
                        PlayMoveLocalYAnimation(transform.localPosition.y + 60, 0.5f, 0.18f);
                        break;
                    }
                case DamageBarType.Treat:
                    {
                        transform.localScale *= 0.8f;
                        PlayAlphaAnimation(0, 1, 0.05f);
                        PlayMoveLocalYAnimation(transform.localPosition.y + 80, 1, 0.05f);
                        break;
                    }
                default:
                    {
                        PlayMoveLocalYAnimation(transform.localPosition.y - 30, 0.05f, 0);
                        PlayScaleAnimation(2.2f, 0.05f, 0);
                        PlayScaleAnimation(0.9f, 0.1f, 0.07f);
                        PlayAlphaAnimation(0.9f, 0.5f, 0.07f);
                        PlayMoveLocalYAnimation(transform.localPosition.y + 40, 0.5f, 0.07f);
                        break;
                    }
            }
        }
    }
}