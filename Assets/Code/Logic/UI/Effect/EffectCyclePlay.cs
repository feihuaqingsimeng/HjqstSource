using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.UI.CommonAnimations;
public class EffectCyclePlay : MonoBehaviour {

	public float delay;
	public float duringTime;
	public bool firstPlay;

	public GameObject root;
	void Awake()
	{

		StartCoroutine(StartActionCoroutine());
	}

	void Start () {

	}
	
	private IEnumerator StartActionCoroutine()
	{
		Hide(false);
		if(firstPlay)
		{

		}else 
		{
			yield return new WaitForSeconds(delay);
		}
		while(true)
		{
			Show();
			yield return new WaitForSeconds(duringTime);
			Hide(true);
			yield return new WaitForSeconds(delay);
		}
	}
	private void Hide(bool needFade)
	{
		if(needFade)
		{
			root.SetActive(false);
//			CommonFadeToAnimation fadeTo = CommonFadeToAnimation.Get(root);;
//			fadeTo.init(1,0,0.3f);
//			fadeTo.onComplete = FadeOutFinied;
		}else
		{
			root.SetActive(false);
		}
	}
	private void FadeOutFinied()
	{
		root.GetComponent<CanvasGroup>().alpha = 1;
		root.SetActive(false);
	}
	private void Show()
	{
		root.SetActive(true);
	}
}
