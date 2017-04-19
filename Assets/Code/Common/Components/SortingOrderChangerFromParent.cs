using UnityEngine;
namespace Common.Components
{
	public class SortingOrderChangerFromParent : MonoBehaviour
    {
        private Renderer[] _renderers;

		[SerializeField]
		private Logic.UI.EUISortingLayer _uiSortingLayer = Logic.UI.EUISortingLayer.MainUI;
		public Logic.UI.EUISortingLayer UISortingLayer
		{
			get
			{
				return _uiSortingLayer;
			}
			set
			{
				_uiSortingLayer = value;
			}
		}
	
		public static SortingOrderChangerFromParent Get(GameObject go)
		{
			SortingOrderChangerFromParent changer = go.GetComponent<SortingOrderChangerFromParent>();
			if(changer == null)
				changer = go.AddComponent<SortingOrderChangerFromParent>();
			return changer;
		}
        void Awake()
        {

        }

        void Start()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
			if(_renderers != null)
			{
				Canvas parentCanvas = GetComponentInParent<Canvas> ();
				for (int i = 0, count = _renderers.Length; i < count; i++)
				{
					Renderer r = _renderers[i];
					if (r)
					{
						r.sortingLayerName = _uiSortingLayer.ToString();
						r.sortingOrder = r.sortingOrder+parentCanvas.sortingOrder;
					}
				}
			}

        }
    }
}
