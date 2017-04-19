using UnityEngine;
using System.Collections;

namespace Logic.UI.FightResult.View{

	public class ScaleShake : MonoBehaviour {

		public float from = 0;
		public float to = 1;
		public float time = 0.2f;
		public float delay = 0;
		public ScaleShakeType type = ScaleShakeType.ScleShake_After;

		public bool reset;

		private Transform _tran ;

		public enum ScaleShakeType{
			ScaleShake_Before,//先shake在缩放
			ScleShake_After,//先缩放在shake
		}
	
		void Awake(){
			_tran = transform;
		}
		void Update () {
			if(reset)
			{
				reset = false;
				StartAction();
			}
		}
		public void init(float from,float to,float time,float delay ,ScaleShakeType type){
			this.from = from;
			this.to = to;
			this.time = time;
			this.delay = delay;
			this.type = type;
			StartAction();
		}
		public void StartAction(){
			_tran.localScale = new Vector3(from,from,1);
			if(type == ScaleShakeType.ScaleShake_Before){
				LeanTween.scale(_tran as RectTransform,new Vector3(from+0.1f,from+0.1f,1),time/4).setDelay(delay).setOnComplete(ScaleShake_BeforeComplete);
			}else{
				LeanTween.scale(_tran as RectTransform,new Vector3(to,to,1),time).setDelay(delay).setOnComplete(ScleShake_AfterComplete);
			}
			
		}
		private void ScleShake_AfterComplete(){
			LeanTween.scale(_tran as RectTransform,new Vector3(to-0.1f,to-0.1f,1),time/4).setLoopPingPong(1);
		}
		private void ScaleShake_BeforeComplete(){
			LeanTween.scale(_tran as RectTransform,new Vector3(to,to,1),time);
		}
	}
}

