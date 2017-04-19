using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.Enums;
using Logic.FunctionOpen.Model;
using Logic.UI;
using Logic.UI.Mask.View;


public class UnlockTranslateAnimation : MonoBehaviour {


	public List<Transform> transformList;
	public List<FunctionOpenType> functionOpenList;
	public float moveTime = 0.5f;
	public float stayTime = 0.3f;

	private Dictionary<int,Vector3> originPositionDic = new Dictionary<int, Vector3>();
	private bool _isExecute = false;//是否正在执行动画
	private Dictionary<int,bool> _curOpenIdDic = new Dictionary<int,bool>();
	void Start () 
	{
		if(functionOpenList.Count != transformList.Count)
		{
			Debugger.LogError("transformList count is not equal functionOpenList count,please fix it!!!  it's very important!!!");
		}
		for(int i = 0,count = transformList.Count;i<count;i++)
		{
			originPositionDic.Add(i,transformList[i].localPosition);
		}

		FunctionOpenProxy.instance.FunctionAnimationDelegate += SetData;

		Refresh();
	}
	void OnDestroy()
	{
		FunctionOpenProxy.instance.FunctionAnimationDelegate -= SetData;
	}
	private void SetData(List<int> openIdList)
	{
		if (_isExecute)
			return;
		_isExecute = true;
		_curOpenIdDic.Clear();
		for(int i = 0,count = openIdList.Count;i<count;i++)
		{
			if (functionOpenList.Contains((FunctionOpenType)openIdList[i]))
			{
				_curOpenIdDic.Add(openIdList[i],true);
			}
		}
		Refresh();
	}

	private void Refresh()
	{
		int id = 0;
		int index = 0;
		for(int i = 0,count = transformList.Count;i<count;i++)
		{
			id = (int)functionOpenList[i];

			if(_curOpenIdDic.ContainsKey(id))
			{
				transformList[i].gameObject.SetActive(false);
				continue;
			}
				
			FunctionData data = FunctionData.GetFunctionDataByID(id);

			if (!FunctionOpenProxy.instance.IsFunctionOpen(functionOpenList[i]))//未开放
			{
				if(data.show_animation_status == 0)//正常显示
				{
					transformList[i].localPosition = originPositionDic[index];
					index ++;
				}else
				{
					transformList[i].localPosition = originPositionDic[i];
					transformList[i].gameObject.SetActive(false);
				}
			}else//开放
			{
				transformList[i].gameObject.SetActive(true);
				transformList[i].localPosition = originPositionDic[index];
				index ++;
			}
		}
		if (_curOpenIdDic.Count > 0)
			StartCoroutine(StartAnimationCoroutine());
		else
			_isExecute = false;
		
	}
	private IEnumerator StartAnimationCoroutine()
	{
		BlockView.Open();
		foreach (var value in _curOpenIdDic)
		{
			int index = functionOpenList.IndexOf((FunctionOpenType)value.Key);

			Debugger.Log("[StartAnimation]insert index:{0}",index);
			Translate(index);

			yield return new WaitForSeconds(moveTime);
			transformList[index].gameObject.SetActive(true);
			yield return new WaitForSeconds(stayTime);
		}
		BlockView.Close();
		_isExecute = false;
	}

	private void Translate(int index)
	{
		Transform tran;
		int moveIndex = 0;
		for (int i = 0,count = transformList.Count;i<count;i++)
		{
			tran = transformList[i];
			if (!tran.gameObject.activeSelf)
			{
				tran.localPosition = originPositionDic[moveIndex];
				continue;
			}
			if (i >= index)
			{
				if(moveIndex == count -1)
					tran.localPosition = originPositionDic[moveIndex];
				else
					LeanTween.moveLocal(tran.gameObject,originPositionDic[moveIndex+1],moveTime);
			}
			moveIndex ++;
		}
	}
	private GameObject _mask = null;
}
