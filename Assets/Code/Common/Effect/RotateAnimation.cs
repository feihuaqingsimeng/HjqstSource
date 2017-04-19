using UnityEngine;

namespace Common.Effect
{
	public class RotateAnimation : MonoBehaviour
	{
		public Vector3 axis;
		public float add;
		public float delay;
		public float time;
		public int repeat;
		public bool destroyOnComplete = false;

		void Start ()
		{
			LTDescr ltDescr = LeanTween.rotateAroundLocal(gameObject, axis, add, time);
			ltDescr.setDelay(delay);
			ltDescr.setRepeat(repeat);
			ltDescr.setDestroyOnComplete(destroyOnComplete);
		}
	}
}