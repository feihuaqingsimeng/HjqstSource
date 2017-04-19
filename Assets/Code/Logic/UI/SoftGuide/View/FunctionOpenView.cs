using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Logic.UI.CommonAnimations;
using Logic.FunctionOpen.Model;
using Common.Localization;
using Common.ResMgr;

namespace Logic.UI.SoftGuide.View
{
	public class FunctionOpenView : MonoBehaviour 
	{

		public const string PREFAB_PATH = "ui/soft_guide/function_open_view";
		public static FunctionOpenView Open(int functionId)
		{
			
			FunctionOpenView view = UIMgr.instance.Open<FunctionOpenView>(PREFAB_PATH, EUISortingLayer.Notice);
			view.SetData(functionId);
			return view;
		}
		public static FunctionOpenView Open(List<int> idList)
		{
			
			FunctionOpenView view = UIMgr.instance.Open<FunctionOpenView>(PREFAB_PATH, EUISortingLayer.Notice);
			view.SetDataList(idList);
			return view;
		}
		public static void Close()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
		#region ui component
		public Text textName;
		public Image imgIcon;
		public GameObject root;
		#endregion
		
		private List<int> _openList = new List<int>();
		private bool _isStartCoroutine = false;
		public void SetData(int id)
		{
			_openList.Add(id);
			if(!_isStartCoroutine)
			{
				StartCoroutine(RefreshCoroutine());
				_isStartCoroutine = true;
			}
			
		}
		public void SetDataList(List<int> idList)
		{
			_openList.AddRange(idList);
			if(!_isStartCoroutine)
			{
				_isStartCoroutine = true;
				StartCoroutine(RefreshCoroutine());
			}
		}
		private IEnumerator RefreshCoroutine()
		{
			for(int i = 0;i<_openList.Count;i++)
			{
				int id = _openList[i];
				Refresh(id);
				yield return new WaitForSeconds(2.5f);
				root.SetActive(false);
				yield return new WaitForSeconds(0.1f);
			}
			Destroy();
		}
		
		private void Refresh(int id)
		{
			root.SetActive(true);
			CommonFadeToAnimation.Get(root).init(0,1,0.2f);
			FunctionData data = FunctionData.GetFunctionDataByID(id);

			textName.text = Localization.Get(data.show_new_des);
			imgIcon.SetSprite(ResMgr.instance.Load<Sprite>(data.show_new_pic));
		}
		private void Destroy()
		{
			UIMgr.instance.Close(PREFAB_PATH);
		}
	}
}
