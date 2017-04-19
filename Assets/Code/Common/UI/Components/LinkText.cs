using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using System.Text;


namespace Common.UI.Components
{
	[Serializable]
	public class ClickEvent : UnityEvent<LinkText>
	{

	}
	public class LinkText : MonoBehaviour,IPointerClickHandler 
	{

		public static LinkText Create(Text parentText)
		{
			GameObject go = new GameObject();
			Text underlineText = go.AddComponent<Text>();
			LinkText linkText = go.AddComponent<LinkText>();
			underlineText.name = "underline";
			underlineText.transform.SetParent(parentText.transform,false);
			linkText.Refresh();

			return linkText;
		}

		public bool refresh;
		public ClickEvent onClick;

		private int _id;
		public int Id
		{
			get
			{
				return _id;
			}
		}
		private string _alignString;
		private Vector2 _sizeDelta;
		public Vector2 SizeDelta
		{
			get
			{
				return _sizeDelta;
			}
		}
		void Awake()
		{

		}
		public void Set(int id ,UnityAction<LinkText> click)
		{
			this._id = id;

			if(onClick == null)
				onClick = new ClickEvent();
			if(click!= null)
				onClick.AddListener(click);
		}


		public void Refresh()
		{
			Text underlineText = GetComponent<Text>();
			Text parentText = transform.parent.GetComponent<Text>();
			_alignString = parentText.text;

			RectTransform rt = underlineText.rectTransform;

			rt.offsetMax = Vector2.zero;
			rt.offsetMin = Vector2.zero;
			rt.anchorMax = new Vector2(1,1);
			rt.anchorMin = new Vector2(0,0);
			rt.pivot = new Vector2(0,1);
			rt.anchoredPosition3D = new Vector3(0,-3,0);
			underlineText.font = parentText.font;
			underlineText.fontSize = parentText.fontSize;
			underlineText.lineSpacing = parentText.lineSpacing;
			underlineText.alignment = TextAnchor.UpperLeft;
			underlineText.horizontalOverflow = HorizontalWrapMode.Wrap;
			underlineText.verticalOverflow = VerticalWrapMode.Overflow;
			underlineText.color = parentText.color;
			underlineText.material = parentText.material;
			underlineText.text = _alignString;
			float alignWidth = underlineText.preferredWidth;
			float alignHeight = underlineText.preferredHeight;
			underlineText.text = "_";
			float perLineW = underlineText.preferredWidth;
            int count = (int)Mathf.Round( alignWidth/perLineW);
			for(int i = 1;i<count;i++)
            {
                underlineText.text += "_";
            }
			_sizeDelta = new Vector2(alignWidth,alignHeight);
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			if(onClick!=null)
			{
				onClick.Invoke(this);
			}
		}

		void Update()
		{
			if(refresh)
			{
				refresh = false;
				Refresh();

			}
		}
	}
}

