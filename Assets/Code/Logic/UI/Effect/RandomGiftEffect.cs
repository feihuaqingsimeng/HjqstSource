using UnityEngine;
using System.Collections;

public class RandomGiftEffect : MonoBehaviour {


	public Animator animator;

	public float time = 5;

	void Start () {
	
	}
	
	void OnEnable()
	{
		StartCoroutine(animationCoroutine());
	}
	private IEnumerator animationCoroutine()
	{
		while(true)
		{
			//animator.Play("run");
			animator.Play("run",0,0f);
			yield return new WaitForSeconds(time);
		}
	}
}
