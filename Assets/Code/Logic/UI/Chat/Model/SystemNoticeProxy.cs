using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Common.Localization;
using Logic.Hero.Model;
using Logic.Equipment.Model;

namespace Logic.UI.Chat.Model
{
	public class SystemNoticeProxy : SingletonMono<SystemNoticeProxy> 
	{

		public Action<float,string> onUpdateSystemNoticeDelegate;
		public System.Action<string> onUpdateStartNoticeDelegate;
		public System.Action onUpdateEndNoticeDelegate;
		public bool isStartNotice;

		public string currentString//当前显示的文字
		{
			get{
				return _currentString;
			}
		}
		public float positionPercent//当前文字的位置
		{
			get
			{
				return _positionPercent;
			}
		}
		//如果文字长度比背景长度小，就忽略移出动画
		public void IgnoreMoveOutTime(bool value)
		{
			_ignoreMoveOutTime = value;
		}

		private float _moveInTime = 3f;
		private float _stayTime = 4f;
		private float _moveOutTime = 2f;
		private float _moveOutStayTime = 2f;
		private Queue<string> _systemNoticeQueue = new Queue<string>();
		private int _showState;
		private string _currentString = string.Empty;//当前显示的文字
		private float _positionPercent;//当前文字的位置
		private bool _ignoreMoveOutTime = false;
		void Awake()
		{
			instance = this;
		}
		public void Clear()
		{
			LeanTween.cancel(gameObject);
			_systemNoticeQueue.Clear();
			_showState = 4;
			isStartNotice =  false;
			if(onUpdateEndNoticeDelegate != null)
				onUpdateEndNoticeDelegate();
		}

		public void AddSystemNotice(string s)
		{
			_systemNoticeQueue.Enqueue(s);
			if(_showState >= 4)
				_showState = 0;

		}
		public void AddSystemNotice(int id,List<string> param = null)
		{

			BroadCastData data = BroadCastData.GetBroadcastDataById(id);
			if(data == null)
				return;
			string log = "[AddSystemNotice]id "+id;
			for(int i  = 0;i< param.Count;i++)
			{
				log += "  ,param:"+param[i];
			}
			Debugger.Log(log);
			string s = string.Empty;
			if(param == null || param.Count == 0)
			{
				s = Localization.Get(data.des);
			}else
			{
				if (data.id == 2001 && param.Count >= 2 )//恭喜{0}玩家获得橙色英雄{1}，从此之后实力更进一步
				{
					int heroId =  param[1].ToInt32();
					HeroData heroData = HeroData.GetHeroDataByID(heroId);
					if(heroData != null)
						s = string.Format(Localization.Get(data.des),param[0],UIUtil.FormatStringWithinQualityColor(heroData.roleQuality, Localization.Get( heroData.name)));
					else
						Debugger.Log("can not find herodata by id:"+heroId);
				}else if (data.id == 2002 && param.Count >= 3 )//
				{
					int heroId = param[1].ToInt32();
					HeroData heroData = HeroData.GetHeroDataByID(heroId);
					if(heroData != null)
						s = string.Format(Localization.Get(data.des), param[0], Localization.Get( heroData.name), param[2]);
					else
						Debugger.Log("can not find herodata by id:"+heroId);
				}else if (data.id == 2003 && param.Count >= 2)//恭喜{0}玩家获得橙色品质装备{1}，如虎添翼，战无不胜
				{
					int equipId = param[1].ToInt32();
					EquipmentData equipData = EquipmentData.GetEquipmentDataByID(equipId);
					if (equipData != null)
						s = string.Format(Localization.Get(data.des),param[0],UIUtil.FormatStringWithinQualityColor(equipData.quality, Localization.Get(equipData.name)));
					else
						Debugger.Log("can not find equipdata by id:"+equipId);
				}
				else
				{
					s = string.Format(Localization.Get(data.des),param.ToArray());
				}
			}
			AddSystemNotice(s);
		}
		public void AddSystemCycleNotice(int id,List<string> param = null)
		{
			BroadCastData data = BroadCastData.GetBroadcastDataById(id);
			if(data != null)
			{
				StartCoroutine(CycleNoticeCoroutine(data,param));
			}
		}
		private IEnumerator CycleNoticeCoroutine(BroadCastData data,List<string> param)
		{
			int repeat = data.repeat_times;
			int per = data.repeat_every_time;
			string s = string.Empty;
			if(param == null || param.Count == 0)
			{
				s = Localization.Get(data.des);
			}else
			{
				s = string.Format(Localization.Get(data.des),param.ToArray());
			}

			while(true)
			{
				AddSystemNotice(s);
				repeat --;
				if(repeat <= 0)
					break;
				yield return new WaitForSeconds(per);
			}
		}
		void Update()
		{
			
			switch(_showState)
			{
			case 0://start in
			{

				if(_systemNoticeQueue.Count == 0)
				{
					UpdateNoticeView(1,"");
					_showState = 4;//完成啦
					break;
				}

				isStartNotice = true;
				_currentString = _systemNoticeQueue.Dequeue();
				if(onUpdateStartNoticeDelegate!= null)
					onUpdateStartNoticeDelegate(_currentString);

				UpdateNoticeView(1,_currentString);
				OnUpdateFloat(0);
				MoveInComplete();
				//LeanTween.value(gameObject, 1, 0, _moveInTime).setOnUpdate(OnUpdateFloat).setEase(LeanTweenType.easeOutSine).onComplete = MoveInComplete;
				_showState = 1;
			}
				break;
			case 1://wait for second
				break;
			case 2://move out
			{
				if(!_ignoreMoveOutTime)
				{
					LeanTween.value(gameObject, 0, -1, _moveOutTime).setOnUpdate(OnUpdateFloat).setEase(LeanTweenType.easeOutSine).onComplete = MoveOutComplete;
					_ignoreMoveOutTime = false;
				}else
				{
					MoveOutComplete();
				}
				_showState = 3;
			}
				break;
			case 3://wait for second
				break;
			}
			
		}
		private void OnUpdateFloat(float t)
		{
			_positionPercent  = t;
			UpdateNoticeView(t,_currentString);
		}
		private void MoveInComplete()
		{
			Invoke("AddState",_stayTime);
		}
		private void MoveOutComplete()
		{
			Invoke("DoMoveOut",_moveOutStayTime);

		}
		private void DoMoveOut()
		{
			if(_systemNoticeQueue.Count > 0)
			{
				_showState = 0;
			}else
			{
				_showState = 4;
				isStartNotice = false;
				if(onUpdateEndNoticeDelegate != null)
					onUpdateEndNoticeDelegate();
			}
		}
		private void AddState()
		{
			_showState++;
		}
		private void UpdateNoticeView(float t,string s)
		{
			if(onUpdateSystemNoticeDelegate != null)
			{
				onUpdateSystemNoticeDelegate(t,s);
			}
		}

	}
}

