using UnityEngine;
using System.Collections;
using Common.ResMgr;

namespace Logic.UI.FightResult.View{
	public class TreasureBoxRotateShake : MonoBehaviour {
		
		public bool reset;
		public float MoveDistance = 600;
		public float MoveBounce = 10;
		public float bounceTime = 0.01f;
		public float RootPanelShakeDistance = 5;
		public float RootPanelShakeTime = 0.006f;
		public float RootPanelShakeTimeDelay = 0.01f;
		public float moveTime = 0.2f;
		public float shakeRotate = 10;
		public int RootPanelshakeCount = 1;
		public float shakeTime = 0.1f;
		public float shakeDelay = 0.5f;
		public bool isLoop = true;

		public Vector3 smokePos = new Vector3(14,-177,0);
		public float smokeTime = 0.2f;
		public string smokePath ="effects/prefabs/ui_effect_06";
		public Transform RootPanel;
		
		
		void Update () {
			if(reset)
			{
				reset = false;
				StartAction();
			}
		}
		private Vector3 oldPos ;
		public void init(){
			StartAction();
		}
		public void StartAction(){
			oldPos = transform.localPosition;
			oldPos = new Vector3(oldPos.x,oldPos.y,oldPos.z);
			transform.localPosition = new Vector3(oldPos.x,oldPos.y+MoveDistance,oldPos.z);
			LeanTween.moveLocalY(gameObject,oldPos.y,moveTime).setOnComplete(moveFinished).setEase(LeanTweenType.easeInSine);

			if(!string.IsNullOrEmpty(smokePath))
				StartCoroutine(PlayerSmokeCoroutine());

		}
		private IEnumerator PlayerSmokeCoroutine(){
			yield return new WaitForSeconds(smokeTime);

			GameObject go = ResMgr.instance.Load<GameObject>(smokePath);
			GameObject effectSmoke =  Instantiate(go);
			Common.Components.SortingOrderChanger sortingOrder = effectSmoke.AddComponent<Common.Components.SortingOrderChanger>();
			effectSmoke.transform.SetParent(transform.parent,false);
			sortingOrder.sortingOrder = 120;
			
			effectSmoke.transform.localPosition = smokePos;
		}
		private void moveFinished(){
			LeanTween.moveLocalY(gameObject,oldPos.y-MoveBounce,bounceTime).setLoopPingPong(1).setOnComplete(DropFinished);
			LeanTween.moveLocalY(RootPanel.gameObject,RootPanel.localPosition.y-RootPanelShakeDistance,RootPanelShakeTime).setLoopPingPong(RootPanelshakeCount).setDelay(RootPanelShakeTimeDelay);

		}
		private void DropFinished()
		{
			LeanTween.moveLocalY(gameObject,oldPos.y+3,0.06f).setLoopPingPong().setLoopCount(-1);
			LeanTween.moveLocalX(gameObject,oldPos.x+1,0.06f).setLoopPingPong().setLoopCount(-1);
			MoveAllFinished();
			Observers.Facade.Instance.SendNotification(string.Format("{0}::{1}", FightResultView.PREFAB_PATH, "OnBoxDropComplete"));
		}
		private void MoveAllFinished(){

			LeanTween.rotateAroundLocal(transform as RectTransform,Vector3.forward,shakeRotate,shakeTime).setOnComplete(ShakePartFinished).setDelay(shakeDelay);
		}
		private void ShakePartFinished(){
			LeanTween.rotateAroundLocal(transform as RectTransform,Vector3.forward,-2*shakeRotate,shakeTime).setLoopPingPong(1).setOnComplete(ShakeReset);
		}
		private void ShakeReset(){
			LeanTween.rotateAroundLocal(transform as RectTransform,Vector3.forward,-shakeRotate,shakeTime).setOnComplete(ShakeFinished);
		}
		private void ShakeFinished(){

			if(isLoop)
				MoveAllFinished();
		}
		
	}
}

