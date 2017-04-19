using UnityEngine;
using System.Collections;

namespace Logic.UI.Expedition.Model
{
	public class ExpeditionDungeonInfo 
	{
		public int id;
		public ExpeditionData data;
		public bool isFinished;
		public bool isGetReward;
		public bool isUnlocked;
		public ExpeditionDungeonInfo(int dataid,bool isFinished,bool isGetReward,bool isUnlocked)
		{
			Set(dataid,isFinished,isGetReward,isUnlocked);
		}
		public void Set(int dataid,bool isFinished,bool isGetReward,bool isUnlocked)
		{
			id = dataid;
			data = ExpeditionData.GetExpeditionDataByID(dataid);
			this.isFinished = isFinished;
			this.isGetReward = isGetReward;
			this.isUnlocked = isUnlocked;
		}
		public void SetFinished(bool isFinished)
		{
			this.isFinished = isFinished;
		}
		public void SetGetReward(bool isGetReward)
		{
			this.isGetReward = isGetReward;
		}
		public void SetUnlocked(bool isUnlocked)
		{
			this.isUnlocked = isUnlocked;
		}

	}
}

