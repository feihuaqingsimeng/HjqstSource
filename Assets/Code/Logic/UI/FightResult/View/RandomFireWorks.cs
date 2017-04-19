using UnityEngine;
using System.Collections;
using Common.Util;
using Logic.Enums;
using Common.Components;
using Common.ResMgr;

namespace Logic.UI.FightResult.View
{
	public class RandomFireWorks : MonoBehaviour 
	{
		public float delay = 0;
		public int width = 300;
		public int height = 200;
		public float minIntervalTime = 0.3f;
		public float maxIntervalTime = 1f;
		public float duringTime = 1;
		public string fireWorksPath = "effects/prefabs/ui_effect_09";
		public bool start = false;
		public bool stop;
		public IEnumerator CreateFireWorksCoroutine()
		{
			yield return new WaitForSeconds(delay);
			for(;;)
			{
				if(stop)
					break;
				int x = Random.Range(-width,width);
				int y = Random.Range(-height,height);
				float time = Random.Range(minIntervalTime,maxIntervalTime);
				yield return new WaitForSeconds(time);
				GameObject go = ResMgr.instance.Load<GameObject>(fireWorksPath) as GameObject;
				go = Instantiate<GameObject>(go);

				go.transform.SetParent(gameObject.transform,false);
				go.transform.localPosition = new Vector3(x,y);
				SortingOrderChanger sortingOrder = SortingOrderChanger.Get(go);
				sortingOrder.sortingOrder = sortingOrder.sortingOrder+gameObject.GetComponentInParent<Canvas>().sortingOrder;
				GameObject.Destroy(go,duringTime);
			}
			
		}
		public void StartRandom()
		{
			StopCoroutine(CreateFireWorksCoroutine());
			StartCoroutine(CreateFireWorksCoroutine());
		}
		void Update()
		{
			if(start)
			{
				start = false;
				StartRandom();
			}
		}
	}
}

