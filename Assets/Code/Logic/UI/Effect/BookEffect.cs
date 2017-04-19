using UnityEngine;
using System.Collections;
using Common.Animators;

public class BookEffect : MonoBehaviour 
{
	public float animatorIdleTime;
	public float animatorRunTime;
	public float idleEffectTime;
	public float runEffectTime;
	public GameObject idleEffect;
	public GameObject runEffect;
	public Animator animator;

	void Awake()
	{


	}
	void OnEnable()
	{
		StartCoroutine(animationCoroutine());
	}
	private IEnumerator animationCoroutine()
	{
		while(true)
		{
			animator.Play("Idle");
			IdleEffect();
			StartCoroutine(effectRunCoroutine(idleEffectTime));
			yield return new WaitForSeconds(animatorIdleTime);
			animator.Play("Run");
			RunEffect();
			StartCoroutine(effectIdleCoroutine(runEffectTime));
			yield return new WaitForSeconds(animatorRunTime);
		}
	}
	private IEnumerator effectIdleCoroutine(float time)
	{
		yield return new WaitForSeconds(time);
		IdleEffect();
	}
	private IEnumerator effectRunCoroutine(float time)
	{
		yield return new WaitForSeconds(time);
		RunEffect();
	}
	private void RunEffect()
	{
		idleEffect.SetActive(false);
		runEffect.SetActive(true);
	}
	private void IdleEffect()
	{
		idleEffect.SetActive(true);
		runEffect.SetActive(false);
		
	}
}
