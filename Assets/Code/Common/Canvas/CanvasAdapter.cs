using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Common.Canvases
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasAdapter : MonoBehaviour
    {
        private Canvas _canvas;
        private CanvasScaler _canvasScaler;

        private Vector2 _designResolution;
		private ScreenOrientation _screenOrientation;

        void Awake()
        {
			_screenOrientation = Screen.orientation;
            _canvas = GetComponent<Canvas>();
            _canvasScaler = GetComponent<CanvasScaler>();
            if (_canvas.worldCamera == null)
                _canvas.worldCamera = Logic.UI.UIMgr.instance.uiCamera;
            _designResolution = _canvasScaler.referenceResolution;
            AdjustCanvasSize();
        }		

        //		public void SetDesignResolution (Vector2 designResolution)
        //		{
        //			_designResolution = designResolution;
        //			_canvasScaler.referenceResolution = _designResolution;
        //			AdjustCanvasSize();
        //		}

        private void AdjustCanvasSize()
        {
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();

            if (canvasScaler == null)
            {
                return;
            }

            float designScreenAspectRatio = _designResolution.x / _designResolution.y;
            float currentScreenAspectRatio = (float)Screen.width / Screen.height;
            if (currentScreenAspectRatio >= designScreenAspectRatio)
            {
                canvasScaler.matchWidthOrHeight = 1;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0;
            }
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause)
                AdjustCanvasSize();
        }


        void Update()
        {
			#if UNITY_EDITOR
            	AdjustCanvasSize();
			#endif
			if (_screenOrientation != Screen.orientation)
			{
				AdjustCanvasSize();
				_screenOrientation = Screen.orientation;
			}
        }

    }
}
