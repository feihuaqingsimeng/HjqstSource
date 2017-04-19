using UnityEngine;
using System.Collections;
namespace Logic.UI.Mask.View
{
    public class MaskView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/mask/mask_view";
        
		public GameObject loadingCircle;
		public float loadingBarRotateTime;
		public GameObject maskGO;
		void Awake()
		{

		}
		void Start()
        {
			maskGO.SetActive(false);

			LTDescr ltDescr = LeanTween.rotateAroundLocal(loadingCircle, Vector3.back, 360, loadingBarRotateTime);
			ltDescr.setRepeat(-1);

			LeanTween.delayedCall(gameObject, 2, ShowCircle);
        }

		void OnDestroy ()
		{
			LeanTween.cancel(gameObject);
		}

		private void ShowCircle()
		{
			maskGO.SetActive(true);
		}
    }
}