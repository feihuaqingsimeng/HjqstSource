using UnityEngine;
using System.Collections;

public class RepeatRotate : MonoBehaviour {

	public float rotateTime =  1;
	public float rotate = 360;

	void Awake(){
		repeat();
	}
	private void repeat(){
		LeanTween.rotateAroundLocal(gameObject,Vector3.forward, rotate,rotateTime).setOnComplete(repeat);
	}
}
