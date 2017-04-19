using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.Role.Model;
using Common.Util;
using Logic.UI.IllustratedHandbook.Model;

namespace Logic.UI.IllustratedHandbook.View
{
	public class IllustratedContentView : MonoBehaviour 
	{
		
		private List<IllustrationInfo> _roleInfoList;

		#region ui component
		public RectTransform root;
		public RectTransform titleRoot;
		public Text titleText;
		public RectTransform contentRoot;
		public IllustratedHeroButton heroButtonPrefab;
		#endregion
		private bool _isHideTitle = false;
		void Awake()
		{
		}
	
		public void SetData(string title,List<IllustrationInfo> roleList,bool hideTitle = false)
		{
			heroButtonPrefab.gameObject.SetActive(false);
			titleText.text = title;
			_roleInfoList = roleList;
			_isHideTitle = hideTitle;
			titleRoot.gameObject.SetActive(!_isHideTitle);
			UpdateList();

		}

		private void UpdateList()
		{
			//TransformUtil.ClearChildren(contentRoot,true);
			RoleInfo roleInfo ;
			int count = _roleInfoList.Count;
			int childCount = contentRoot.childCount;
			Transform childTran;

			//StopAllCoroutines();
			//update
			for(int i = 0;i<count && i < childCount;i++)
			{
				roleInfo = _roleInfoList[i].roleInfo;
				childTran = contentRoot.GetChild(i);
				childTran.gameObject.SetActive(true);
				IllustratedHeroButton child = childTran.GetComponent<IllustratedHeroButton>();
				bool isGot = IllustratedHandbookProxy.instance.isHeroGotInIllustration(roleInfo.modelDataId,roleInfo.advanceLevel);
				child.SetRoleInfo(roleInfo);
				child.ShowMask(!isGot);
			}
			CreateItems(childCount);
			//StartCoroutine(CreateItemsCoroutine(childCount));
			//hide child
			for(int i = count;i<childCount;i++)
			{
				contentRoot.GetChild(i).gameObject.SetActive(false);
			}
			//update size
			GridLayoutGroup grid = contentRoot.GetComponent<GridLayoutGroup>();
			if( _isHideTitle)
			{
				int row = count/grid.constraintCount + (count % grid.constraintCount == 0 ? 0 : 1);
				float h = grid.cellSize.y*row + grid.spacing.y*(row-1);
				Vector2 rootSize = root.sizeDelta;
				root.sizeDelta = new Vector2(rootSize.x,Mathf.Abs( contentRoot.anchoredPosition.y)+h);
			}else
			{
				Vector2 rootSize = root.sizeDelta;
				root.sizeDelta = new Vector2(rootSize.x,Mathf.Abs( titleRoot.anchoredPosition.y)+titleRoot.sizeDelta.y);
			}

		}
		private IEnumerator UpdateItemsCoroutine()
		{
			RoleInfo roleInfo ;
			int count = _roleInfoList.Count;
			int childCount = contentRoot.childCount;
			Transform childTran;
			for(int i = 0;i<count && i < childCount;i++)
			{
				roleInfo = _roleInfoList[i].roleInfo;
				childTran = contentRoot.GetChild(i);
				childTran.gameObject.SetActive(true);
				IllustratedHeroButton child = childTran.GetComponent<IllustratedHeroButton>();
				bool isGot = IllustratedHandbookProxy.instance.isHeroGotInIllustration(roleInfo.modelDataId,roleInfo.advanceLevel);
				child.SetRoleInfo(roleInfo);
				child.ShowMask(!isGot);
				yield return null;
			}
		}
		private void CreateItems(int start)
		{
			RoleInfo roleInfo ;
			int count = _roleInfoList.Count;
			for(int i = start;i<count;i++)
			{
				roleInfo = _roleInfoList[i].roleInfo;
				IllustratedHeroButton icon = Instantiate<IllustratedHeroButton>(heroButtonPrefab);
				icon.gameObject.SetActive(true);
				icon.transform.SetParent(contentRoot,false);
				bool isGot = IllustratedHandbookProxy.instance.isHeroGotInIllustration(roleInfo.modelDataId,roleInfo.advanceLevel);
				icon.SetRoleInfo(roleInfo);
				icon.ShowMask(!isGot);
			}
		}
//		private IEnumerator CreateItemsCoroutine(int start)
//		{
//			RoleInfo roleInfo ;
//			int count = _roleInfoList.Count;
//			for(int i = start;i<count;i++)
//			{
//				roleInfo = _roleInfoList[i].roleInfo;
//				IllustratedHeroButton icon = Instantiate<IllustratedHeroButton>(heroButtonPrefab);
//				icon.gameObject.SetActive(true);
//				icon.transform.SetParent(contentRoot,false);
//				bool isGot = IllustratedHandbookProxy.instance.isHeroGotInIllustration(roleInfo.modelDataId,roleInfo.advanceLevel);
//				icon.SetRoleInfo(roleInfo);
//				icon.ShowMask(!isGot);
//				yield return null;
//			}
//		}
	}
}

