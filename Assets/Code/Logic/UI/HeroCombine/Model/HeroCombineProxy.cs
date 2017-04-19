using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Hero.Model;
using Logic.Enums;
using Logic.Hero.Model.Combine;
/// <summary>
/// 英雄合成.
/// </summary>
namespace Logic.UI.HeroCombine.Model{


	public class HeroCombineProxy : SingletonMono<HeroCombineProxy> {


		public delegate void UpdateCombineByProtocolHandler(bool isSuccess,int newHeroId,bool isSpecial);

		public UpdateCombineByProtocolHandler onUpdateCombineByProtocolDelegate;

		private SortedDictionary<CombinePosition,uint> _combineMaterialDictionary = new SortedDictionary<CombinePosition, uint>();

		public SortedDictionary<CombinePosition,uint> CombineMaterialDictionary{
			get{
				return _combineMaterialDictionary;
			}
		}

		public int NewHeroIntanceId;
		public bool IsCombineSpecial;
		public bool IsCombineSuccess;

		public void ClearCombineMaterialDic(){
			_combineMaterialDictionary.Clear();
		}
		void Awake(){
			instance = this;
		}

		public bool IsPositionEmpty(CombinePosition position){
			bool isEmpty = true;
			isEmpty = !_combineMaterialDictionary.ContainsKey(position);
			return isEmpty;
		}

		public uint GetHeroInstanceIDAt(CombinePosition position){
			if(_combineMaterialDictionary.ContainsKey(position))
				return _combineMaterialDictionary[position];
			return 0;
		}

		public bool IsHeroInCombineMaterial(uint heroInstanceID){
			return _combineMaterialDictionary.ContainsValue(heroInstanceID);
		}

		public HeroInfo GetHeroAt(CombinePosition pos){
			HeroInfo info = null;
			if(_combineMaterialDictionary.ContainsKey(pos)){
				uint instanceId = _combineMaterialDictionary[pos];
				info = GetHeroInfo(instanceId);
			}
			return info;
		}

		public CombinePosition GetHeroInCombinePosition(uint heroInstanceID){
			CombinePosition pos = CombinePosition.Invalide_Position;
			if(_combineMaterialDictionary.ContainsValue(heroInstanceID)){
				List<CombinePosition> keys = _combineMaterialDictionary.GetKeys();
				CombinePosition key;
				for(int i = 0,count = keys.Count;i<count;i++){
					key = keys[i];
					if(_combineMaterialDictionary[key] == heroInstanceID){
						pos = key;
						break;
					}
				}
			}
			return pos;
		}

		public bool AddHeroToCombineMaterial(CombinePosition pos,uint heroInstanceID){

			if(IsPositionEmpty(pos)){
				_combineMaterialDictionary.Add(pos,heroInstanceID);
				return true;
			}
			return false;
		}
		public bool AddHeroToCombineMaterial(uint heroInstanceID){
			if(IsHeroInCombineMaterial(heroInstanceID))
			   return false;
			int start = (int)CombinePosition.Combine_Position_1;
			int count = (int)CombinePosition.Combine_Max;
			CombinePosition pos ;
			for(int i = start;i<count;i++){
				pos = (CombinePosition)i;
				if(IsPositionEmpty(pos)){
					return AddHeroToCombineMaterial(pos,heroInstanceID);
				}
			}
			return false;
		}

		public bool RemoveHeroFromCombineMaterial(CombinePosition pos){
			bool result = _combineMaterialDictionary.Remove(pos);
			return result;
		}
		public bool RemoveHeroFromCombineMaterial(uint heroInstanceID){
			CombinePosition pos = GetHeroInCombinePosition(heroInstanceID);
			return RemoveHeroFromCombineMaterial(pos);
		}



		public List<HeroInfo> GetEnableCombineHeros(){
			List<HeroInfo> bagHeros = HeroProxy.instance.GetNotInAnyTeamHeroInfoList();

			List<HeroInfo> enableHeros = new List<HeroInfo>();
			HeroInfo info;
			for(int i = 0,count = bagHeros.Count;i<count;i++){
				info = bagHeros[i];
				if(info.advanceLevel<6)
					enableHeros.Add(info);
			}
			enableHeros.Sort(Logic.Hero.HeroUtil.CompareHeroByQualityAsc);
			return enableHeros;


		}
		public HeroInfo GetHeroInfo(uint instanceId){
			return HeroProxy.instance.GetHeroInfo(instanceId);

		}
		public List<HeroInfo> GetMaterialHeroInfoList(){
			List<uint> materialIds = new List<uint>(CombineMaterialDictionary.Values);
			List<HeroInfo> heroInfoList = new List<HeroInfo>();
			for(int i = 0,count = materialIds.Count;i<count;i++){

				heroInfoList.Add(GetHeroInfo(materialIds[i]));
			}
			return heroInfoList;
		}

		public void RefreshCombineByProtocol(bool isSuccess,int newHeroid,bool isSpecial){
			Debugger.Log(string.Format("issuccess:{0},new heroid:{1},isSpecial:{2}",isSuccess,newHeroid,isSpecial));
			NewHeroIntanceId = newHeroid;
			IsCombineSpecial = isSpecial;
			IsCombineSuccess = isSuccess;
			if(onUpdateCombineByProtocolDelegate != null)
				onUpdateCombineByProtocolDelegate(isSuccess,newHeroid,isSpecial);
		}
	}
}


