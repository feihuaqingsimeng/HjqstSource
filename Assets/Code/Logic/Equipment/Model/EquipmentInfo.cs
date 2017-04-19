using UnityEngine;
using Logic.Protocol.Model;
using System.Collections.Generic;
using Logic.Enums;
using LuaInterface;
using System.Collections;

namespace Logic.Equipment.Model
{
	public class EquipmentInfo
	{
		public int instanceID;
		public EquipmentData equipmentData;
		public int strengthenLevel;
		public int strengthenExp;
		public int ownerId = 0;
		public int star = 0;

		private EquipmentAttribute  _baseAttr;
		private List<EquipmentAttribute> _randomAttrs = new List<EquipmentAttribute>();

		private List<EquipmentAttribute> _equipAttribute = new List<EquipmentAttribute>();
		//当前属性
		public List<EquipmentAttribute> EquipAttribute
		{
			get
			{
				return _equipAttribute;
			}
			set
			{
				_equipAttribute = value;
			}
		}


		public EquipmentAttribute MainAttribute
		{
			get
			{
				return equipmentData.GetBaseAttr();
			}
		}

		public EquipmentAttribute FirstDeputyAttribute
		{
			get
			{
				if (_randomAttrs.Count >= 1)
				{
					return _randomAttrs[0];
				}
				return null;
			}
		}

		public EquipmentAttribute SecondDeputyAttribute
		{
			get
			{
				if (_randomAttrs.Count >= 2)
				{
					return _randomAttrs[1];
				}
				return null;
			}
		}

		public EquipmentInfo (LuaTable equipmentInfoLuaTable)
		{
			this.instanceID = equipmentInfoLuaTable["id"].ToString().ToInt32();
			LuaTable equipDataLuaTable = (LuaTable)equipmentInfoLuaTable["data"];
			this.equipmentData = EquipmentData.GetEquipmentDataByID(equipDataLuaTable["id"].ToString().ToInt32());
			LogNullEquipData(equipDataLuaTable["id"].ToString().ToInt32());

			LuaTable attr1Table = (LuaTable)equipmentInfoLuaTable.GetLuaFunction("GetTotalBaseAttr").Call(equipmentInfoLuaTable)[0];
			EquipmentAttributeType type = (EquipmentAttributeType)attr1Table["type"].ToString().ToInt32();
			_baseAttr = new EquipmentAttribute(type, attr1Table["value"].ToString().ToFloat());

			if (equipmentInfoLuaTable["randomAttrs"] != null)
			{
				_randomAttrs.Clear();
				LuaTable attr2Table = (LuaTable)equipmentInfoLuaTable["randomAttrs"];

				foreach(DictionaryEntry kvp in attr2Table.ToDictTable())
				{
					LuaTable table = kvp.Value as LuaTable;
					if(table != null)
					{
						type = (EquipmentAttributeType)table["type"].ToString().ToInt32();
						_randomAttrs.Add(new EquipmentAttribute(type, table["value"].ToString().ToFloat()));
					}
				}
			}
			_equipAttribute.Clear();
			_equipAttribute.Add(_baseAttr);
			for(int i = 0;i<_randomAttrs.Count;i++)
			{
				_equipAttribute.Add(_randomAttrs[i]);
				
			}
			this.ownerId = equipmentInfoLuaTable["ownerId"].ToString().ToInt32();
			this.star = equipmentInfoLuaTable["star"].ToString().ToInt32();
		}

        public EquipmentInfo(int instanceID, EquipmentData equipmentData, int strengthenLevel)
		{
			this.instanceID = instanceID;
			this.equipmentData = equipmentData;
			this.strengthenLevel = strengthenLevel;
		}

		public EquipmentInfo (int instanceID, int equipmentDataID, int strengthenLevel)
		{
			this.instanceID = instanceID;
			this.equipmentData = EquipmentData.GetEquipmentDataByID(equipmentDataID);
			LogNullEquipData(equipmentDataID);
			this.strengthenLevel = strengthenLevel;
		}

		public EquipmentInfo (DrawCardDropProto drawCardDropProto)
		{
			this.instanceID = -1;
			this.equipmentData = EquipmentData.GetEquipmentDataByID(drawCardDropProto.no);
			LogNullEquipData(drawCardDropProto.no);
			this.strengthenLevel = 0;
		}

		public EquipmentInfo (EquipmentInfo info){
			this.instanceID = info.instanceID;
			this.equipmentData = info.equipmentData;
			this.strengthenLevel = info.strengthenLevel;
			this.EquipAttribute = new List<EquipmentAttribute>(info.EquipAttribute);
		}

		private void LogNullEquipData(int equipid)
		{
			if(equipmentData == null)
				Debugger.LogError("equipmentData is null, id :"+equipid);
		}

		public int Power
		{
			get{
				return (int) EquipmentUtil.CalcEquipPower(this);
			}
		}
	}
}