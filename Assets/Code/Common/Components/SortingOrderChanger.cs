using UnityEngine;
namespace Common.Components
{
    public class SortingOrderChanger : MonoBehaviour
    {
        private Renderer[] _renderers;
        private int[] _layers;

        [SerializeField]
        private Logic.UI.EUISortingLayer _uiSortingLayer = Logic.UI.EUISortingLayer.MainUI;
        public bool isPlus = false;
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

        [SerializeField]
        private int _sortingOrder;
        public int sortingOrder
        {
            get
            {
                return _sortingOrder;
            }
            set
            {
                _sortingOrder = value;
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    Start();
            }
        }
        public static SortingOrderChanger Get(GameObject go)
        {
            SortingOrderChanger changer = go.GetComponent<SortingOrderChanger>();
            if (changer == null)
                changer = go.AddComponent<SortingOrderChanger>();
            return changer;
        }
        void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            
			if(_renderers != null)
			{
				_layers = new int[_renderers.Length];
				for (int i = 0, count = _renderers.Length; i < count; i++)
				{
					Renderer r = _renderers[i];
					_layers[i] = r.sortingOrder;
				}
			}
            
        }

        void Start()
        {
			if(_renderers != null)
			{
				for (int i = 0, count = _renderers.Length; i < count; i++)
				{
					Renderer r = _renderers[i];
					if (r)
					{
						r.sortingLayerName = _uiSortingLayer.ToString();
						if (isPlus)
							r.sortingOrder = _layers[i] + sortingOrder;
						else
							r.sortingOrder = sortingOrder;
					}
				}
			}
            
        }

#if UNITY_EDITOR
        void Update()
        {
            Start();
        }
#endif
    }
}
