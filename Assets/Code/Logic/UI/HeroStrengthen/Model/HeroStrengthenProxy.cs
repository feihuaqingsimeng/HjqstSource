using UnityEngine;
using System.Collections;
using Logic.Enums;
using System.Collections.Generic;
using Logic.Hero.Model;
using Logic.Hero;
using Logic.Player.Model;
using Logic.Role.Model;
using LuaInterface;
using Logic.Game.Model;

namespace Logic.UI.HeroStrengthen.Model{
	public class HeroStrengthenProxy : SingletonMono<HeroStrengthenProxy> {
		private static LuaTable _heroStrengthenModelLuaTable;
		public static LuaTable HeroStrengthenModelLuaTable
		{
			get
			{
				if (_heroStrengthenModelLuaTable == null)
					_heroStrengthenModelLuaTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "hero_strengthen_model")[0];
				return _heroStrengthenModelLuaTable;
			}
		}

		public delegate void HeroStrengthenSuccess(bool isCrit);

		public HeroStrengthenSuccess onHeroStrengthenSuccessDelegate;


		private HeroInfo[] _selectedMaterialHeroInfos = new HeroInfo[4];

		public HeroInfo[] SelectedMaterialHeroInfos{
			get{
				return _selectedMaterialHeroInfos;
			}
		}

//		private RoleInfo _StrengthenHeroInfo;
		private uint _strengthenHeroInstanceID;
		public RoleInfo StrengthenHeroInfo
		{
			set
			{
//				_StrengthenHeroInfo = value;
				_strengthenHeroInstanceID = value.instanceID;
			}
			get
			{
//				return _StrengthenHeroInfo;
				RoleInfo roleInfo = null;
				if (GameProxy.instance.IsPlayer(_strengthenHeroInstanceID))
					roleInfo = GameProxy.instance.PlayerInfo;
				else
					roleInfo = HeroProxy.instance.GetHeroInfo(_strengthenHeroInstanceID);
				return roleInfo;
			}
		}
		public HeroSortType currentSortType = HeroSortType.QualityAsc;
		public List<HeroInfo> currentHeroInfoList;

		void Awake(){
			instance = this;
		}

		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver("UpdateStrengthenSuccess", UpdateStrengthenSuccess);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver("UpdateStrengthenSuccess", UpdateStrengthenSuccess);
			}
		}

		public void SetSelectedMaterialHeroInfo(int index,HeroInfo info){
			if(index<0 || index>=_selectedMaterialHeroInfos.Length)
				return;
			_selectedMaterialHeroInfos[index] = info;
		}

		public void SetSelectedMaterialHeroInfo(uint oldInstanceId,HeroInfo newInfo){
			int count = _selectedMaterialHeroInfos.Length;
			for (int i = 0; i < count; i++)
			{
				if (_selectedMaterialHeroInfos[i]!= null &&_selectedMaterialHeroInfos[i].instanceID == oldInstanceId)
				{
					_selectedMaterialHeroInfos[i] = newInfo;
					break;
				}
			}
		}

		public void SetSelectedMaterialHeroInfo(HeroInfo info){
			int count = _selectedMaterialHeroInfos.Length;
			for (int i = 0; i < count; i++)
			{
				if (_selectedMaterialHeroInfos[i] == null)
				{
					_selectedMaterialHeroInfos[i] = info;
					break;
				}
			}
		}
		public int GetSelectedMaterialCount()
		{
			int num = 0;
			for(int i = 0,count = _selectedMaterialHeroInfos.Length;i<count;i++)
			{
				if(_selectedMaterialHeroInfos[i] != null)
					num++;
			}
			return num;
		}
		public List<HeroInfo> GetSelectedMaterialHeroInfoList(){
			return _selectedMaterialHeroInfos.ToList();
		}

		public void ClearMaterials(){
			int count = _selectedMaterialHeroInfos.Length;
			for (int i = 0; i < count; i++)
			{
				_selectedMaterialHeroInfos[i] = null;
				
			}
		}

		public List<HeroInfo> GetHeroInfoBySortType(HeroSortType type){
			List<HeroInfo> allInBagHeroInfoList = HeroProxy.instance.GetNotInAnyTeamHeroInfoList();
			for (int index = 0, count = allInBagHeroInfoList.Count; index < count; index++)
			{
				if (allInBagHeroInfoList[index].instanceID == StrengthenHeroInfo.instanceID)
				{
					allInBagHeroInfoList.RemoveAt(index);
					break;
				}
			}
			if(type == HeroSortType.QualityAsc)
				allInBagHeroInfoList.Sort(HeroUtil.CompareHeroByQualityAsc);
			else
				allInBagHeroInfoList.Sort(HeroUtil.CompareHeroByQualityDesc);
			currentHeroInfoList = allInBagHeroInfoList;
			return currentHeroInfoList;
		}

		public List<HeroInfo> GetHeroInfoBySortType(){
			return GetHeroInfoBySortType(currentSortType);
		}

//		public void UpdateStrengthenSuccess(bool isCrit){
//			if(onHeroStrengthenSuccessDelegate!= null){
//				onHeroStrengthenSuccessDelegate(isCrit);
//			}
//		}

		public bool UpdateStrengthenSuccess(Observers.Interfaces.INotification note)
		{
			bool isCrit = note.Body.ToString().ToBoolean();
			if(onHeroStrengthenSuccessDelegate!= null)
				onHeroStrengthenSuccessDelegate(isCrit);
			return true;
		}
	}
}

