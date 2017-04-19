using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

namespace Common.UI.Components
{
	public class ToggleContent : MonoBehaviour 
	{
		[NoToLua]
		public List<GameObject> onList;
		[NoToLua]
		public List<GameObject> offList;
		[NoToLua]
		public Text onTitleText;
		[NoToLua]
		public Text offTitleText;


		public int id;

		void Awake()
		{
			Toggle toggle = GetComponent<Toggle>();
			if(toggle!= null)
			{
				toggle.onValueChanged.AddListener (UpdateValueChangedHandler);
			}
			UpdateValueChangedHandler(toggle.isOn);
		}

		public void Set(int id,string titleString = "")
		{
			this.id = id;
			if(onTitleText!= null)
				onTitleText.text = titleString;
			if(offTitleText!=null)
				offTitleText.text = titleString;
		}

		private void UpdateValueChangedHandler(bool value)
		{
			if(value)
			{
				int count = onList.Count;
				for(int i = 0;i<count;i++)
				{
					onList[i].SetActive(true);
				}
				count = offList.Count;
				for(int i = 0;i<count;i++)
				{
					offList[i].SetActive(false);
				}
				if(offTitleText!= null)
					offTitleText.gameObject.SetActive(false);
				if(onTitleText!=null)
					onTitleText.gameObject.SetActive(true);

			}else{
				int count = onList.Count;
				for(int i = 0;i<count;i++)
				{
					onList[i].SetActive(false);
				}
				count = offList.Count;
				for(int i = 0;i<count;i++)
				{
					offList[i].SetActive(true);
				}
				if(onTitleText!=null)
					onTitleText.gameObject.SetActive(false);
				if(offTitleText!= null)
					offTitleText.gameObject.SetActive(true);
			}
		}
	}

}
