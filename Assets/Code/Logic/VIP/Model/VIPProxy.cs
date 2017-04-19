using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

namespace Logic.VIP.Model
{
	public class VIPProxy : SingletonMono<VIPProxy>
	{
		private LuaTable _vipModelLuaTable;
		public LuaTable VIPModelLuaTable
		{
			get
			{
				if (_vipModelLuaTable == null)
					_vipModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "vip_model")[0];
				return _vipModelLuaTable;
			}
		}

		public int TotalRecharge
		{
			get
			{
				return VIPModelLuaTable["totalRecharge"].ToString().ToInt32();
			}
		}

		public int TotalRechargeDiamond
		{
			get
			{
				return TotalRecharge * 10;
			}
		}

		public int VIPLevel
		{
			get
			{
				return VIPModelLuaTable["vipLevel"].ToString().ToInt32();
			}
		}

        public bool dailyGetted
        {
			get
			{
				return VIPModelLuaTable["dailyGetted"].ToString().ToBoolean();
			}
        }

		public VIPData VIPData
		{
			get
			{
				return VIPData.GetVIPData(VIPLevel);
			}
		}
		public List<int> HasReceivedGiftVIPLevelList
		{
			get
			{
				List<int> result = new List<int>();
				LuaTable hasReceivedGiftVIPLevelListLuaTable = VIPModelLuaTable["hasReceivedGiftVIPLevelList"] as LuaTable;
				foreach (object level in hasReceivedGiftVIPLevelListLuaTable.ToArrayTable())
				{
					result.Add(level.ToString().ToInt32());
				}
				return result;
			}
		}

		public delegate void OnVIPInfoUpdateDelegate (int vipLevel, int totalRecharge, List<int> hasReceivedGiftVIPLevelList);
		public OnVIPInfoUpdateDelegate onVIPInfoUpdateDelegate;

		public delegate void OnDrawVIPBenefitsSuccessDelegate (int vipLevel);
		public OnDrawVIPBenefitsSuccessDelegate onDrawVIPBenefitsSuccessDelegate;

		void Awake ()
		{
			instance = this;
			Observers.Facade.Instance.RegisterObserver("OnVIPInfoUpdate", OnVIPInfoUpdate);
			Observers.Facade.Instance.RegisterObserver("OnDrawVIPBenefitsSuccess", OnDrawVIPBenefitsSuccess);
		}

		public bool OnVIPInfoUpdate (Observers.Interfaces.INotification note)
		{
			if (onVIPInfoUpdateDelegate != null)
			{
				onVIPInfoUpdateDelegate(VIPLevel, TotalRecharge, HasReceivedGiftVIPLevelList);
			}
			return true;
		}

		public bool OnDrawVIPBenefitsSuccess (Observers.Interfaces.INotification note)
		{
			if (onDrawVIPBenefitsSuccessDelegate != null)
			{
				onDrawVIPBenefitsSuccessDelegate((int)note.Body);
			}
			return true;
		}

		/*
		public void UpdateVIPInfo (bool dailyget,int totalRecharge, List<int> hasReceivedGiftVIPLevelList)
		{
            dailyGetted = dailyget;
			_totalRecharge = totalRecharge;
			_hasReceivedGiftVIPLevelList = hasReceivedGiftVIPLevelList;

			int vipLevel = 0;
			List<VIPData> allVIPDataList = VIPData.GetAllVIPDataList();
			int vipDataCount = allVIPDataList.Count;
			for (int i = 0; i < vipDataCount; i++)
			{
				if (_totalRecharge >= allVIPDataList[i].exp)
				{
					vipLevel = allVIPDataList[i].id;
				}
				else
				{
					break;
				}
			}
			_vipLevel = vipLevel;

			if (onVIPInfoUpdateDelegate != null)
			{
				onVIPInfoUpdateDelegate(_vipLevel, _totalRecharge, _hasReceivedGiftVIPLevelList);
			}
            Logic.UI.SoftGuide.Model.SoftGuideProxy.instance.UpdateSoftGuide();
		}
		*/

		/*
		public void DrawVIPBenefitsSuccess (int vipLevel)
		{
			if (onDrawVIPBenefitsSuccessDelegate != null)
			{
				onDrawVIPBenefitsSuccessDelegate(vipLevel);
			}
		}
		*/

		public int GetRechargeDiamondToNextVIPLevel ()
		{
			int rechargeDiamondToNextVIPLevel = 0;
			if (!VIPData.IsMaxLevelVIPData())
			{
				VIPData nextVIPData = VIPData.GetNextLevelVIPData ();
				rechargeDiamondToNextVIPLevel = (nextVIPData.exp - VIPProxy.instance.TotalRecharge) * 10;
			}
			return rechargeDiamondToNextVIPLevel;
		}
	}
}
