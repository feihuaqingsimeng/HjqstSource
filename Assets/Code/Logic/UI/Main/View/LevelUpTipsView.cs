using UnityEngine;
using System.Collections;
using Logic.UI;
using Logic.Audio.Controller;

public class LevelUpTipsView : MonoBehaviour {
	
	public const string PREFAB_PATH = "ui/main/level_up_view";
	public float duringTime = 1;
	public float delayTime = 0.3f;
	public GameObject root;
	public static void Open()
		
	{
		LevelUpTipsView view = UIMgr.instance.Open<LevelUpTipsView>(PREFAB_PATH,EUISortingLayer.Notice);
		view.StartAction();
	}
	
	
	public void StartAction()
	{
		root.SetActive(false);
		AudioController.instance.PlayAudio(AudioController.account_level_up,false,delayTime);
		StopCoroutine("StartActionCoroutine");
		StartCoroutine("StartActionCoroutine");
	}
	
	private IEnumerator StartActionCoroutine()
	{
		yield return new WaitForSeconds(delayTime);
		root.SetActive(true);

		yield return new WaitForSeconds(duringTime);
		UIMgr.instance.Close(PREFAB_PATH);
	}
}


