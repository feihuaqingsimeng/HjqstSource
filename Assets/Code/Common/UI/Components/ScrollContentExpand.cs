using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Common.Util;
using System.Collections;
using LuaInterface;

namespace Common.UI.Components
{
	[Serializable]
	public class OnResetItemEvent : UnityEvent<GameObject, int>
	{
	}

	[Serializable]
	public class OnInitCompleteEvent : UnityEvent
	{
	}

	public class ScrollContentExpand : MonoBehaviour
	{
		private ScrollRect _scrollRect;
		private Vector3 _cachedLocalPosition;
		private float _contentHeight = 0;
		private float _contentWidth = 0;
		private int _totalCount;
		private Vector2 _firstContentItemPosition;
		private float _itemRectWidth;
		private float _itemRectHeight;
		private int _currentFirstIndex = 0;
		private int _currentLastIndex = 0;
		private List<GameObject> _contentItemList = new List<GameObject>();
		private List<GameObject> _contentItemRemovedList = new List<GameObject>();
		private SortedDictionary<int, GameObject> _contentItemIndexDictionary = new SortedDictionary<int, GameObject>();
		private Dictionary<int, Vector3> _contentItemPositionDictionary = new Dictionary<int, Vector3>();
		private bool _shouldGenerateItems = false;
		private bool _shouldPlayInitAnimation = false;
		private float _shouldPlayInitAnimationDelayTime = 0;
		private bool _isVertical;
		private Transform _transform;
		private RectTransform _rectTransform;
		[NoToLua]
		public GameObject contentItemPrefab;
		[NoToLua]
		public bool shouldDisablePrefab = false;
		[NoToLua]
		public int row;
		[NoToLua]
		public int col;
		[NoToLua]
		public float paddingTop;
		[NoToLua]
		public float paddingBottom;
		[NoToLua]
		public float paddingLeft;
		[NoToLua]
		public float horizontalSpacing;
		[NoToLua]
		public float verticalSpacing;
		[NoToLua]
		public bool isVariableSize;//子控件是否可变长度
		[NoToLua]
		public bool refresh;
		[NoToLua]
		public OnResetItemEvent onResetItem;
		[NoToLua]
		public OnInitCompleteEvent onInitComplete;

		private bool autoHide;
		public ScrollRect scrollRect
		{
			get
			{
				return _scrollRect;
			}
		}
		private Vector3[] contentRootWorldCorners ;
		
		[NoToLua]
		void Awake ()
		{
			if (shouldDisablePrefab)
			{
				contentItemPrefab.SetActive(false);
			}
			_transform = transform;
			_rectTransform = transform as RectTransform;
			_scrollRect = GetComponentInParent<ScrollRect>();
			_scrollRect.onValueChanged.AddListener(OnScroll);
			_cachedLocalPosition = _transform.localPosition;
			_isVertical = _scrollRect.vertical;

			if(row == 0 || col == 0)
				Debugger.LogError("scrollContentExpand row or col is 0");

		}

		public void Init (int totalCount, bool shouldPlayInitAnimation = false,float delay = 0)
		{
			_totalCount = totalCount;
			_transform.localPosition = new Vector2(_transform.localPosition.x, 0);
			_contentItemList.Clear();
			//_contentItemPositionDictionary.Clear();
			_contentItemIndexDictionary.Clear();
			scrollRect.inertia = false;
			//add to temp remove
			GameObject go;
			int childCount = _transform.childCount;
			_contentItemRemovedList.Clear();
			for(int i = 0;i<childCount;i++)
			{
				go = _transform.GetChild(i).gameObject;
				go.SetActive(false);
				_contentItemRemovedList.Add(go);
			}
			
			//TransformUtil.ClearChildren(transform, true);
			//_shouldGenerateItems = true;
			_shouldPlayInitAnimation = shouldPlayInitAnimation;
			_isAdjustLoadLastContentHeight = true;
			_shouldPlayInitAnimationDelayTime = delay;
			GenerateItems();
			Invoke("InertiaDelay",0.1f);
		}
		private void InertiaDelay()
		{
			scrollRect.inertia = true;
		}
		public void RefreshCount(int count,int playActionStartIndex = -1)
		{
			int delta = count - _totalCount;
			_totalCount = count;

			_isAdjustLoadLastContentHeight = true;
			_isAdjustLoadFirstPosition = true;
			Vector2 prefabSize = contentItemPrefab.GetComponent<RectTransform>().rect.size;
			if(_isVertical){
				_contentHeight += prefabSize.y*delta;
				_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _contentHeight);
			}else{
				_contentWidth += prefabSize.x*delta;
				_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _contentWidth);
			}

			int maxItemCount =Mathf.Min( row * col,_totalCount);
			int contentListCount = _contentItemList.Count;
			int deltaCount = maxItemCount-contentListCount;
			for(int i = 0;i<deltaCount;i++)
			{
				GenerateItem(contentListCount+i);
			}
			if(deltaCount!= 0)
				_currentLastIndex = Mathf.Max( maxItemCount-1,0);
			if(delta < 0)//删除了列表元素
			{
				List<int> removeKeyList = new List<int>();
				foreach(var itemIndex in _contentItemIndexDictionary)
				{
					if(itemIndex.Key >= _totalCount)
					{
						removeKeyList.Add(itemIndex.Key);
					}
				}
				for(int i = 0;i<removeKeyList.Count;i++)
				{
					int key = removeKeyList[i];
					GameObject go = _contentItemIndexDictionary[key];
					int firstKey = _contentItemIndexDictionary.First().Key;
					if(firstKey != 0)
					{
						_contentItemIndexDictionary[firstKey-1] = go;
					}
					_contentItemIndexDictionary.Remove(key);
				}
                //int absDelta = Mathf.Abs(delta);

			}

			RefreshAllContentItems();

			//特殊移动效果
			Vector2 firstItemSizeDelta = Vector2.zero;
			if(playActionStartIndex != -1 && _contentItemIndexDictionary.Count > 0)
			{
				GameObject go = _contentItemIndexDictionary.First().Value;
				firstItemSizeDelta = go.GetComponent<RectTransform>().sizeDelta;
			}
			//特殊移动效果
			if(playActionStartIndex != -1)
			{
				GameObject go  = null;
				Vector3 localPos ;
				foreach(var value in _contentItemIndexDictionary)
				{
					if(playActionStartIndex <= value.Key)
					{
						go = value.Value;
						localPos = go.transform.localPosition;
						if(_isVertical)
						{
							go.transform.localPosition = localPos - new Vector3(0,firstItemSizeDelta.y+verticalSpacing);
							LeanTween.moveLocalY(go,localPos.y,0.3f).setDelay(0.01f);
						}else
						{
							go.transform.localPosition = localPos + new Vector3(firstItemSizeDelta.x+horizontalSpacing,0);
							LeanTween.moveLocalX(go,localPos.x,0.3f).setDelay(0.01f);
						}

					}
				}
				CreateMask(0.3f);
			}


			RemoveItemByTotalCountChange();
		}
		public void AddResetItemListener(UnityAction<GameObject,int> func)
		{
			onResetItem.AddListener(func);
		}
		
		private GameObject _mask = null;
		private void CreateMask(float destroyDelayTime)
		{
			if(_mask != null)
				return;
			Transform parent = GetComponentsInParent<Canvas>(true)[0].transform;
			GameObject go = new GameObject("_mask");
			Image image = go.AddComponent<Image>();
			RectTransform rt = image.rectTransform;
			rt.SetParent(parent,false);
			rt.anchoredPosition = Vector2.zero;
			rt.sizeDelta = new Vector2(1136,640);
			image.color  = new Color(1,1,1,0);
			_mask = go;
			Invoke("DestroyMask",destroyDelayTime);
		}
		private void DestroyMask()
		{
			if(_mask != null)
				GameObject.DestroyImmediate(_mask);
			_mask = null;
		}
		private void ResetItemIndex()
		{
			int count = Mathf.Min(_totalCount,_contentItemList.Count);
			_contentItemIndexDictionary.Clear();
			for(int i = 0;i<count;i++)
			{
				_contentItemIndexDictionary.Add(i,_contentItemList[i]);
			}
		}
		private void RemoveItemByTotalCountChange()
		{
			GameObject removeGO;
			List<GameObject> removeList = new List<GameObject>();
			int removeCount = _contentItemList.Count;
			_contentItemRemovedList.Clear();
			for(int i = 0;i<removeCount;i++)
			{
				removeGO = _contentItemList[i];
				if(! _contentItemIndexDictionary.ContainsValue(removeGO))
				{
					removeList.Add(removeGO);
				}
			}
			removeCount = removeList.Count;
			for(int i = 0;i<removeCount;i++)
			{
				removeGO = removeList[i];
				removeGO.SetActive(false);
				_contentItemList.Remove(removeGO);
				_contentItemRemovedList.Add(removeGO);
			}
		}
		private void GenerateItems ()
		{
				
			RectTransform itemRectTransfrom = contentItemPrefab.GetComponent<RectTransform>();

			int actualRow = Mathf.CeilToInt( _totalCount * 1.0f / col);

			int maxItemCount = Mathf.Min( row * col,_totalCount);

			_currentFirstIndex = 0;
			_currentLastIndex = Mathf.Max(maxItemCount-1,0);
			//_rectTransform.anchoredPosition3D = Vector3.zero;
			if(_isVertical)
			{
				_contentHeight = paddingTop + actualRow * itemRectTransfrom.rect.size.y + (actualRow - 1) * verticalSpacing + paddingBottom;
				//_contentHeight = Mathf.Abs( lastGo.transform.localPosition.y);
				_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _contentHeight);
			}else
			{
				int maxCol = _totalCount/row;
				_contentWidth = paddingLeft+ maxCol*itemRectTransfrom.rect.size.x + (maxCol -1)*horizontalSpacing ;
				//_contentHeight = Mathf.Abs( lastGo.transform.localPosition.x);
				_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _contentWidth);
			}

			int shouldGenerateItemCount = Mathf.Min(_totalCount, maxItemCount);

			if (shouldGenerateItemCount > 0)
			{
				GameObject lastGo = null;
				for (int i = 0; i < shouldGenerateItemCount; i++)
				{
					lastGo = GenerateItem(i);
				}
				if (!_shouldPlayInitAnimation)
				{
					OnInitComplete();
				}
			
			}
			else
			{
				OnInitComplete();
			}
			if(_shouldPlayInitAnimation)
			{
				GameObject go;
				for (int i = 0,count = _contentItemList.Count;i<count; i++)
				{
					go = _contentItemList[i];
					
					if (_shouldPlayInitAnimation)
					{
						float debutAnimationDelay = 0;
						if(_isVertical)
							debutAnimationDelay = (i / col) * 0.03f;
						else
							debutAnimationDelay = (i / row) * 0.03f;
						PlayDebutAnimation(go, debutAnimationDelay, i == shouldGenerateItemCount - 1);
					}
				}

			}

			RefreshAutoHide();
		}
		private GameObject GenerateItem(int index)
		{
			GameObject contentItemGameObject = null;
			if(_contentItemRemovedList.Count != 0)
			{
				contentItemGameObject = _contentItemRemovedList[0];
				_contentItemRemovedList.RemoveAt(0);
			}else
			{
				contentItemGameObject = GameObject.Instantiate(contentItemPrefab);
			}

			_contentItemList.Add(contentItemGameObject);
			//_contentItemIndexDictionary.Add(contentItemGameObject, index);
			_contentItemIndexDictionary[index] = contentItemGameObject;
			RectTransform rt = contentItemGameObject.GetComponent<RectTransform>();
			rt.SetParent(transform, false);
			contentItemGameObject.SetActive(true);
			rt.anchorMax = new Vector2(0,1);
			rt.anchorMin = new Vector2(0,1);
			ResetContentItem(contentItemGameObject, index);
			return contentItemGameObject;
		}


		public void ScrollToBottom(float time)
		{
			_scrollBarDesValue = 0;
			StopCoroutine("DoScrollToBottomCoroutine");
			StartCoroutine(DoScrollToBottomCoroutine(time));
		}
		public void ScrollToTop(float time)
		{
			_scrollBarDesValue = 1;
			StopCoroutine("DoScrollToBottomCoroutine");
			StartCoroutine(DoScrollToBottomCoroutine(time));
		}
		public void ScrollToPosition(float percent)
		{
			_scrollBarDesValue = percent;
			StopCoroutine("DoScrollToBottomCoroutine");
			StartCoroutine(DoScrollToBottomCoroutine(0.3f));
		}
		private float _scrollBarDesValue;
		private IEnumerator DoScrollToBottomCoroutine(float time)
		{
			float t = 0;
			float value = 0;
			while(true)
			{
				yield return null;
				if(value == 0)
					value = _scrollRect.verticalScrollbar.value;
				t += Time.deltaTime/time;

				_scrollRect.verticalScrollbar.value = Mathf.Lerp(value,_scrollBarDesValue,t);

				if(t >= 1)
					break;

			}
		}
		private void OnScroll(Vector2 vec)
		{
			//autoHide
			RefreshAutoHide();
			if( _isVertical && vec.y <= 0.001 && _contentItemIndexDictionary.Count > 0)
			{
				RectTransform rt = _contentItemIndexDictionary.Last().Value.transform as RectTransform;
				_contentHeight = Mathf.Abs( rt.localPosition.y)+rt.pivot.y*rt.rect.height+paddingBottom;
				_contentWidth = Mathf.Abs( rt.localPosition.x) + (1-rt.pivot.x)*rt.rect.width+horizontalSpacing ;
				_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,_contentHeight);
			}else if(!_isVertical && vec.x <=  0.001 && _contentItemIndexDictionary.Count > 0)
			{
				RectTransform rt = _contentItemIndexDictionary.Last().Value.transform as RectTransform;
				_contentHeight = Mathf.Abs( rt.localPosition.y)+rt.pivot.y*rt.rect.height+paddingBottom;
				_contentWidth = Mathf.Abs( rt.localPosition.x) + (1-rt.pivot.x)*rt.rect.width+horizontalSpacing ;
				_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,_contentWidth);
			}

		}
		private void RefreshAutoHide()
		{
			if(!autoHide)
				return;
			GameObject go;
			for(int i = 0,count = _contentItemList.Count;i<count;i++)
			{
				go = _contentItemList[i];
				go.SetActive(IsItemInViewPort(go));
				
			}
		}
		private bool IsItemInViewPort(GameObject go)
		{
			float max = 0;
			float min = -(_scrollRect.transform as RectTransform).rect.height;
			RectTransform rt;
			float parentY = transform.localPosition.y;
			rt = go.transform as RectTransform;
			float y = go.transform.localPosition.y+parentY;
			if(y-rt.rect.height * rt.pivot.y > max || y+rt.rect.height*(1-rt.pivot.y) < min)
			{
				return false;
			}else 
			{
				return true;
			}
		}

		private Vector3 _localPosition;
		void Update ()
		{
			if(refresh)
			{
				refresh = false;
				Init(_totalCount,false);
				return;
			}
			if (_shouldGenerateItems)
			{
				GenerateItems();
				_shouldGenerateItems = false;
			}
			_localPosition = _transform.localPosition;
			if(_isVertical)
			{
				if (_localPosition.y < _cachedLocalPosition.y)
				{
					OnScrollDown();
					//float y = Mathf.Clamp( _transform.localPosition.y,_cachedLocalPosition.y-30,_cachedLocalPosition.y);
					//_cachedLocalPosition = new Vector3(_localPosition.x,y,_localPosition.z);
				}
				else if (_localPosition.y > _cachedLocalPosition.y)
				{
					OnScrollUp();
					//float y = Mathf.Clamp( _transform.localPosition.y,_cachedLocalPosition.y,_cachedLocalPosition.y+30);
					//_cachedLocalPosition = new Vector3(_localPosition.x,y,_localPosition.z);
				}
			}else
			{
				if (_localPosition.x > _cachedLocalPosition.x)
				{
					OnScrollLeft();
					//float x = Mathf.Clamp( _transform.localPosition.x,_cachedLocalPosition.x,_cachedLocalPosition.x+30);
					//_cachedLocalPosition = new Vector3(x,_localPosition.y,_localPosition.z);
				}
				else if (_localPosition.x < _cachedLocalPosition.x)
				{
					OnScrollRight();
					//float x = Mathf.Clamp( _transform.localPosition.x,_cachedLocalPosition.x-30,_cachedLocalPosition.x);
					//_cachedLocalPosition = new Vector3(x,_localPosition.y,_localPosition.z);
				}
			}
			_cachedLocalPosition = _localPosition;
		}

		private void PlayDebutAnimation (GameObject contentItemGameObject, float delay, bool isLast)
		{

			if(_isVertical)
			{

				float startY = contentItemGameObject.transform.localPosition.y - _scrollRect.GetComponent<RectTransform>().rect.size.y;
				Vector3 startLocalPosition = contentItemGameObject.transform.localPosition;
				startLocalPosition.y = startY;
				float endY = contentItemGameObject.transform.localPosition.y;
				
				contentItemGameObject.transform.localPosition = startLocalPosition;
				LeanTween.cancel(contentItemGameObject);
				LTDescr ltDescr = LeanTween.moveLocalY(contentItemGameObject, endY, 0.6f);
				ltDescr.tweenType = LeanTweenType.easeInSine;
				ltDescr.setDelay(delay+_shouldPlayInitAnimationDelayTime);
				if (isLast)
				{
					ltDescr.setOnComplete(OnInitComplete);
				}
			}else
			{
				float startX = contentItemGameObject.transform.localPosition.x + _scrollRect.GetComponent<RectTransform>().rect.size.x;
				Vector3 startLocalPosition = contentItemGameObject.transform.localPosition;
				startLocalPosition.x = startX;
				float endX = contentItemGameObject.transform.localPosition.x;

				contentItemGameObject.transform.localPosition = startLocalPosition;
				LeanTween.cancel(contentItemGameObject);
				LTDescr ltDescr = LeanTween.moveLocalX(contentItemGameObject, endX, 0.6f);
				ltDescr.tweenType = LeanTweenType.easeInSine;
				ltDescr.setDelay(delay+_shouldPlayInitAnimationDelayTime);
				if (isLast)
				{
					ltDescr.setOnComplete(OnInitComplete);
				}
			}

			CreateMask(0.6f);
		}

		private void OnInitComplete ()
		{
			onInitComplete.Invoke();
		}

		private bool _isAdjustLoadLastContentHeight;
		private bool _isAdjustLoadFirstPosition;
//		private IEnumerator AdjustContentHeight()
//		{
//			if(_isAdjustLoadLastContentHeight)
//			{
//				_isAdjustLoadLastContentHeight = false;
//				RectTransform rt = GetComponent<RectTransform>();
//				float height = rt.rect.height;
//				float t = 0;
//				float h= 0;
//				bool isOver = false;
//				while(true)
//				{
//					t += 0.2f;
//					h = Mathf.Lerp(height,_contentHeight,t);
//					if(t >= 1)
//						isOver = true;
//					rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,h );
//					if(isOver)
//						break;
//					yield return null;
//				}
//			}
//		}
		private void ResetContentItem (GameObject contentItemGameObject, int newIndex,bool isAddBefore = false)
		{


			KeyValuePair<int,GameObject> v = GetContentItemIndexDictionaryPair(contentItemGameObject);
			_contentItemIndexDictionary.Remove(v.Key);
			_contentItemIndexDictionary[newIndex] = contentItemGameObject;
			contentItemGameObject.name = newIndex.ToString();
			onResetItem.Invoke(contentItemGameObject.gameObject, newIndex);

			contentItemGameObject.SetActive(true);
			if(_isVertical)
				ResetVerticalContentItem(contentItemGameObject,newIndex,isAddBefore);
			else 
				ResetHorizonContentItem(contentItemGameObject,newIndex,isAddBefore);
			//adjust content Height
			if(newIndex == _totalCount-1)
			{
				RectTransform rt = contentItemGameObject.transform as RectTransform;
				_contentHeight = Mathf.Abs( rt.localPosition.y)+rt.pivot.y*rt.rect.height+paddingBottom;
				_contentWidth = Mathf.Abs( rt.localPosition.x) + (1-rt.pivot.x)*rt.rect.width+horizontalSpacing ;
				if(_isVertical)
					_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,_contentHeight);
				else
					_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,_contentWidth);
			}
			//reset all position when load first object
			if(newIndex == 0 && _isAdjustLoadFirstPosition)
			{
				_isAdjustLoadFirstPosition = false;
				_isAdjustLoadLastContentHeight = false;
				RefreshAllContentItems();
			}
		}
		//排列顺序 横向
		//0 1 2
		//3 4 5
		//6 7 
		private void ResetVerticalContentItem(GameObject contentItemGameObject, int newIndex,bool isAddBefore = false)
		{
			if(!isVariableSize && _contentItemPositionDictionary.ContainsKey(newIndex))
			{
				contentItemGameObject.transform.localPosition  = _contentItemPositionDictionary[newIndex];
				return;
			}

			RectTransform itemRectTransfrom = contentItemGameObject.transform as RectTransform;
			_itemRectWidth = itemRectTransfrom.rect.width;
			_itemRectHeight = itemRectTransfrom.rect.height;
			Vector2 pivot = itemRectTransfrom.pivot;
			Vector3 itemLocalPosition = contentItemGameObject.transform.localPosition;
			int newcol = newIndex % col;
			int newRow = newIndex / col;
			if(newIndex == 0)
			{
				_firstContentItemPosition = new Vector2(_itemRectWidth * pivot.x, -(_itemRectHeight * (1-pivot.y))) + new Vector2(paddingLeft, -paddingTop);
				itemLocalPosition = _firstContentItemPosition;
			}else
			{
                //float x = 0;
                //float y = 0;
				GameObject go ;
				if(isAddBefore)
				{
					go = GetContentItem((newRow+1)*col+newcol);
					if(go != null)
					{
						RectTransform rt = go.transform as RectTransform;
						itemLocalPosition = go.transform.localPosition+new Vector3(0,rt.rect.height*(1-rt.pivot.y)+  _itemRectHeight*pivot.y + verticalSpacing);
					}else
					{
                        Debugger.Log("[ScrollContentExpand]AddBefore in 0 col,but go is null.index:"+newIndex);
                    }
				}else
				{
					if(newcol == 0)
					{
						go = GetContentItem((newRow-1)*col+newcol);
						if(go != null)
						{
							RectTransform rt = go.transform as RectTransform;
							itemLocalPosition = go.transform.localPosition+new Vector3(0,-(rt.rect.height*rt.pivot.y+  _itemRectHeight*(1-pivot.y) + verticalSpacing));
						}else
						{
							Debugger.Log("[ScrollContentExpand]AddAfter in 0 col ,but go is null.index:"+newIndex);
						}
					}else{
						if(newcol != 0)
						{
							go = GetContentItem(newIndex-1);
							if(go != null)
							{
								RectTransform rt = go.transform as RectTransform;
								itemLocalPosition = go.transform.localPosition+new Vector3(rt.rect.width*(1-rt.pivot.x)+ _itemRectWidth*pivot.x + horizontalSpacing,0);
                                
                            }else
                            {
                                Debugger.Log("[ScrollContentExpand]add in same row ,but go is null.index:"+newIndex);
                            }
                        }
					}
				}

			}
			contentItemGameObject.transform.localPosition  = itemLocalPosition;
			_contentItemPositionDictionary[newIndex] = itemLocalPosition;
		}
		//排列顺序 纵向
		//0 3 6
		//1 4 7
		//2 5 
		private void ResetHorizonContentItem(GameObject contentItemGameObject, int newIndex,bool isAddBefore = false)
		{
			if(!isVariableSize && _contentItemPositionDictionary.ContainsKey(newIndex))
			{
				contentItemGameObject.transform.localPosition  = _contentItemPositionDictionary[newIndex];
				return;
			}

			RectTransform itemRectTransfrom = contentItemGameObject.transform as RectTransform;
			_itemRectWidth = itemRectTransfrom.rect.width;
			_itemRectHeight = itemRectTransfrom.rect.height;
			Vector2 pivot = itemRectTransfrom.pivot;
			Vector3 itemLocalPosition = contentItemGameObject.transform.localPosition;
			//int maxcol = _totalCount/row;
			int newcol = newIndex / row;
			int newRow = newIndex % row;

			if(newIndex == 0)
			{
				_firstContentItemPosition = new Vector2(_itemRectWidth * pivot.x, -(_itemRectHeight * (1-pivot.y))) + new Vector2(paddingLeft, -paddingTop);
				itemLocalPosition = _firstContentItemPosition;
			}else
			{
                //float x = 0;
                //float y = 0;
				GameObject go ;
				if(isAddBefore)
				{
					go = GetContentItem((newcol+1)*row+newRow);
					if(go != null)
					{
						RectTransform rt = go.transform as RectTransform;
						itemLocalPosition = go.transform.localPosition-new Vector3(rt.rect.width*rt.pivot.x+ _itemRectWidth*(1-pivot.x) + horizontalSpacing,0);
					}else
					{
						Debugger.Log("[ScrollContentExpand]AddBefore in 0 row,but go is null.index:"+newIndex);
					}
				}else
				{
					if(newRow == 0)
					{
						go = GetContentItem((newcol-1)*row+newRow);
						if(go != null)
						{
							RectTransform rt = go.transform as RectTransform;
							itemLocalPosition = go.transform.localPosition+new Vector3(rt.rect.width*(1-rt.pivot.x)+ _itemRectWidth*pivot.x + horizontalSpacing,0);
						}else
						{
							Debugger.Log("[ScrollContentExpand]AddAfter in 0 row ,but go is null.index:"+newIndex);
						}
					}else
					{
						go = GetContentItem(newIndex-1);
						if(go != null)
						{
							RectTransform rt = go.transform as RectTransform;
							itemLocalPosition = go.transform.localPosition-new Vector3(0,rt.rect.height*rt.pivot.y+  _itemRectHeight*(1-pivot.y) + verticalSpacing);
							
						}else
						{
							Debugger.Log("[ScrollContentExpand]AddAfter in same col ,but go is null.index:"+newIndex);
						}
					}
                }
            }
            contentItemGameObject.transform.localPosition  = itemLocalPosition;
            _contentItemPositionDictionary[newIndex] = itemLocalPosition;
        }
		
		[NoToLua]
		public GameObject GetContentItem(int index)
		{
			if(!_contentItemIndexDictionary.ContainsKey(index))
				return null;

			return _contentItemIndexDictionary[index];
		}
		private KeyValuePair<int ,GameObject> GetContentItemIndexDictionaryPair(GameObject value)
		{
			KeyValuePair<int ,GameObject> def = new KeyValuePair<int, GameObject>(-1,null);
			foreach(KeyValuePair<int,GameObject> v in _contentItemIndexDictionary)
			{
				
				if(v.Value == value)
                {
                    return v;
                }
            }
            return def;
        }
		public void RefreshAllContentItems ()
		{

			List<int> removeList = new List<int>();


			List<GameObject> itemGoList = _contentItemIndexDictionary.GetValues();
			int count = itemGoList.Count;
			for(int i = 0;i<count;i++)
			{
				KeyValuePair<int,GameObject> pair = GetContentItemIndexDictionaryPair(itemGoList[i]);
				if(pair.Key == -1)
					continue;
				if(pair.Key >= _totalCount)
				{
					removeList.Add(pair.Key);
				}else
				{
					ResetContentItem(pair.Value, pair.Key);
				}
			}
			count = removeList.Count;
			for(int i = 0;i<count;i++)
			{
				_contentItemIndexDictionary.Remove(removeList[i]);
			}
		}
	
		public void RefreshContentItem(int index)
		{
			if(!_contentItemIndexDictionary.ContainsKey(index))
				return;
			ResetContentItem(_contentItemIndexDictionary[index],index);
		}
		
		[NoToLua]
		public List<T> GetContentItemComponentList<T> ()
		{
			List<T> contentItemComponentList = new List<T>();
			int contentItemCount = _contentItemList.Count;
			for (int i = 0; i < contentItemCount; i++)
			{
				contentItemComponentList.Add(_contentItemList[i].GetComponent<T>());
			}
			return contentItemComponentList;
		}

		private void OnScrollDown ()
		{
			if (_currentFirstIndex > 0)
			{
				GameObject go = GetEnabledUseGameObject(true);
				if(go!= null)
				{
					_currentFirstIndex --;
					_currentLastIndex --;
					ResetContentItem(go, _currentFirstIndex,true);
					OnScrollDown();
				}

			}
		}

		private void OnScrollUp ()
		{

			if (_currentLastIndex < _totalCount - 1)
			{
				GameObject go = GetEnabledUseGameObject(false);
				if(go!= null)
				{
					_currentLastIndex ++;
					_currentFirstIndex ++;
					ResetContentItem(go, _currentLastIndex,false);
					OnScrollUp();
				}

			}
		}
		private void OnScrollLeft ()
		{
			if (_currentFirstIndex > 0)
			{
				GameObject go = GetEnabledUseGameObject(true);
				if(go!= null)
				{
					_currentFirstIndex --;
					_currentLastIndex --;
					ResetContentItem(go, _currentFirstIndex,true);
					OnScrollLeft();
				}
				
			}
		}
		private void OnScrollRight ()
		{
			
			if (_currentLastIndex < _totalCount - 1)
			{
				GameObject go = GetEnabledUseGameObject(false);
				if(go!= null)
				{
					_currentLastIndex ++;
					_currentFirstIndex ++ ;
					ResetContentItem(go, _currentLastIndex,false);
					OnScrollRight();
				}
			}
		}
		private GameObject GetEnabledUseGameObject(bool addBefore)
		{
			if(_contentItemIndexDictionary.Count == 0)
				return null;
			if(contentRootWorldCorners == null)
			{
				contentRootWorldCorners = new Vector3[4];
				_scrollRect.GetComponent<RectTransform>().GetWorldCorners(contentRootWorldCorners);
			}

			GameObject contentItem = null;
			RectTransform contentItemRectTransform = null;
			if(!addBefore)
			{
				contentItem = _contentItemIndexDictionary.First().Value;
				contentItemRectTransform = contentItem.GetComponent<RectTransform>();
				Vector3[] contentItemWorldCorners = new Vector3[4];
				contentItemRectTransform.GetWorldCorners(contentItemWorldCorners);
				
				float verticalDiff = contentItemWorldCorners[0].y - contentRootWorldCorners[1].y;
				if (_isVertical && verticalDiff > 0)
				{
					return contentItem;
				}
				float horizonDiff = contentItemWorldCorners[3].x - contentRootWorldCorners[0].x;
				if( !_isVertical && horizonDiff < 0 )
				{
					return contentItem;
				}
			}else
			{
				contentItem = _contentItemIndexDictionary.Last().Value;
				contentItemRectTransform = contentItem.GetComponent<RectTransform>();
				Vector3[] contentItemWorldCorners = new Vector3[4];
				contentItemRectTransform.GetWorldCorners(contentItemWorldCorners);
				float verticalDiff = contentItemWorldCorners[1].y - contentRootWorldCorners[0].y;
				if(_isVertical && verticalDiff < 0)
				{
					return contentItem;
				}
				float horizonDiff = contentItemWorldCorners[0].x - contentRootWorldCorners[3].x;
				if(!_isVertical && horizonDiff > 0 )
				{
					return contentItem;
				}
			}


			return null;
		}
	}
}
