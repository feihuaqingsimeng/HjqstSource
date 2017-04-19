using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Localization;
namespace Logic.UI.Components
{
	public class LocalizationText : MonoBehaviour 
	{
		
		public string localString;
		
		void Start()
		{
			Text text = GetComponent<Text>();
			if(text!= null)
			{
				text.text = Localization.Get(localString);
			}
		}
	}
}

