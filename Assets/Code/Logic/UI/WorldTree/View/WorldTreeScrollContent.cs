using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using Common.ResMgr;
using Logic.Game.Model;

namespace Logic.UI.WorldTree.View
{
	public class WorldTreeScrollContent : MonoBehaviour
	{
		private ScrollRect _scrollRect;
		private float _cachedLocalY;
		private int _currentFirstIndex = 0;
		private int _currentLastIndex = 0;
		private int _bgMaxIndex = 0;

		public List<Sprite> _worldTreeDungeonBGSpriteList = new List<Sprite>();
		public List<RectTransform> worldTreeDungeonBGRectTransformList;

		void Awake ()
		{
			_scrollRect = GetComponentInParent<ScrollRect>();
			_cachedLocalY = transform.localPosition.y;
			_currentFirstIndex = 0;
			_currentLastIndex = worldTreeDungeonBGRectTransformList.Count - 1;
			_bgMaxIndex = GlobalData.GetGlobalData().worldTreeBGPicNameList.Count - 1;

			float contentHeight = 0;
			for (int index = 0; index <= _bgMaxIndex; index++)
			{
				Sprite sprite = ResMgr.instance.LoadSprite(Path.Combine("sprite/big_texture_2/", GlobalData.GetGlobalData().worldTreeBGPicNameList[index]));
				_worldTreeDungeonBGSpriteList.Add(sprite);
				contentHeight += sprite.rect.height;
			}

			GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);

			int worldTreeDungeonBGRectTransformCount = worldTreeDungeonBGRectTransformList.Count;
			for (int index = 0; index < worldTreeDungeonBGRectTransformCount; index++)
			{
				worldTreeDungeonBGRectTransformList[index].GetComponent<Image>().SetSprite(_worldTreeDungeonBGSpriteList[index]);
				worldTreeDungeonBGRectTransformList[index].GetComponent<Image>().SetNativeSize();
				worldTreeDungeonBGRectTransformList[index].localPosition = GetTreeSegmentPos(index);
			}
		}

		void Update ()
		{
			if (_cachedLocalY == transform.localPosition.y)
			{
				return;
			}

			if (transform.localPosition.y > _cachedLocalY)
			{
				OnScrollUp();
			}
			else if (transform.localPosition.y < _cachedLocalY)
			{
				OnScrollDown();
			}
			_cachedLocalY = transform.localPosition.y;
		}

		private Vector3 GetTreeSegmentPos (int bgIndex)
		{
			float x = bgIndex == 0 ? 16 : 0;
			float y = 0;
			for (int index = 0; index <= bgIndex - 1; index++)
			{
				Sprite sprite = _worldTreeDungeonBGSpriteList[index];
				y += sprite.rect.height;
			}
			return new Vector3(x, y, 0);
		}

		private void OnScrollUp ()
		{
			if (_currentFirstIndex == 0)
			{
				return;
			}

			Vector3[] contentRootWorldCorners = new Vector3[4];
			_scrollRect.GetComponent<RectTransform>().GetWorldCorners(contentRootWorldCorners);
			Vector3 contentRootTopLeftCorner = contentRootWorldCorners[1];

			int worldTreeBGRectTransformCount = worldTreeDungeonBGRectTransformList.Count;
			RectTransform worldTreeBGRectTransform = null;
			for (int i = 0; i < worldTreeBGRectTransformCount; i++)
			{
				worldTreeBGRectTransform = worldTreeDungeonBGRectTransformList[i];
				Vector3[] worldTreeBGWorldCorners = new Vector3[4];
				worldTreeBGRectTransform.GetWorldCorners(worldTreeBGWorldCorners);
				Vector3 worldTreeBGBottomLeftCorner = worldTreeBGWorldCorners[0];

				float yDiff = worldTreeBGBottomLeftCorner.y - contentRootTopLeftCorner.y;
				if (yDiff > 0)
				{
					if (_currentFirstIndex > 0)
					{
						_currentFirstIndex -= 1;
						_currentLastIndex -= 1;

						Debugger.Log( GlobalData.GetGlobalData().worldTreeBGPicNameList[_currentFirstIndex]);
						worldTreeBGRectTransform.GetComponent<Image>().SetSprite(_worldTreeDungeonBGSpriteList[_currentFirstIndex]);
						worldTreeBGRectTransform.GetComponent<Image>().SetNativeSize();
						worldTreeBGRectTransform.localPosition = GetTreeSegmentPos(_currentFirstIndex);
					}
				}
			}
		}

		private void OnScrollDown ()
		{
			if (_currentLastIndex == _bgMaxIndex)
			{
				return;
			}

			Vector3[] contentRootWorldCorners = new Vector3[4];
			_scrollRect.GetComponent<RectTransform>().GetWorldCorners(contentRootWorldCorners);
			Vector3 contentRootBottomLeftCorner = contentRootWorldCorners[0];

			int worldTreeBGRectTransformCount = worldTreeDungeonBGRectTransformList.Count;
			RectTransform worldTreeBGRectTransform = null;
			for (int i = 0; i < worldTreeBGRectTransformCount; i++)
			{
				worldTreeBGRectTransform = worldTreeDungeonBGRectTransformList[i];
				Vector3[] worldTreeBGWorldCorners = new Vector3[4];
				worldTreeBGRectTransform.GetWorldCorners(worldTreeBGWorldCorners);
				Vector3 worldTreeBGTopLeftCorner = worldTreeBGWorldCorners[1];

				float yDiff = worldTreeBGTopLeftCorner.y - contentRootBottomLeftCorner.y;
				if (yDiff < 0)
				{
					if (_currentLastIndex < _bgMaxIndex)
					{
						_currentFirstIndex += 1;
						_currentLastIndex += 1;

						worldTreeBGRectTransform.GetComponent<Image>().SetSprite(_worldTreeDungeonBGSpriteList[_currentLastIndex]);
						worldTreeBGRectTransform.GetComponent<Image>().SetNativeSize();
						worldTreeBGRectTransform.localPosition = GetTreeSegmentPos(_currentLastIndex);
					}
				}
			}
		}
	}
}
