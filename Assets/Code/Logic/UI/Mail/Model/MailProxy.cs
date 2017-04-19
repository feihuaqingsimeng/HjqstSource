using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.FunctionOpen.Model;
using Logic.Enums;

namespace Logic.UI.Mail.Model
{
	public class MailProxy : SingletonMono<MailProxy> 
	{

		public System.Action<int> onUpdateGetOneMailRewardSuccessDelegate;
		public System.Action onUpdateGetAllMailRewardSuccessDelegate;
		public System.Action onRefreshAllDelegate;

		public MailInfo GetOneMailRewardInfo;
		public bool hasNewMailComing;
		private Dictionary<int ,MailInfo> _mailInfoDictionary = new Dictionary<int, MailInfo>();
		public Dictionary<int ,MailInfo> MailInfoDictionary
		{
			get
			{
				return _mailInfoDictionary;
			}
		}

		private List<MailInfo> _currentMailInfoList = new List<MailInfo>();


		public List<MailInfo> CurrentMailInfoList
		{
			get
			{
				return _currentMailInfoList;
			}

		}

		void Awake()
		{
			instance = this;
		}

		public List<MailInfo> GetMailInfoListByType()
		{
			_currentMailInfoList = _mailInfoDictionary.GetValues();
			_currentMailInfoList.Sort(CompareMailInfo);
			return _currentMailInfoList;
		}
		private static int CompareMailInfo(MailInfo info1,MailInfo info2)
		{
			int weight1 = info1.isGetReward ? 1 : 0 ;
			int weight2 = info2.isGetReward ? 1 : 0 ;

			int w = weight1-weight2;
			if(w != 0)
				return w;
			return (int)( info1.createTime-info2.createTime);
		}
		public void AddAllMail(List<MailProtoData> dataList)
		{
			_mailInfoDictionary.Clear();
			MailProtoData proto;
			for(int i = 0,count = dataList.Count;i<count;i++)
			{
				proto = dataList[i];
				_mailInfoDictionary.Add(proto.id,new MailInfo(proto));
			}
		}

		public void UpdateMail(MailProtoData data)
		{
			if(_mailInfoDictionary.ContainsKey(data.id))
			{
				_mailInfoDictionary[data.id].Update(data);
			}
		}
		public void UpdateMail(List<MailProtoData> dataList)
		{
			for(int i = 0,count = dataList.Count;i<count;i++)
			{
				UpdateMail(dataList[i]);
			}
		}
		public void DeleteMail(List<int> idList)
		{
			int id;
			for(int i = 0,count = idList.Count;i<count;i++)
			{
				id = idList[i];
				if(_mailInfoDictionary.ContainsKey(id))
				{
					_mailInfoDictionary.Remove(id);
				}
			}
		}
		public int GetNotGetRewardMailCount()
		{
			if(!FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Mail))
				return 0;

			if(hasNewMailComing)
				return 1;
			int num = 0;
			foreach(KeyValuePair<int,MailInfo> value in _mailInfoDictionary)
			{
				if(!value.Value.isGetReward && value.Value.rewardList.Count!=0)
				{
					num ++;
				}
			}
			return num;
		}
		#region server update
		public void RefreshAllByProtocol()
		{
			if(onRefreshAllDelegate!= null)
				onRefreshAllDelegate();
		}
		public void UpdateGetAllMailRewardSuccessByProtocol()
		{
			if(onUpdateGetAllMailRewardSuccessDelegate!=null)
				onUpdateGetAllMailRewardSuccessDelegate();
		}
		public void UpdateGetOneMailRewardSuccessByProtocol()
		{
			if(onUpdateGetOneMailRewardSuccessDelegate != null)
				onUpdateGetOneMailRewardSuccessDelegate(GetOneMailRewardInfo.id);
		}
		#endregion
	}
}

