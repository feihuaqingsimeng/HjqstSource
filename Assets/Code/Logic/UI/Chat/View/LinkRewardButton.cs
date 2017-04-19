using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.Equipment.Model;
using Common.Localization;
using Logic.UI.Description.View;

namespace Logic.UI.Chat.View
{
	public class LinkRewardButton : MonoBehaviour 
	{
		
		#region ui component 
		public Text nameText;
		public Text starText;
		public Image starImg;
		public RectTransform textRoot;
		public RectTransform imageBgRt;
		#endregion

		public Vector2 board = new Vector2(20,15);

		public bool isReset;
		private EquipmentInfo _equipInfo;
		private RectTransform _rt;

		public void SetEquip(EquipmentInfo info)
		{
			Clear();
			_equipInfo = info;
			nameText.text = Localization.Get( _equipInfo.equipmentData.name);
			Vector2 nameSize = new Vector2(nameText.preferredWidth,nameText.preferredHeight);
			starText.text = string.Format("x{0}",_equipInfo.equipmentData.quality);
			Vector2 starTextSize = new Vector2(starText.preferredWidth,starText.preferredHeight);
			starImg.rectTransform.anchoredPosition = new Vector2(nameSize.x,0);
			Vector2 starImageSize = starImg.rectTransform.sizeDelta;
			starText.rectTransform.anchoredPosition = new Vector2(nameSize.x+starImageSize.x,0);
			_rt = transform as RectTransform;
			Vector2 rootSize =  new Vector2(nameSize.x+starTextSize.x+starImageSize.x,nameSize.y);
			_rt.sizeDelta = rootSize+new Vector2(board.x,0);
			imageBgRt.sizeDelta = rootSize+board;
		}
		public RectTransform rectTransform
		{
			get
			{
				return _rt;
			}
		}

		private void Clear()
		{
			_equipInfo = null;
		}
		public void OnClickButtonHandler()
		{
			if(_equipInfo!= null)
			{
				RectTransform rt = transform as RectTransform;
				CommonEquipDesView.Open(_equipInfo,transform.position,rt.rect.size);
			}
		}

		void Update()
		{
			if(isReset)
			{
				isReset = false;
				SetEquip(_equipInfo);
			}
		}
	}
}

