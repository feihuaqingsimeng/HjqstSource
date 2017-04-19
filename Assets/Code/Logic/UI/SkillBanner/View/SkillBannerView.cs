using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using Logic.Skill.Model;
using Common.Components.Effect;

namespace Logic.UI.SkillBanner.View
{
    public class SkillBannerView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/skill_banner/skill_banner_view";

        #region UI components
        public Text skillNameText;
        public Text skillDescriptionText;
        public GameObject particleGameObject;
        public GameObject coreGO;
        #endregion

		public static SkillBannerView Open ()
		{
			SkillBannerView skillBannerView = UIMgr.instance.Open<SkillBannerView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
			return skillBannerView;
		}

        void Awake ()
        {
            coreGO.SetActive(false);
            ShowParticle(false);
        }

        public void Show(SkillInfo skillInfo)
        {
			LeanTween.cancel(gameObject);

            skillNameText.text = Localization.Get(skillInfo.skillData.skillName);
            skillDescriptionText.text = Localization.Get(skillInfo.skillData.skillName);
            coreGO.SetActive(true);

            LTDescr lTdescr = LeanTween.scaleX(gameObject, 1, 0.2f);
            lTdescr.tweenType = LeanTweenType.easeInCubic;
            lTdescr.setIgnoreTimeScale(true);
            lTdescr.setFrom(0);

			LTDescr delayHideLTDescr = LeanTween.value(gameObject, 0, 1, 2.5f);
			delayHideLTDescr.setIgnoreTimeScale(true);
			delayHideLTDescr.setOnComplete(Hide);
        }

        private void Hide()
        {
            coreGO.SetActive(false);
        }

        public void ShowParticle(bool show)
        {
            particleGameObject.SetActive(show);
            if (show)
            {
                ParticlePlayer pp = particleGameObject.AddComponent<ParticlePlayer>();
                pp.isLoop = true;
                pp.isUI = true;
                //pp.speed = Game.GameSetting.instance.speed;
            }
            else
            {
                ParticlePlayer pp = particleGameObject.GetComponent<ParticlePlayer>();
                if (pp)
                    UnityEngine.Object.Destroy(pp);
            }
        }
    }
}