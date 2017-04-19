using UnityEngine;
using System.Collections.Generic;
using Logic.Team.Model;
using LuaInterface;

namespace Logic.Dungeon.Model
{
	public class DungeonInfo
	{
		public int id;
		public DungeonData dungeonData;
		public bool isLock = true;
		public int star;
		public int todayChallengedTimes;
		public int dayRefreshTimes;

		public DungeonInfo (int dungeonDataID, int star)
		{
			this.id = dungeonDataID;
			this.dungeonData = DungeonData.GetDungeonDataByID(dungeonDataID);
			this.star = star;
		}

		public DungeonInfo (LuaTable dungeonInfoLuaTable)
		{
			this.id = dungeonInfoLuaTable["id"].ToString().ToInt32();
			this.dungeonData = DungeonData.GetDungeonDataByID(this.id);
			this.isLock = dungeonInfoLuaTable["isLock"].ToString().ToBoolean();
			this.star = dungeonInfoLuaTable["star"].ToString().ToInt32();
			this.todayChallengedTimes = dungeonInfoLuaTable["todayChallengedTimes"].ToString().ToInt32();
			this.dayRefreshTimes = dungeonInfoLuaTable["dayRefreshTimes"].ToString().ToInt32();
		}

		public void UpdateBy (LuaTable dungeonInfoLuaTable)
		{
			this.isLock = dungeonInfoLuaTable["isLock"].ToString().ToBoolean();
			this.star = dungeonInfoLuaTable["star"].ToString().ToInt32();
			this.todayChallengedTimes = dungeonInfoLuaTable["todayChallengedTimes"].ToString().ToInt32();
			this.dayRefreshTimes = dungeonInfoLuaTable["dayRefreshTimes"].ToString().ToInt32();
		}

		public bool hasChallengeTimes()
		{
			if(dungeonData.dayChallengeTimes <= 0)
				return true;

			return dungeonData.dayChallengeTimes -todayChallengedTimes > 0;
		}
		public int GetRemindChallengeTimes()
		{
			if (dungeonData.dayChallengeTimes == -1)
			{
				return 999;
			}
			return dungeonData.dayChallengeTimes -todayChallengedTimes;
		}
	}
}
