using UnityEngine;
using System.Collections;
using Logic.Equipment.Model;
using System.Collections.Generic;
using Logic.Enums;

namespace Logic.UI.EquipmentsStrengthen.Model
{
	public class EquipmentStrengthenProxy : SingletonMono<EquipmentStrengthenProxy> 
	{

		public EquipmentInfo StrengthenEquipInfo;

		public EquipmentInfo[] materialsEquipInfo = new EquipmentInfo[4];

		public List<EquipmentInfo> currentEqupmentInfoList ;


		public delegate void EquipmentStrengthenSuccessDelegate();
		
		public EquipmentStrengthenSuccessDelegate onEquipmentStrengthenSuccessDelegate;
		
		void Awake()
		{
			instance = this;
		}

		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver("OnEquipmentStrengthenSuccess", StrengthenSuccess);
		}

		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver("OnEquipmentStrengthenSuccess", StrengthenSuccess);
			}
		}

		public void ClearMaterials()
		{
			for(int i = 0,count = materialsEquipInfo.Length;i<count;i++)
			{
				materialsEquipInfo[i] = null;
			}
		}

		public EquipmentInfo GetMaterialEquipInfo(int index)
		{
			if(index<0 || index>=materialsEquipInfo.Length)
				return null;
			return materialsEquipInfo[index];
		} 

		public List<EquipmentInfo> GetMaterialEquipInfoList()
		{
			return new List<EquipmentInfo>(materialsEquipInfo);
		}

		public bool AddMaterialEquipInfo(EquipmentInfo info)
		{
			for(int i = 0,count = materialsEquipInfo.Length;i<count;i++)
			{
				if(materialsEquipInfo[i] == null)
				{
					materialsEquipInfo[i] = info;
					return true;
				}
			}
			return false;
		}

		public bool RemoveMaterialEquipInfo(int instanceId)
		{
			for(int i = 0,count = materialsEquipInfo.Length;i<count;i++)
			{
				if(materialsEquipInfo[i]!= null && materialsEquipInfo[i].instanceID == instanceId)
				{
					materialsEquipInfo[i] = null;
					return true;
				}
				
			}
			return false;
		}

		public List<EquipmentInfo> GetEquipmentsByType(EquipmentStrengthenSortType type)
		{
			List<EquipmentInfo> equipList = EquipmentProxy.instance.GetFreeEquipmentInfoList();
			equipList.Remove(StrengthenEquipInfo);
			List<EquipmentInfo> typeList = new List<EquipmentInfo>();
			EquipmentInfo info ;
			int count = equipList.Count;
			switch(type){
			case EquipmentStrengthenSortType.All:
				typeList = equipList;
				break;
			case EquipmentStrengthenSortType.Weapon:
				
				for(int i = 0;i<count;i++)
				{
					info = equipList[i];
					if(info.equipmentData.equipmentType == EquipmentType.MagicalWeapon|| info.equipmentData.equipmentType == EquipmentType.PhysicalWeapon)
					{
						typeList.Add(info);
						
					}
				}
				
				break;
			case EquipmentStrengthenSortType.Armor:
				
				for(int i = 0;i<count;i++)
				{
					info = equipList[i];
					if(info.equipmentData.equipmentType == EquipmentType.Armor)
					{
						typeList.Add(info);
						
					}
				}
				
                    
                    break;
                case EquipmentStrengthenSortType.Accessory:
                    
                    for(int i = 0;i<count;i++)
                    {
                        info = equipList[i];
                        if(info.equipmentData.equipmentType == EquipmentType.Accessory)
                        {
                            typeList.Add(info);
                        }
                    }
                    
                    
                    break;
                    
            }
			currentEqupmentInfoList = typeList;
			return currentEqupmentInfoList;
        }
        
        public bool StrengthenSuccess(Observers.Interfaces.INotification note)
        {
            if(onEquipmentStrengthenSuccessDelegate!= null)
                onEquipmentStrengthenSuccessDelegate();
			return true;
        }
    }
}

