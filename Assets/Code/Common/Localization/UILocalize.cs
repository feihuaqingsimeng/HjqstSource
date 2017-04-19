using UnityEngine;

namespace Common.Localization
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(UnityEngine.UI.Text))]
	[AddComponentMenu("UI/Localize")]
	public class UILocalize : MonoBehaviour
	{
		public string key;

		public string value
		{
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					UnityEngine.UI.Text text = GetComponent<UnityEngine.UI.Text>();
					if (text != null)
					{
						text.text = value;
					}
				}
			}
		}

		bool _started = false;

		void OnEnabel ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			if (_started)
				OnLocalize ();
		}

		void Awake ()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif
			_started = true;
			OnLocalize ();
		}

		void OnLocalize ()
		{
			if (string.IsNullOrEmpty(key))
			{
				UnityEngine.UI.Text text = GetComponent<UnityEngine.UI.Text>();
				if (text != null)
					key = text.text;
			}

			if (!string.IsNullOrEmpty(key))
				value = Localization.Get(key);
		}
	}
}
