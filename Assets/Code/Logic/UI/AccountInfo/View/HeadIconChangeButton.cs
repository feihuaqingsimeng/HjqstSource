using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.ResMgr;

namespace Logic.UI.AccountInfo.View
{
	public class HeadIconChangeButton : MonoBehaviour 
	{
		[HideInInspector]
		public bool isSelect;
		[HideInInspector]
		public int headNo;

		public Image imgHeadIcon;
		public GameObject selectGO;


		public void Set(int headNo,bool isSelect)
		{
			this.headNo = headNo;
			this.isSelect = isSelect;
			Refresh();
		}

		private void Refresh()
		{
			imgHeadIcon.SetSprite( ResMgr.instance.Load<Sprite>(UIUtil.ParseHeadIcon(headNo)));
			selectGO.SetActive(isSelect);
		}
	}
}

