using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using Common.ResMgr;
using Common.Localization;
using Logic.Hero.Model;

namespace Logic.UI.Shop.View
{
	public class RecuitHeroItem : MonoBehaviour
	{
		private HeroInfo _heroInfo;
		public HeroInfo HeroInfo
		{
			get
			{
				return _heroInfo;
			}
		}

		public bool playAni = true;
		#region UI components
		public Image frameImage;
		public Logic.UI.CommonHeroIcon.View.CommonHeroIcon commonHeroIcon;
		public GameObject specialEffectGameObject;
		public GameObject normalEffectGameObject;
		public WhiteFrameFadeInOutAnimation whiteFrameAnimation;
		#endregion

		private Sprite _frameBackSprite;
		private bool _isPlaySpecialEffect;
		private float _scaleToMin = 0.15f;
		private float _scaleToMax = 0.15f;
		void Awake ()
		{
			_frameBackSprite = ResMgr.instance.Load<Sprite>("sprite/main_ui/icon_head_box_big_back");
			commonHeroIcon.HideLevel();
			commonHeroIcon.gameObject.SetActive(false);
			if(whiteFrameAnimation!=null)
				whiteFrameAnimation.gameObject.SetActive(false);
		}
		public bool isStart ;
		void Update()
		{
			if(isStart)
			{
				isStart = false;
				Reset();
				TurnOverAfter(0);
			}
		}

		void Start ()
		{
			Reset();
		}

		public void Reset ()
		{
			frameImage.enabled = true;
			frameImage.SetSprite( _frameBackSprite);
			commonHeroIcon.gameObject.SetActive(false);
		}

		public void SetHeroInfo (HeroInfo heroInfo)
		{
			_heroInfo = heroInfo;
			commonHeroIcon.SetHeroInfo(_heroInfo);
			if(_heroInfo.advanceLevel >= 6)
			{
				_isPlaySpecialEffect = true;
				_scaleToMin = 0.5f;
				_scaleToMax = 0.3f;
			}else
			{
				_isPlaySpecialEffect = false;
				_scaleToMin = 0.15f;
				_scaleToMax = 0.15f;
			}
		}

		public void TurnOverAfter (float delay)
		{
			Invoke("TurnOver", delay);
		}

		public void TurnOver ()
		{
			LTDescr ltDescr = LeanTween.scaleX(gameObject, 0,_scaleToMin);
			ltDescr.setOnComplete(OnTurnOverComplete);
			if(_isPlaySpecialEffect)
			{
				if (specialEffectGameObject != null)
				{
					GameObject effect = Instantiate<GameObject>(specialEffectGameObject);
					effect.SetActive(true);
					effect.transform.SetParent(gameObject.transform,false);
					GameObject.Destroy(effect,3);
					
				}
			}
		}

		private void OnTurnOverComplete ()
		{
			frameImage.enabled = false;
			commonHeroIcon.gameObject.SetActive(true);
			LeanTween.scaleX(gameObject, 1, _scaleToMax);
			if(!_isPlaySpecialEffect)
			{
				StartCoroutine(StartWhiteFrameAnimationCoroutine(0));
			}

		}

		private IEnumerator StartWhiteFrameAnimationCoroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			if(whiteFrameAnimation!=null)
			{
				whiteFrameAnimation.gameObject.SetActive(true);
				whiteFrameAnimation.StartAction();
			}
			if(normalEffectGameObject != null)
			{
				GameObject effect = Instantiate<GameObject>(normalEffectGameObject);
				effect.SetActive(true);
				effect.transform.SetParent(gameObject.transform,false);
				GameObject.Destroy(effect,3);
			}
		}
	}
}
