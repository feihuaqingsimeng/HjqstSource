using UnityEngine;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.Equipment.Model;

namespace Logic.Player.Model
{
	public class PlayerProxy : SingletonMono<PlayerProxy>
	{
		private string _playerName;
		public string PlayerName
		{
			get
			{
				return _playerName;
			}
		}

		private Dictionary<int, PlayerInfo> _playerInfoDictionary;
		public Dictionary<int, PlayerInfo> PlayerInfoDictionary
		{
			get
			{
				if (_playerInfoDictionary == null)
				{
					_playerInfoDictionary = new Dictionary<int, PlayerInfo>();
				}
				return _playerInfoDictionary;
			}
		}

		public delegate void OnPlayerInfoUpdateDelegate ();
		public OnPlayerInfoUpdateDelegate onPlayerInfoUpdateDelegate;

		void Awake ()
		{
			instance = this;
		}

		public void SetPlayerName (string playerName)
		{
			_playerName = playerName;
		}

		public bool IsPlayerUnlocked (int playerDataID)
		{
			List<PlayerInfo> playerInfoList = PlayerInfoDictionary.GetValues();
			int playerInfoCount = playerInfoList.Count;
			for (int i = 0; i < playerInfoCount; i++)
			{
				if (playerInfoList[i].playerData.Id == playerDataID)
				{
					return true;
				}
			}
			return false;
		}

		public PlayerInfo GetPlayerDataCorrespondingPlayerInfo (int playerDataID)
		{
			List<PlayerInfo> playerInfoList = PlayerInfoDictionary.GetValues();
			int playerInfoCount = playerInfoList.Count;
			for (int i = 0; i < playerInfoCount; i++)
			{
				if (playerInfoList[i].playerData.Id == playerDataID)
				{
					return playerInfoList[i];
				}
			}
			return null;
		}

		public bool AddPlayer (PlayerInfo playerInfo)
		{
			if (PlayerInfoDictionary.ContainsKey((int)playerInfo.instanceID))
			{
				Debugger.Log("The instanceID of new player is already exists in the player info dictionary.");
				return false;
			}
			PlayerInfoDictionary.Add((int)playerInfo.instanceID, playerInfo);
			return true;
		}

		public PlayerInfo GetPlayerInfo (int playerInstanceID)
		{
			PlayerInfo playerInfo = null;
			PlayerInfoDictionary.TryGetValue(playerInstanceID, out playerInfo);
			return playerInfo;
		}
		public PlayerInfo GetPlayerInfoByModelId(int modelId)
		{
			foreach(var data in PlayerInfoDictionary)
			{
				if(data.Value.modelDataId == modelId)
					return data.Value;
			}
			return null;
		}
		public void OnPlayerInfoUpdate ()
		{
			if (onPlayerInfoUpdateDelegate != null)
			{
				onPlayerInfoUpdateDelegate();
			}
		}

		public void PutOnEquipment (int playerInstanceID, int equipmentInstanceID)
		{
			PlayerInfo playerInfo = GetPlayerInfo(playerInstanceID);
			EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(equipmentInstanceID);
			if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.PhysicalWeapon
			    || equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.MagicalWeapon)
			{
				EquipmentInfo oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(playerInfo.weaponID);
				if (oldEquipmentInfo != null)
				{
					oldEquipmentInfo.ownerId = 0;
				}
				playerInfo.weaponID = equipmentInstanceID;
				equipmentInfo.ownerId = playerInstanceID;
			}
			else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Armor)
			{
				EquipmentInfo oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(playerInfo.armorID);
				if (oldEquipmentInfo != null)
				{
					oldEquipmentInfo.ownerId = 0;
				}
				playerInfo.armorID = equipmentInstanceID;
				equipmentInfo.ownerId = playerInstanceID;
			}
			else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Accessory)
			{
				EquipmentInfo oldEquipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(playerInfo.accessoryID);
				if (oldEquipmentInfo != null)
				{
					oldEquipmentInfo.ownerId = 0;
				}
				playerInfo.accessoryID = equipmentInstanceID;
				equipmentInfo.ownerId = playerInstanceID;
			}
		}
		
		public void PutOffEquipment (int playerInstanceID, int equipmentInstanceID)
		{
			PlayerInfo playerInfo = GetPlayerInfo(playerInstanceID);
			EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(equipmentInstanceID);
			if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.PhysicalWeapon
			    || equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.MagicalWeapon)
			{
				playerInfo.weaponID = 0;
				equipmentInfo.ownerId = 0;
			}
			else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Armor)
			{
				playerInfo.armorID = 0;
				equipmentInfo.ownerId = 0;
			}
			else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Accessory)
			{
				playerInfo.accessoryID = 0;
				equipmentInfo.ownerId = 0;
			}
		}
		
		public void UpdatePlayerEquipments (int playerInstanceID, List<int> equipmentIDs)
		{
			PlayerInfo playerInfo = GetPlayerInfo(playerInstanceID);
			
			int weaponID = 0;
			int armorID = 0;
			int accessoryID = 0;
			
			int equipentIDsCount = equipmentIDs.Count;
			if (equipentIDsCount == 0) //装备列表为空，表示无需更新装备
			{
				return;
			}

			if (equipmentIDs.Contains(-1))
			{
				PutOffEquipment(playerInstanceID, playerInfo.weaponID);
				PutOffEquipment(playerInstanceID, playerInfo.armorID);
				PutOffEquipment(playerInstanceID, playerInfo.accessoryID);
				return;
			}

			for (int i = 0; i < equipmentIDs.Count; i++)
			{
				EquipmentInfo equipmentInfo = EquipmentProxy.instance.GetEquipmentInfoByInstanceID(equipmentIDs[i]);
				if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.PhysicalWeapon
				    || equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.MagicalWeapon)
				{
					weaponID = equipmentIDs[i];
				}
				else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Armor)
				{
					armorID = equipmentIDs[i];
				}
				else if (equipmentInfo.equipmentData.equipmentType == Logic.Enums.EquipmentType.Accessory)
				{
					accessoryID = equipmentIDs[i];
				}
			}
			
			if (playerInfo.weaponID != weaponID)
			{
				if (weaponID > 0)
				{
					PutOnEquipment(playerInstanceID ,weaponID);
				}
				else
				{
					PutOffEquipment(playerInstanceID, playerInfo.weaponID);
				}
			}
			if (playerInfo.armorID != armorID)
			{
				if (armorID > 0)
				{
					PutOnEquipment(playerInstanceID, armorID);
				}
				else
				{
					PutOffEquipment(playerInstanceID, playerInfo.armorID);
				}
			}
			if (playerInfo.accessoryID != accessoryID)
			{
				if (accessoryID > 0)
				{
					PutOnEquipment(playerInstanceID, accessoryID);
				}
				else
				{
					PutOffEquipment(playerInstanceID, playerInfo.accessoryID);
				}
			}
		}
	}
}
