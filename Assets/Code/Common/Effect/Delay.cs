using UnityEngine;
using System.Collections;

public class Delay : MonoBehaviour {
	
	public float delayTime = 1.0f;
    
	public void Start () {
		gameObject.SetActive(false);
		Invoke("DelayFunc", delayTime);
	}
	
	void DelayFunc()
	{
        gameObject.SetActive(true);
	}

	public void PlayOnce () {
        gameObject.SetActive(false);
		Invoke("DelayFunc", delayTime);
	}
	
}

