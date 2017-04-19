using UnityEngine;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.Enums;

namespace Logic.Activity.Model
{
	public class ActivityProxy : SingletonMono<ActivityProxy>
	{
		private Dictionary<int, ActivityInfo> _activityInfoDictionary = new Dictionary<int, ActivityInfo>();

		public List<DropItem> fixedRewardGameResDataList = new List<DropItem>();
		
		#region delegates
		public delegate void OnActivyChallengeOverDelegate();
		public OnActivyChallengeOverDelegate onActivyChallengeOverDelegate;

		public delegate void OnActivityAwardMultipledDelegate();
		public OnActivityAwardMultipledDelegate onActivityAwardMultipledDelegate;
		#endregion delegates

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			List<ActivityData> allActivityDataList = ActivityData.GetAllActivityData();
			int activityDataCount = allActivityDataList.Count;
			for (int i = 0; i < activityDataCount; i++)
			{
				ActivityInfo activityInfo = new ActivityInfo(allActivityDataList[i]);
				_activityInfoDictionary.Add(activityInfo.ActivityID, activityInfo);
			}
		}

		public List<ActivityInfo> GetAllActivityInfoList ()
		{
			return _activityInfoDictionary.GetValues();
		}

		public List<ActivityInfo> GetEnabledActivityInfoList ()
		{
			List<ActivityInfo> enabledActivityInfoList = new List<ActivityInfo>();

			List<ActivityInfo> allActivityInfoList = GetAllActivityInfoList();
			int allActivityInfoCount = allActivityInfoList.Count;
			for (int i = 0 ; i < allActivityInfoCount; i++)
			{
				if (allActivityInfoList[i].ActivityData.isEnabled)
				{
					enabledActivityInfoList.Add(allActivityInfoList[i]);
				}
			}
			return enabledActivityInfoList;
		}

		public bool HaveNotChallengeAnyActivityToday ()
		{
			List<ActivityInfo> allActivityInfoList = GetAllActivityInfoList();
			for (int i = 0, count = allActivityInfoList.Count; i < count; i++)
			{
				if (allActivityInfoList[i].isOpen && (allActivityInfoList[i].remainChallengeTimes < allActivityInfoList[i].ActivityData.count))
				{
					return false;
				}
			}
			return true;
		}

		public void ResetAllActivityInfos ()
		{
			List<ActivityInfo> allActivityInfoList = GetAllActivityInfoList();
			int allActivityCount = allActivityInfoList.Count;
			for (int i = 0; i < allActivityCount; i++)
			{
				allActivityInfoList[i].Reset();
			}
		}

		public void RefreshActivityInfos (List<ActivityPveProto> activityPveProtoList)
		{
			ResetAllActivityInfos();
			int activityPveProtoCount = activityPveProtoList.Count;
			for (int i = 0; i < activityPveProtoCount; i++)
			{
				ActivityPveProto activityPveProto = activityPveProtoList[i];
				ActivityInfo activityInfo = _activityInfoDictionary[activityPveProto.id];
				activityInfo.Refresh(activityPveProto);
			}

		}

//		public void OnActivityChallengeOver (List<DropItem> fixedDrops, List<DropItem> drawDrops)
//		{
//			fixedRewardGameResDataList = fixedDrops;
//			drawRewardGameResDataList = drawDrops;
//			if (onActivyChallengeOverDelegate != null)
//			{
//				onActivyChallengeOverDelegate();
//			}
//		}

		// wangxf
		public void OnActivityChallengeOver (List<DropItem> fixedDrops)
		{
			fixedRewardGameResDataList = fixedDrops;
			if (onActivyChallengeOverDelegate != null)
			{
				onActivyChallengeOverDelegate();
			}
		}

		public void OnActivityPveAwardMultipled ()
		{
			if (onActivityAwardMultipledDelegate != null)
			{
				onActivityAwardMultipledDelegate();
			}
		}
	}
}