using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Hero.Model;
using Logic.Fight.Model;
using Logic.Protocol.Model;
using Logic.Enums;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.Dungeon.Model;
using Logic.FunctionOpen.Model;
using Logic.Fight.Controller;
using Logic.UI.WorldTree.Model;
using Logic.Activity.Model;

namespace Logic.UI.FightResult.Model
{
    public class FightResultProxy : SingletonMono<FightResultProxy>
    {
        private List<DropItem> _dropHeroAndEquipList = new List<DropItem>();
        public List<DropItem> DropHeroAndEquipList
        {
            get
            {
                return _dropHeroAndEquipList;
            }
        }
        private Dictionary<int, HeroInfo> _fightBeforeTeamHeroDictionary = new Dictionary<int, HeroInfo>();
        public Dictionary<int, HeroInfo> FightBeforeTeamHeroDictionary
        {
            get
            {
                return _fightBeforeTeamHeroDictionary;
            }
        }
        private PlayerInfo _fightBeforeTeamPlayer;
        public PlayerInfo FightBeforeTeamPlayer
        {
            get
            {
                return _fightBeforeTeamPlayer;
            }
        }
        public Dictionary<BaseResType, int> dropBaseResDictionary = new Dictionary<BaseResType, int>();

        void Awake()
        {
            instance = this;
        }
        public void Clear()
        {
            _fightBeforeTeamHeroDictionary.Clear();
            _fightBeforeTeamPlayer = null;
			_dropHeroAndEquipList.Clear();
        }
        //保存战斗前阵型英雄信息(主要是英雄经验 结算用)
        public void SaveLastTeamData()
        {
            List<FightHeroInfo> pveHeroProtoDataList = FightProxy.instance.fightHeroInfoList;
            _fightBeforeTeamHeroDictionary.Clear();
            HeroInfo heroInfo;
            for (int i = 0, count = pveHeroProtoDataList.Count; i < count; i++)
            {
                heroInfo = pveHeroProtoDataList[i].heroInfo;
                HeroInfo info = new HeroInfo(heroInfo.instanceID, heroInfo.heroData.id, heroInfo.breakthroughLevel, heroInfo.strengthenLevel, heroInfo.advanceLevel, heroInfo.level);
                info.exp = heroInfo.exp;
                info.strengthenExp = heroInfo.strengthenExp;
                _fightBeforeTeamHeroDictionary.Add((int)info.instanceID, info);
            }

			_fightBeforeTeamPlayer = null;
			if (FightProxy.instance.fightPlayerInfo != null)
			{
				PlayerInfo player = FightProxy.instance.fightPlayerInfo.playerInfo;
				_fightBeforeTeamPlayer = new PlayerInfo(player.instanceID, player.playerData.Id, player.hairCutIndex, player.hairColorIndex, player.faceIndex,player.skinIndex, GameProxy.instance.AccountName);
				_fightBeforeTeamPlayer.exp = player.exp;
				_fightBeforeTeamPlayer.level = player.level;
				_fightBeforeTeamPlayer.weaponID = player.weaponID;
				_fightBeforeTeamPlayer.accessoryID = player.accessoryID;
				_fightBeforeTeamPlayer.armorID = player.armorID;
			}
            //            _fightBeforeTeamPlayer = new PlayerInfo(player.instanceID, player.playerData.Id, player.hairCutIndex, player.hairColorIndex, player.faceIndex, player.name);
            

        }

        public HeroInfo GetTeamHeroInfo(int instanceID)
        {
            if (FightBeforeTeamHeroDictionary.ContainsKey(instanceID))
            {
                return FightBeforeTeamHeroDictionary[instanceID];
            }
            return null;
        }
        public void CalcDropItem(FightType type = FightType.PVE)
        {
            if (type == FightType.DailyPVE)
            {
                _dropHeroAndEquipList = ActivityProxy.instance.fixedRewardGameResDataList;
                return;
            }
            if (FightProxy.instance.CurrentDungeonData != null && FightProxy.instance.CurrentDungeonData.dungeonType == DungeonType.WorldTree)
            {
                _dropHeroAndEquipList = FightProxy.instance.dropItems;
            }
            else
            {
                _dropHeroAndEquipList.Clear();
                dropBaseResDictionary.Clear();
                List<DropItem> dropItemList = FightProxy.instance.dropItems;
                if (dropItemList == null)
                    return;
                DropItem item;
                int count = dropItemList.Count;

                for (int i = 0; i < count; i++)
                {
                    item = dropItemList[i];
                    if (item.itemType == (int)BaseResType.Hero
                             || item.itemType == (int)BaseResType.Equipment
                             || item.itemType == (int)BaseResType.Item)
                    {
                        _dropHeroAndEquipList.Add(item);
                    }
                    else
                    {
                        dropBaseResDictionary[(BaseResType)item.itemType] = dropBaseResDictionary.GetValue((BaseResType)item.itemType) + item.itemNum;
                    }
                }
            }

        }
        public bool IsHeroDie(uint instanceId)
        {
            Dictionary<uint, Logic.Character.HeroEntity> deadHero = Logic.Character.Controller.PlayerController.instance.deadHeroDic;
            if (deadHero.ContainsKey(instanceId))
            {
                return true;
            }
            return false;
        }

        private DungeonInfo GetNextDungeon()
        {
            DungeonData currentDungeonData = Logic.Fight.Model.FightProxy.instance.CurrentDungeonData;
            DungeonData nextDungeonData = DungeonData.GetDungeonDataByID(currentDungeonData.unlockDungeonIDNext1);


            DungeonInfo nextDungeonInfo = Dungeon.Model.DungeonProxy.instance.GetDungeonInfo(nextDungeonData.dungeonID);

            return nextDungeonInfo;
        }
        private FightResultQuitType quitResultType;
        public void GotoMainScene(FightResultQuitType type, System.Action loadFinishedCallback)
        {
            quitResultType = type;
            UIMgr.instance.CloseLayerBelow(Logic.UI.EUISortingLayer.FlyWord);
            FightController.instance.QuitFight(true, () =>
            {
                Logic.Game.Controller.GameController.instance.LoadUIResources(loadFinishedCallback);
            });
        }

        public void QuitPveCallback()
        {
			DungeonData currentDungeonData = FightProxy.instance.CurrentDungeonData;
            DungeonType lastSelectedDungeonType = Logic.Chapter.Model.ChapterProxy.instance.LastSelectedDungeonType;
            if (lastSelectedDungeonType == DungeonType.Invalid)
                lastSelectedDungeonType = DungeonType.Easy;
            int lastSelectedDungeonDataID = Logic.Dungeon.Model.DungeonProxy.instance.GetLastUnlockDungeonID(lastSelectedDungeonType);
            DungeonData lastSelectDungeonData = DungeonData.GetDungeonDataByID(lastSelectedDungeonDataID);
            switch (quitResultType)
            {
                case FightResultQuitType.Fight_Again:
                    Logic.UI.DungeonDetail.Model.DungeonDetailProxy.instance.StartFight();
                    break;
                case FightResultQuitType.Fight_Again_Map:
					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_Detail_View, currentDungeonData.dungeonID, true);
                    break;
                case FightResultQuitType.Fight_Next_Dungeon:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_Detail_View, GetNextDungeon().dungeonData.dungeonID, true);
                    break;
                case FightResultQuitType.Go_Equip:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, lastSelectDungeonData.dungeonID, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Equipment_View);
                    break;
                case FightResultQuitType.Go_Hero:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, lastSelectDungeonData.dungeonID, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.RoleInfoView);
                    break;
                case FightResultQuitType.Go_Map:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, currentDungeonData.dungeonID, true);
                    break;
                case FightResultQuitType.Go_Player:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, lastSelectDungeonData.dungeonID, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
                    break;
                case FightResultQuitType.Go_Formation:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, lastSelectDungeonData.dungeonID, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
                    break;
                case FightResultQuitType.Go_Shop:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, lastSelectDungeonData.dungeonID, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View);
                    break;
                case FightResultQuitType.Go_MainView:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView);
                    break;
				case FightResultQuitType.GO_Boss_List:
					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, lastSelectDungeonData.dungeonID, true);
					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Boss_List_View);
					break;
            }
        }

        public void QuitWorldTreeCallback()
        {
            switch (quitResultType)
            {
                case FightResultQuitType.Go_WorldTree:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_WorldTree, null, true);
                    break;
                case FightResultQuitType.Fight_Next_Dungeon:
                    WorldTreeDungeonInfo nextWorldTreeDungeonInfo = WorldTreeProxy.instance.GetNextWorldTreeDungeonInfo(Logic.Fight.Model.FightProxy.instance.CurrentDungeonData.dungeonID);
                    if (nextWorldTreeDungeonInfo.dungeonID != null)
                    {
                        Logic.UI.WorldTree.Controller.WorldTreeController.instance.CLIENT2LOBBY_WORLD_TREE_CHALLENGE_REQ(nextWorldTreeDungeonInfo.dungeonID);
                    }
                    break;
                case FightResultQuitType.Go_Equip:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_WorldTree, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Equipment_View);
                    break;
                case FightResultQuitType.Go_Hero:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_WorldTree, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.RoleInfoView);
                    break;
                case FightResultQuitType.Go_Player:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_WorldTree, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
                    break;
                case FightResultQuitType.Go_Formation:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_WorldTree, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
                    break;
                case FightResultQuitType.Go_Shop:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_WorldTree, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View);
                    break;
            }
        }
        public void QuitActivityCallback()
        {
            switch (quitResultType)
            {
                case FightResultQuitType.Go_Activity:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_DailyDungeon, null, true);
                    break;
                case FightResultQuitType.Fight_Activity_again:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_DailyDungeon, null, true);
                    break;
                case FightResultQuitType.Go_Equip:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_DailyDungeon, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Equipment_View);
                    break;
                case FightResultQuitType.Go_Hero:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_DailyDungeon, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.RoleInfoView);
                    break;
                case FightResultQuitType.Go_Formation:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_DailyDungeon, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
                    break;
                case FightResultQuitType.Go_Player:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_DailyDungeon, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
                    break;
                case FightResultQuitType.Go_Shop:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_DailyDungeon, null, true);
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View);
                    break;
            }
        }
        public void QuitExpeditionCallback()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_Expedition, null, true);
            switch (quitResultType)
            {
                case FightResultQuitType.Go_Equip:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Equipment_View);
                    break;
                case FightResultQuitType.Go_Hero:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.RoleInfoView);
                    break;
                case FightResultQuitType.Go_Player:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
                    break;
                case FightResultQuitType.Go_Shop:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View);
                    break;
				case FightResultQuitType.Go_Formation:
					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
					break;
                case FightResultQuitType.Go_Expedition:
                    break;
            }
        }
        public void QuitPvpCallback()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_Arena, null, true);
            switch (quitResultType)
            {
                case FightResultQuitType.Go_Equip:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Equipment_View);
                    break;
                case FightResultQuitType.Go_Hero:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.RoleInfoView);
                    break;
                case FightResultQuitType.Go_Player:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
                    break;
                case FightResultQuitType.Go_Shop:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View);
                    break;
				case FightResultQuitType.Go_Formation:
					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
					break;
                case FightResultQuitType.Go_Pvp:
                    //FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.pvp);
                    break;
            }
        }

        public void QuitPvpRaceCallback()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MultpleFight_View, null, true);
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_PvpRace);
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_PvpRaceMatch);
            switch (quitResultType)
            {
                case FightResultQuitType.Go_Equip:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Equipment_View);
                    break;
                case FightResultQuitType.Go_Hero:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.RoleInfoView);
                    break;
                case FightResultQuitType.Go_Player:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
                    break;
                case FightResultQuitType.Go_Shop:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View);
                    break;
				case FightResultQuitType.Go_Formation:
					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
					break;
                case FightResultQuitType.GO_PVP_RACE:
                    break;
            }
        }

        public void QuitFriendCallback()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Friend, null, true);
            switch (quitResultType)
            {
                case FightResultQuitType.Go_Equip:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Equipment_View);
                    break;
                case FightResultQuitType.Go_Hero:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.RoleInfoView);
                    break;
                case FightResultQuitType.Go_Player:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
                    break;
                case FightResultQuitType.Go_Shop:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View);
                    break;
				case FightResultQuitType.Go_Formation:
					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
					break;
                case FightResultQuitType.GO_FRIEND:
                    //FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.pvp);
                    break;
            }
        }

        public void QuitMineCallback()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_FightCenter, null, true);
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.FightCenter_MineBattle);
            switch (quitResultType)
            {
                case FightResultQuitType.Go_Equip:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Equipment_View);
                    break;
                case FightResultQuitType.Go_Hero:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.RoleInfoView);
                    break;
                case FightResultQuitType.Go_Player:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PlayerInfoView);
                    break;
                case FightResultQuitType.Go_Shop:
                    FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Shop_View);
                    break;
				case FightResultQuitType.Go_Formation:
					FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.PveEmbattleView);
					break;
            }
        }

    }
}

