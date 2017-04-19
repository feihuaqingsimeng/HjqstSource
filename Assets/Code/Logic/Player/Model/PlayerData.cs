using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Common.ResMgr;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Hero.Model;

namespace Logic.Player.Model
{
	public class PlayerData
	{
		private static Dictionary<uint, PlayerData> _playerDataDictionary;

        public static Dictionary<uint, PlayerData> GetPlayerDatas()
        {
            if (_playerDataDictionary == null)
            {
                _playerDataDictionary = CSVUtil.Parse<uint, PlayerData>("config/csv/player", "id");
            }
            return _playerDataDictionary;
        }

		public static Dictionary<uint, PlayerData> PlayerDataDictionary
		{
			get
			{
				if (_playerDataDictionary == null)
                {
                    GetPlayerDatas();
				}
				return _playerDataDictionary;
			}
		}

		public static List<PlayerData> GetAllPlayerDataList ()
		{
			return new List<PlayerData>(PlayerDataDictionary.Values);
		}

		public static List<PlayerData> GetBasicPlayerDataList ()
		{
			List<PlayerData> initialPlayerDataList = new List<PlayerData>();
			List<int> initialPlayerDataIDList = GlobalData.GetGlobalData().initialPlayerDataIDList;
			int initialPlayerDataIDCount = initialPlayerDataIDList.Count;
			for (int i = 0; i < initialPlayerDataIDCount; i++)
			{
				PlayerData playerData = PlayerDataDictionary[(uint)initialPlayerDataIDList[i]];
				initialPlayerDataList.Add(playerData);
			}
			return initialPlayerDataList;
		}

		public static PlayerData GetPlayerData (uint playerID)
		{
			PlayerData playerData = null;
			if (PlayerDataDictionary.ContainsKey(playerID))
			{
				playerData = PlayerDataDictionary[playerID];
			}
			return playerData;
		}

		public static List<PlayerData> GetChangeProfessionPlayerDataList (PlayerData playerData)
		{
			List<PlayerData> changeProfessionPlayerDataList = new List<PlayerData>();
			uint[] hoppingIDs = playerData.hoppingIds;
			int hoppingIDCount = hoppingIDs.Length;
			for (int i = 0; i < hoppingIDCount; i++)
			{
				changeProfessionPlayerDataList.Add(GetPlayerData(hoppingIDs[i]));
			}
			return changeProfessionPlayerDataList;
		}

		public string PlayerModelPath
		{
			get
			{
				return ResPath.GetPlayerModelPath(this.model);
			}
		}

		[CSVElement("id")]
		public uint Id;


	//	public string name;


	//	public string description;


		public string model
		{
			get
			{
				HeroData herodata = heroData;
				return herodata.modelNames[herodata.starMin-1];
			}
		}
		public HeroData heroData
		{
			get
			{
				return HeroData.GetHeroDataByID(heroId);
			}
		}
		[CSVElement("hero_id")]
		public int heroId;

        [CSVElement("portrait")]
        public string portrait;

        [CSVElement("figureImage")]
		public string figureImage;

		[CSVElement("avatar")]
        public uint avatarID;
	
		[CSVElement("offence")]
		public float offence;

		[CSVElement("defence")]
		public float defence;
		
		public List<int> transferTaskConditionIDList;
		[CSVElement("task_condition")]
		public string taskCondition
		{
			set
			{
				transferTaskConditionIDList = value.ToList<int>(CSVUtil.SYMBOL_SEMICOLON);
			}
		}

		public List<GameResData> transferCostList;
		[CSVElement("transfer_item")]
		public string transferItem
		{
			set
			{
				transferCostList = GameResData.ParseGameResDataList(value);
			}
		}

		public uint[] hoppingIds;
		[CSVElement("hopping")]
		public string hopping
		{
			set
			{
				hoppingIds = value.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);
			}
		}

		[CSVElement("summonId")]
		public uint summonID;

        [CSVElement("summonSkillId")]
        public uint summonSkillId;

        [CSVElement("summonMax")]
        public uint maxAngry;

		public GameResData breakThroughItem;
		[CSVElement("pet_breakthrough_item")]
		public string pet_breakthrough_itemString
		{
			set
			{
				breakThroughItem = new GameResData(value);
			}
		}
		[CSVElement("pet_id")]
		public int pet_id;

	}
}