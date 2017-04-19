using UnityEngine;
using System.Collections;
using Logic.Hero.Model;
using LuaInterface;

namespace Logic.UI.HeroAdvance.Model
{
	public class HeroAdvanceProxy : SingletonMono<HeroAdvanceProxy> 
	{
		private static LuaTable _heroAdvanceModelLuaTable;
		public static LuaTable HeroAdvanceModelLuaTable
		{
			get
			{
				if (_heroAdvanceModelLuaTable == null)
					_heroAdvanceModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "hero_advance_model")[0];
				return _heroAdvanceModelLuaTable;
			}
		}

		public HeroInfo advanceHeroInfo;

		public delegate void OnUpdateHeroAdvanceByProtocol(bool isSuccess);
		public OnUpdateHeroAdvanceByProtocol OnUpdateHeroAdvanceByProtocolDelegate;
				
		void Awake()
		{
			instance = this;
		}

		public void Start ()
		{
			Observers.Facade.Instance.RegisterObserver("UpdateHeroAdvanceByProtocol", UpdateHeroAdvanceByProtocol);
		}

		public void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver("UpdateHeroAdvanceByProtocol", UpdateHeroAdvanceByProtocol);
			}
		}

//		public void UpdateHeroAdvanceByProtocol(bool isSuccess)
//		{
//			if(OnUpdateHeroAdvanceByProtocolDelegate != null)
//				OnUpdateHeroAdvanceByProtocolDelegate(isSuccess);
//		}

		public bool UpdateHeroAdvanceByProtocol(Observers.Interfaces.INotification note)
		{
			bool isSuccess = true;
			if(OnUpdateHeroAdvanceByProtocolDelegate != null)
				OnUpdateHeroAdvanceByProtocolDelegate(isSuccess);
			return true;
		}
	}
}

