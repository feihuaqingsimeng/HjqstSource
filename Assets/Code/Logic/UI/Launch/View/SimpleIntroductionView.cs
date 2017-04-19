using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Logic.UI;
using Common.ResMgr;

namespace Logic.UI.Launch.View
{
	public class SimpleIntroductionView : MonoBehaviour
	{
		public const string PREFAB_PATH = "ui/launch/simple_introduction_view";

		public delegate void OnCompleteDelegate();
		public OnCompleteDelegate onCompleteDelegate;
		
		public List<SimpleIntroductionItem> simpleIntroductionItemList;


		public static SimpleIntroductionView Open()
		{
			return UIMgr.instance.Open<SimpleIntroductionView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Replace);
		}

		void Awake ()
		{
			for (int i = 0, count = simpleIntroductionItemList.Count; i < count; i++)
			{
				simpleIntroductionItemList[i].gameObject.SetActive(false);
				simpleIntroductionItemList[i].SetOnAnimationEndCompleteDelegate(OnAnimationCompleteHandler);
			}
		}

		void Start ()
		{
			simpleIntroductionItemList[0].animator.enabled = true;
			simpleIntroductionItemList[0].animator.Play("introduction_1", 0, 0);
			simpleIntroductionItemList[0].animator.gameObject.SetActive(true);
		}

		public void OnAnimationCompleteHandler (int index)
		{

			if (index + 1 < simpleIntroductionItemList.Count)
			{
				simpleIntroductionItemList[index].gameObject.SetActive(false);
				simpleIntroductionItemList[index + 1].gameObject.SetActive(false);
				simpleIntroductionItemList[index + 1].animator.enabled = true;
				simpleIntroductionItemList[index + 1].animator.Play("introduction_" + (index + 2).ToString(), 0, 0);
				simpleIntroductionItemList[index + 1].animator.gameObject.SetActive(true);
			}
			else
			{
				FinishSimpleIntroduction();
			}
		}

		public void SetOnTimeOutDelegate(OnCompleteDelegate onCompleteDelegate)
		{
			this.onCompleteDelegate = onCompleteDelegate;
		}

		private void FinishSimpleIntroduction ()
		{
			if (onCompleteDelegate != null)
				onCompleteDelegate();
			UIMgr.instance.Close(PREFAB_PATH);
		}

		public void ClickSkipButtonHandler ()
		{
			FinishSimpleIntroduction();
		}
	}
}