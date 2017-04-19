using UnityEngine;
using System.Collections;

namespace Logic.UI.CommonAnimations
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CommonMoveByAnimation : MonoBehaviour
	{
		private CanvasGroup _canvasGroup;

		public float time = 1f;
		public float delay = 0f;
		public Vector3 moveBy;

		public static CommonMoveByAnimation Get(GameObject go) {
			CommonMoveByAnimation move = go.GetComponent<CommonMoveByAnimation>();
			if (move == null) {
				move = go.AddComponent<CommonMoveByAnimation>();
			}
			return move;
		}

		void Awake ()
		{

		}

		void Start ()
		{

		}
		public void Init(float time,float delay,Vector3 moveby)
		{
			this.time = time;
			this.delay = delay;
			this.moveBy = moveby;
			StartAction();
		}
		void StartAction(){

			LeanTween.moveLocal(gameObject,transform.localPosition+moveBy,time);
		}
		
	}
}
