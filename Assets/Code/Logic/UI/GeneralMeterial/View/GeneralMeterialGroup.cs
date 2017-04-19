using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.UI.Components;

namespace Logic.UI.GeneralMeterial.View
{
	public class GeneralMeterialGroup : MonoBehaviour 
	{
		
		public Dictionary<uint, List<Toggle>> toggleGroup = new Dictionary<uint, List<Toggle>>();

		public void AddToggle(uint groupId,Toggle toggle)
		{
			if(toggle == null)
				return;

			bool exist = false;
			List<Toggle> toggleList;
			if(toggleGroup.ContainsKey(groupId))
			{
				toggleList = toggleGroup[groupId];

				for(int i = 0,count = toggleList.Count;i<count;i++)
				{

					if(toggle == toggleList[i])
					{
						exist = true;
						break;
					}
						
				}

			}else
			{
				toggleList = new List<Toggle>();

			}
			if(!exist)
			{
				toggleList.Add(toggle);
				toggleGroup[groupId] = toggleList;

			}

		}
		public void OnClickToggle(Toggle toggle)
		{
			int id = GetToggleGroup(toggle);
			if(id == -1)
				return;
			List<Toggle> toggleList = toggleGroup[(uint)id];
			Toggle t;
			if(toggle.isOn)
			{
				for(int i = 0,count = toggleList.Count;i<count;i++)
				{
					t = toggleList[i];
					if(t != toggle)
						t.isOn = false;
				}
			}else
			{
				bool hasOn = false;
				for(int i = 0,count = toggleList.Count;i<count;i++)
				{
					t = toggleList[i];
					if(t.isOn)
					{
						hasOn = true;
						break;
					}
				}
				if(!hasOn)
					toggle.isOn = true;
			}
		}
		public int GetToggleGroup(Toggle toggle)
		{
			List<Toggle> toggleList;
			int group = -1;
			foreach(var value in toggleGroup)
			{
				toggleList = value.Value;
				for(int i = 0,count = toggleList.Count;i<count;i++)
				{
					if(toggleList[i] == toggle)
					{
						group = (int)value.Key;
						break;
					}
				}
			}
			return group;
		}

	}
}

