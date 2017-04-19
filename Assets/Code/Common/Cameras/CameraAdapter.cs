using UnityEngine;

namespace Common.Cameras
{
	[RequireComponent(typeof(Camera))]
	public class CameraAdapter : MonoBehaviour
	{
		private Camera _camera;
		private float _originalFieldOfView;
		private float _originalOrthographicSize;
		private int _lastScreenWidth;
		private int _lastScreenHeight;

		public Vector2 designResolution = new Vector2(960, 640);
		public bool shouldAdjustSize = false;
		public bool shouldAdjustViewportRect = false;
		
		void Awake ()
		{
			_camera = GetComponent<Camera>();
			_lastScreenWidth = Screen.width;
			_lastScreenHeight = Screen.height;
		}

		void Start ()
		{
			if (_camera != null)
			{
				_originalFieldOfView = _camera.fieldOfView;
				_originalOrthographicSize = _camera.orthographicSize;
				AdjustSize();
			}
		}

#if UNITY_EDITOR
		void Update ()
		{
			if (Screen.width != _lastScreenWidth
			    || Screen.height != _lastScreenHeight)
			{
				AdjustSize();
				_lastScreenWidth = Screen.width;
				_lastScreenHeight = Screen.height;
			}
		}
#endif

		private void AdjustSize ()
		{
			if (_camera == null)
			{
				return;
			}

			float designScreenAspectRatio = designResolution.x / designResolution.y;
			float currentScreenAspectRatio = (float)Screen.width / Screen.height;
			if (currentScreenAspectRatio >= designScreenAspectRatio)
			{
				if (_camera.orthographic)
				{
					_camera.orthographicSize = _originalOrthographicSize;
				}
				else
				{
					_camera.fieldOfView = _originalFieldOfView;
				}

				if (shouldAdjustViewportRect)
				{
					Rect viewportRect = new Rect(0, 0, 1, 1);
					_camera.rect = viewportRect;
				}
			}
			else
			{
				int screenWidth = Screen.width;
				int screenHeight = Screen.height;
				float expectedHeight = (float)(screenHeight * designResolution.x) / screenWidth;
				float multiple = expectedHeight / designResolution.y;
				if (shouldAdjustSize)
				{
					if (_camera.orthographic)
					{
						_camera.orthographicSize = _originalOrthographicSize * multiple;
					}
					else
					{
						_camera.fieldOfView = _originalFieldOfView * multiple;
					}
				}

				if (shouldAdjustViewportRect)
				{
					float newHeight = designResolution.y * Screen.width / designResolution.x;
					float hScaleFactor = newHeight / Screen.height;
					Rect viewportRect = new Rect(0, (1 - hScaleFactor) * 0.5f, 1, hScaleFactor);
					_camera.rect = viewportRect;
				}
			}

		}
	}
}
