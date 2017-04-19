using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Common.Util;

namespace Common.UI.Components
{
    [Serializable]
    public class OnResetItem : UnityEvent<GameObject, int>
    {
    }

    [Serializable]
    public class OnInitComplete : UnityEvent
    {
    }

    public class ScrollContent : MonoBehaviour
    {
        private Vector3 _cachedLocalPosition;
        private RectTransform _itemRectTransform;

        private int _totalCount;
        private Vector2 _firstContentItemPosition;
        private int _currentFirstIndex = 0;
        private int _currentLastIndex = 0;
        private List<GameObject> _contentItemList = new List<GameObject>();
        private Dictionary<GameObject, int> _contentItemIndexDictionary = new Dictionary<GameObject, int>();
        private bool _shouldPlayInitAnimation = false;
        private float _shouldPlayInitAnimationDelay = 0;
        public ScrollRect scrollRect;
        public RectTransform rectTransform;
        public GameObject contentItemPrefab;
        public bool shouldDisablePrefab = false;
        public int row;
        public int col;
        public float paddingTop;
        public float paddingBottom;
        public float paddingLeft;
        public float paddingRight;
        public float horizontalSpacing;
        public float verticalSpacing;
        public OnResetItem onResetItem;
        public OnInitComplete onInitComplete;

        void Awake()
        {
            //_itemRectTransform = contentItemPrefab.GetComponent<RectTransform>();
        }

        public void Init(int totalCount, bool shouldPlayInitAnimation = false, float delay = 0)
        {
            if (_itemRectTransform == null)
                _itemRectTransform = contentItemPrefab.GetComponent<RectTransform>();
            _totalCount = totalCount;
            contentItemPrefab.SetActive(!shouldDisablePrefab);
            _cachedLocalPosition = transform.localPosition;
            _firstContentItemPosition = new Vector2(_itemRectTransform.rect.width * 0.5f, -(_itemRectTransform.rect.height * 0.5f)) + new Vector2(paddingLeft, -paddingTop);

            if (scrollRect.vertical && scrollRect.horizontal)
            {
                Debugger.Log("The ScrollContent does not support both x and y axis scroll at the same time.");
                return;
            }
            _shouldPlayInitAnimation = shouldPlayInitAnimation;
            _shouldPlayInitAnimationDelay = delay;
            transform.localPosition = new Vector2(transform.localPosition.x, 0);
            GenerateItems();
        }

        private void ResizeContentRoot()
        {
			if (_totalCount <= 0)
				return;

            if (scrollRect.vertical)
            {
				if (_itemRectTransform != null)
				{
					int actualRow = Mathf.CeilToInt(_totalCount * 1.0f / col);
                //float contentWidth = GetComponent<RectTransform>().rect.size.x;
	                float contentHeight = paddingTop + actualRow * _itemRectTransform.rect.height + (actualRow - 1) * verticalSpacing + paddingBottom;
	                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
				}
            }
            else if (scrollRect.horizontal)
            {
				if (_itemRectTransform)
				{
	                int actualCol = Mathf.CeilToInt(_totalCount * 1.0f / row);
	                float contentWidth = paddingLeft + actualCol * _itemRectTransform.rect.width + (actualCol - 1) * horizontalSpacing + paddingRight;
	                //float contentHeight = GetComponent<RectTransform>().rect.size.y;
	                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentWidth);
				}
            }
        }

        public void Reset(int totalCount)
        {
            _totalCount = totalCount;
            ResizeContentRoot();
            OnScrollUp();
            OnScrollLeft();
            RefreshAllContentItems();
        }

        private void GenerateItems()
        {
            ResizeContentRoot();
            int maxItemCount = row * col;
            int shouldGenerateItemCount = Mathf.Min(_totalCount, maxItemCount);
            _currentFirstIndex = 0;
            _currentLastIndex = maxItemCount - 1;

            _contentItemList.Clear();
            _contentItemIndexDictionary.Clear();
            TransformUtil.ClearChildren(transform, true);

            for (int i = 0; i < shouldGenerateItemCount; i++)
            {
                GameObject contentItemGameObject = GameObject.Instantiate(contentItemPrefab);
                _contentItemList.Add(contentItemGameObject);
                _contentItemIndexDictionary.Add(contentItemGameObject, i);
                contentItemGameObject.transform.SetParent(transform, false);
                contentItemGameObject.SetActive(true);
                ResetContentItem(contentItemGameObject, i);
                //				ResetContentItemIndexAndPos(contentItemGameObject, i);
            }

            if (_shouldPlayInitAnimation)
            {
                int itemCount = _contentItemList.Count;
                if (itemCount > 0)
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        float debutAnimationDelay = 0;
                        if (scrollRect.vertical)
                        {
                            debutAnimationDelay = (i / col) * 0.025f;
                        }
                        else if (scrollRect.horizontal)
                        {
                            debutAnimationDelay = (i / row) * 0.025f;
                        }
                        PlayDebutAnimation(_contentItemList[i], debutAnimationDelay, i == shouldGenerateItemCount - 1);
                    }
                }
            }
            else
            {
                OnInitComplete();
            }
        }

        void FixedUpdate()
        {
            if (transform.localPosition == _cachedLocalPosition)
                return;
            if (transform.localPosition.y < _cachedLocalPosition.y)
                OnScrollDown();
            else if (transform.localPosition.y > _cachedLocalPosition.y)
                OnScrollUp();
            else if (transform.localPosition.x < _cachedLocalPosition.x)
                OnScrollLeft();
            else if (transform.localPosition.x > _cachedLocalPosition.x)
                OnScrollRight();
            _cachedLocalPosition = transform.localPosition;
        }

        private void PlayDebutAnimation(GameObject contentItemGameObject, float delay, bool isLast)
        {
            float startY = contentItemGameObject.transform.localPosition.y - scrollRect.GetComponent<RectTransform>().rect.size.y;
            Vector3 startLocalPosition = contentItemGameObject.transform.localPosition;
            startLocalPosition.y = startY;
            float endY = contentItemGameObject.transform.localPosition.y;

            contentItemGameObject.transform.localPosition = startLocalPosition;
            LTDescr ltDescr = LeanTween.moveLocalY(contentItemGameObject, endY, 0.6f);
            ltDescr.tweenType = LeanTweenType.easeInSine;
            ltDescr.setDelay(delay + _shouldPlayInitAnimationDelay);

            if (isLast)
            {
                ltDescr.setOnComplete(OnInitComplete);
            }
        }

        private void OnInitComplete()
        {
            onInitComplete.Invoke();
        }

        private Vector3 GetItemLocalPositionByIndex(int index)
        {
            Vector3 itemLocalPosition = Vector3.zero;
            if (scrollRect.vertical)
            {
                itemLocalPosition = _firstContentItemPosition + new Vector2((index % col) * (_itemRectTransform.rect.width + horizontalSpacing), -((index / col) * (_itemRectTransform.rect.height + verticalSpacing)));
            }
            if (scrollRect.horizontal)
            {
                itemLocalPosition = _firstContentItemPosition + new Vector2((index / row) * (_itemRectTransform.rect.width + horizontalSpacing), -((index % row) * (_itemRectTransform.rect.height + verticalSpacing)));
            }
            return itemLocalPosition;
        }

        private void ResetContentItemIndexAndPos(GameObject contentItemGameObject, int newIndex)
        {
            _contentItemIndexDictionary[contentItemGameObject] = newIndex;
            //contentItemGameObject.transform.SetSiblingIndex(newIndex);
            contentItemGameObject.transform.localPosition = GetItemLocalPositionByIndex(newIndex);
			contentItemGameObject.name = newIndex.ToString();
        }

        private void ResetContentItem(GameObject contentItemGameObject, int newIndex)
        {
            //			_contentItemIndexDictionary[contentItemGameObject] = newIndex;
            ResetContentItemIndexAndPos(contentItemGameObject, newIndex);
            onResetItem.Invoke(contentItemGameObject, newIndex);
            //			contentItemGameObject.transform.SetSiblingIndex(newIndex);
            //			contentItemGameObject.transform.localPosition = GetItemLocalPositionByIndex(newIndex);
        }

        private int CompareContentItem(GameObject aItemGO, GameObject bItemGO)
        {
            return aItemGO.transform.GetSiblingIndex() - bItemGO.transform.GetSiblingIndex();
        }

        public void RefreshAllContentItems()
        {
            _contentItemList.Sort(CompareContentItem);
            int contentItemCount = _contentItemList.Count;
            GameObject contentItemGameObject = null;
            for (int i = 0; i < contentItemCount; i++)
            {
                contentItemGameObject = _contentItemList[i];
                ResetContentItem(contentItemGameObject, _contentItemIndexDictionary[contentItemGameObject]);
            }
        }

        public List<T> GetContentItemComponentList<T>()
        {
            List<T> contentItemComponentList = new List<T>();
            int contentItemCount = _contentItemList.Count;
            for (int i = 0; i < contentItemCount; i++)
            {
                contentItemComponentList.Add(_contentItemList[i].GetComponent<T>());
            }
            return contentItemComponentList;
        }

        private Vector3[] GetWorldCorners(GameObject go)
        {
            Vector3[] worldCorners = new Vector3[4];
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.GetWorldCorners(worldCorners);
            }
            return worldCorners;
        }

		public void ScrollTo (int index)
		{
			if (scrollRect.vertical)
			{

			}
			else if (scrollRect.horizontal)
			{
				float idealX = -(paddingLeft + index * (contentItemPrefab.GetComponent<RectTransform>().rect.size.x + horizontalSpacing));
				rectTransform.anchoredPosition = new Vector2(idealX, 0); 
			}
		}

        public void ScrollToEnd()
        {
            if (scrollRect.vertical)
            {
                scrollRect.verticalNormalizedPosition = 0;
                OnScrollUp();
            }
            if (scrollRect.horizontal)
            {
                scrollRect.horizontalNormalizedPosition = 1;
                OnScrollLeft();
            }
        }
		public void ScrollToEndByTime(float duringTime)
		{

			if (scrollRect.vertical)
			{
				LeanTween.value(gameObject,(float t)=>
				{
					scrollRect.verticalNormalizedPosition = t;
					OnScrollUp();
				},scrollRect.verticalNormalizedPosition,0,duringTime);

			}
			if (scrollRect.horizontal)
			{
				LeanTween.value(gameObject,(float t)=>
				                {
					scrollRect.verticalNormalizedPosition = t;
					OnScrollLeft();
				},scrollRect.verticalNormalizedPosition,1,duringTime);

			}
		}

        private void OnScrollDown()
        {
            if (_currentFirstIndex == 0)
                return;

			float generatedContentTotalHeight = row * contentItemPrefab.GetComponent<RectTransform>().rect.height + (row - 1) * verticalSpacing;
			float bottomBorderY = scrollRect.transform.TransformPoint(new Vector3(0, -generatedContentTotalHeight * 0.5f, 0)).y;
            GameObject contentItemGameObject = null;
			for (int i = 0, contentItemCount = _contentItemList.Count; i < contentItemCount; i++)
            {
                contentItemGameObject = _contentItemList[i];
                Vector3 contentItemTopLeftCorner = GetWorldCorners(contentItemGameObject)[1];

                float yDiff = contentItemTopLeftCorner.y - bottomBorderY;
                if (yDiff <= 0)
                {
                    if (_currentFirstIndex > 0)
                    {
                        int prevIndex = _currentFirstIndex - 1;
                        _currentFirstIndex = prevIndex;
                        _currentLastIndex -= 1;
                        ResetContentItem(contentItemGameObject, prevIndex);
                    }
                }
            }
        }

        private void OnScrollUp()
        {
            if (_currentLastIndex == _totalCount - 1)
                return;

			float generatedContentTotalHeight = row * contentItemPrefab.GetComponent<RectTransform>().rect.height + (row - 1) * verticalSpacing;
			float topBorderY = scrollRect.transform.TransformPoint(new Vector3(0, generatedContentTotalHeight * 0.5f, 0)).y;
            GameObject contentItemGameObject = null;
			for (int i = 0, contentItemCount = _contentItemList.Count; i < contentItemCount; i++)
            {
                contentItemGameObject = _contentItemList[i];
                Vector3 contentItemBottomLeftCorner = GetWorldCorners(contentItemGameObject)[0];

                float yDiff = contentItemBottomLeftCorner.y - topBorderY;
                if (yDiff >= 0)
                {
                    if (_currentLastIndex < _totalCount - 1)
                    {
                        int nextIndex = _currentLastIndex + 1;
                        _currentLastIndex = nextIndex;
                        _currentFirstIndex += 1;
                        ResetContentItem(contentItemGameObject, nextIndex);
                    }
                }
            }
        }

        private void OnScrollRight()
        {
            if (_currentFirstIndex == 0)
                return;

			float generatedContentTotalWidth = col * contentItemPrefab.GetComponent<RectTransform>().rect.width + (col - 1) * horizontalSpacing;
			float rightBorderX = scrollRect.transform.TransformPoint(new Vector3(generatedContentTotalWidth * 0.5f, 0, 0)).x;
            GameObject contentItemGameObject = null;

			for (int i = 0, contentItemCount = _contentItemList.Count; i < contentItemCount; i++)
            {
                contentItemGameObject = _contentItemList[i];
                Vector3 contentItemTopLeftCorner = GetWorldCorners(contentItemGameObject)[1];

                float xDiff = contentItemTopLeftCorner.x - rightBorderX;
                if (xDiff >= 0)
                {
                    if (_currentFirstIndex > 0)
                    {
                        int prevIndex = _currentFirstIndex - 1;
                        _currentFirstIndex = prevIndex;
                        _currentLastIndex -= 1;
                        ResetContentItem(contentItemGameObject, prevIndex);
                    }
                }
            }
        }

        private void OnScrollLeft()
        {
            if (_currentLastIndex == _totalCount - 1)
                return;

			float generatedContentTotalWidth = col * contentItemPrefab.GetComponent<RectTransform>().rect.width + (col - 1) * horizontalSpacing;
			float leftBorderX = scrollRect.transform.TransformPoint(new Vector3(-generatedContentTotalWidth * 0.5f, 0, 0)).x;
            GameObject contentItemGameObject = null;

			for (int i = 0, contentItemCount = _contentItemList.Count; i < contentItemCount; i++)
            {
                contentItemGameObject = _contentItemList[i];
                Vector3 contentItemTopRightCorner = GetWorldCorners(contentItemGameObject)[2];

                float xDiff = contentItemTopRightCorner.x - leftBorderX;
                if (xDiff <= 0)
                {
                    if (_currentLastIndex < _totalCount - 1)
                    {
                        int nextIndex = _currentLastIndex + 1;
                        _currentLastIndex = nextIndex;
                        _currentFirstIndex += 1;
                        ResetContentItem(contentItemGameObject, nextIndex);
                    }
                }
            }
        }
    }
}
