using UnityEngine;

namespace Common.Effect
{
	public class SeperateAxisScaleAnimation : MonoBehaviour
	{
		public Vector3 startScale;
		public Vector3 endScale;


		public float xTime;
		public float xDelay;

		public float yTime;
		public float yDelay;

		public float zTime;
		public float zDelay;

		void Start ()
		{
			PlayerAnimation();
		}

		void OnEnable ()
		{
			PlayerAnimation();
		}

		private void PlayerAnimation ()
		{
			LTDescr xAxisLTDescr = LeanTween.scaleX(gameObject, endScale.x, xTime).setFrom(startScale.x).setDelay(xDelay);
			LTDescr yAxisLTDescr = LeanTween.scaleY(gameObject, endScale.y, yTime).setFrom(startScale.y).setDelay(yDelay);
			LTDescr zAxisLTDescr = LeanTween.scaleZ(gameObject, endScale.z, zTime).setFrom(startScale.z).setDelay(zDelay);

			float xTotalTime = xTime + xDelay;
			float yTotalTime = yTime + yDelay;
			float zTotalTime = zTime + zDelay;
			float totalFinishTime = Mathf.Max(xTotalTime, yTotalTime, zTotalTime);
		}
	}
}
