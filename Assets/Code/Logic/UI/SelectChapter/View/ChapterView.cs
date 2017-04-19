using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Util;
using Common.ResMgr;
using Logic.Enums;
using Logic.Dungeon.Model;
using Logic.Chapter.Model;
using Common.Localization;

namespace Logic.UI.SelectChapter.View
{
	public class ChapterView : MonoBehaviour
	{
		public const float CHAPTER_BG_WIDTH = 1139;
		public const float CHAPTER_BG_HEIGHT = 640;

		private ChapterInfo _chapterInfo;
		public ChapterInfo ChapterInfo
		{
			get
			{
				return _chapterInfo;
			}
		}

		#region ui components
		public RectTransform rectTransform;
		public RawImage chapterBGRawImage;
		public RawImage chapterBGLineRawImage;
		#endregion ui components

		public void SetChapterInfoAndType (ChapterInfo chapterInfo)
		{
			_chapterInfo = chapterInfo;

			chapterBGRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CHAPTER_BG_WIDTH);
			chapterBGRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CHAPTER_BG_HEIGHT);
			
			Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bounds.size.x);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bounds.size.y);
		}

		public void Show ()
		{
			if (_chapterInfo == null)
			{
				return;
			}

			string chapterBGTexturePath = ResPath.GetChapterBGPath(_chapterInfo.chapterData.chapterBG);
			Texture chapterBGTexture = ResMgr.instance.Load<Texture>(chapterBGTexturePath);
			chapterBGRawImage.texture = chapterBGTexture;
			chapterBGRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CHAPTER_BG_WIDTH);
			chapterBGRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CHAPTER_BG_HEIGHT);
			
			Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bounds.size.x);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bounds.size.y);
			
			string chapterBGLineTexturePath = ResPath.GetChapterBGPath(_chapterInfo.chapterData.chapterLineBG);
			Texture chapterBGLineTexture = ResMgr.instance.Load<Texture>(chapterBGLineTexturePath);
			chapterBGLineRawImage.texture = chapterBGLineTexture;
			chapterBGRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CHAPTER_BG_WIDTH);
			chapterBGRawImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CHAPTER_BG_HEIGHT);
		}
	}
}
