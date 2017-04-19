using System;
using UnityEngine;
using System.Collections;
using Logic.UI;
using Common.Localization;
using Logic.UI.Description.View;
using Logic.Enums;
using Logic.Game.Model;
using LitJson;
using Logic.UI.ServerList.Model;
using Logic.UI.Login.Model;
using Logic.UI.Pvp.Model;
using Logic.UI.Tips.View;
using Logic.UI.Pvp.Controller;
using Logic.FunctionOpen.Model;
using Logic.UI.IllustratedHandbook.Model;
using Logic.Role.Model;
using Logic.Hero.Model;
using Logic.Player.Model;
using System.Collections.Generic;
using Logic.UI.GoodsJump.Model;
using Logic.Role;
using Logic.Protocol.Model;
using Logic.Fight.Model;
using Logic.Fight.Controller;

namespace LuaInterface
{
    public class LuaCsTransfer
    {
        public static string GetIcon(int typeid, int uid)
        {
            var type = (Logic.Enums.BaseResType)typeid;
            switch (type)
            {
                case Logic.Enums.BaseResType.Hero:
                    return Common.ResMgr.ResPath.GetCharacterHeadIconPath(Logic.Hero.Model.HeroData.GetHeroDataByID(uid).headIcons[0]);
                case Logic.Enums.BaseResType.Equipment:
                    var dt = Logic.Equipment.Model.EquipmentData.GetEquipmentDataByID(uid);
                    if (dt != null)
                        return Common.ResMgr.ResPath.GetEquipmentIconPath(dt.icon);
                    else
                        return "equip icon null";
                case Logic.Enums.BaseResType.Item:
                    return Common.ResMgr.ResPath.GetItemIconPath(Logic.Item.Model.ItemData.GetItemDataByID(uid).icon);
                default:
                    return UIUtil.GetBaseResIconPath(type);
            }
        }

        public static void ShowTips(int typeid, int uid)
        {

        }

        public static void ShowMask(int id)
        {
            Logic.Protocol.ProtocolConf.instance.AddMaskId((int)Logic.Protocol.Model.MSG.ActivityRewardReq);
            Logic.Protocol.ProtocolConf.instance.AddMaskId((int)Logic.Protocol.Model.MSG.ActivityRewardResp);
            Logic.Protocol.ProtocolConf.instance.AddMaskId((int)Logic.Protocol.Model.MSG.ActivityJoinReq);
            Logic.Protocol.ProtocolConf.instance.AddMaskId((int)Logic.Protocol.Model.MSG.ActivityJoinResp);
        }

        public static void SystemNoticeCreate(Transform p)
        {
            Logic.UI.Chat.View.SystemNoticeView.Create(p);
        }

        public static void SystemNoticeAdd(string s)
        {
            Logic.UI.Chat.Model.SystemNoticeProxy.instance.AddSystemNotice(s);
        }

        public static void SystemNoticeAdd(int id, string para)
        {
            System.Collections.Generic.List<string> pm = null;
            string[] param = null;
            if (!string.IsNullOrEmpty(para))
                param = para.Split(';');

            if (param != null && param.Length > 0)
            {
                pm = new System.Collections.Generic.List<string>();
                for (int i = 0; i < param.Length; i++)
                {
                    pm.Add(param[i]);
                }
            }
            Logic.UI.Chat.Model.SystemNoticeProxy.instance.AddSystemNotice(id, pm);
        }

        public static long GetTime()
        {
            return Common.GameTime.Controller.TimeController.instance.ServerTimeTicksSecond;
        }

        public static string GetTime_AddSeconds(string time, int seconds)
        {
            return DateTime.Parse(string.Format("{0}:00:00", time)).AddSeconds(seconds).ToString("HH:mm:ss");
        }

        public static double GetTime_Stamp(string time)
        {
            return (DateTime.Parse(string.Format("{0}:00:00", time)) - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static string GetCountDown(int seconds)
        {
            return DateTime.Parse(DateTime.Now.ToString("00:00:00")).AddSeconds(seconds).ToString("HH:mm:ss");
        }

        public static void SetShopTag(int id)
        {
            var sv = UIMgr.instance.Get<Logic.UI.Shop.View.ShopView>("ui/shop/shop_view");
            if (sv != null) sv.SetTogglePanel(id);
            else Debugger.LogWarning("no shopview");
        }

        public static string GetVipInfo()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Logic.VIP.Model.VIPProxy.instance.VIPLevel);
            sb.Append(";");
            sb.Append(Logic.VIP.Model.VIPProxy.instance.dailyGetted ? 1 : 0);
            sb.Append(";");
            sb.Append(Logic.VIP.Model.VIPProxy.instance.TotalRecharge);
            sb.Append(";");
            if (Logic.VIP.Model.VIPProxy.instance.HasReceivedGiftVIPLevelList != null)
            {
                for (int i = 0, length = Logic.VIP.Model.VIPProxy.instance.HasReceivedGiftVIPLevelList.Count; i < length; i++)
                {
                    sb.Append(Logic.VIP.Model.VIPProxy.instance.HasReceivedGiftVIPLevelList[i]);
                    if (i != length - 1) sb.Append(":");
                }
            }
            return sb.ToString();
        }

        public static string GetDropDataDes(int type, int modelid)
        {
            var data = Logic.UI.GoodsJump.Model.DropMessageData.GetDropMsgDataByResData(type, modelid);
            return data == null ? "" : Localization.Get(data.des);
        }

        //UIMgr
        public static GameObject UIMgrOpen(string uiPath, int uiSortingLayer, int uiOpenMode)
        {
            return UIMgr.instance.Open(uiPath, (EUISortingLayer)uiSortingLayer, (UIOpenMode)uiOpenMode);
        }
        public static void UIMgrClose(string uiPath)
        {
            UIMgr.instance.Close(uiPath);
        }
        //localization
        public static string LocalizationGet(string key)
        {
            return Localization.Get(key);
        }
        //ResMgr
        public static GameObject ResMgrLoad(string path)
        {
            return Common.ResMgr.ResMgr.instance.Load<GameObject>(path);
        }
        public static Sprite ResMgrLoadSprite(string path)
        {
            return Common.ResMgr.ResMgr.instance.Load<Sprite>(path);
        }

        public static void OpenCommonAutoDestroyTipsView(string tipsString)
        {
            Logic.UI.Tips.View.CommonAutoDestroyTipsView.Open(tipsString);
        }

        public static GameObject OpenCommonExpandBagTipsView(int bagTypeValue, int cost, System.Action confirmAction, int ConsumeTipTypeValue)
        {
            return Logic.UI.Tips.View.CommonExpandBagTipsView.Open(bagTypeValue, cost, confirmAction, ConsumeTipTypeValue).gameObject;
        }

        public static GameObject OpenConfirmPutOffEquipmentTipsView(int equipID, int roleID, int costBaseResType, int costResID, int costResCount)
        {
            return Logic.UI.Tips.View.ConfirmPutOffEquipmentTipsView.Open(equipID, roleID, (Logic.Enums.BaseResType)costBaseResType, costResID, costResCount).gameObject;
        }

        public static GameObject OpenConfirmSubstituteEquipmentTipsView(int roleInstanceID, int equipmentID, int costBasResType, int costResId, int costResCount)
        {
            Logic.Game.Model.GameResData costGameResData = new Logic.Game.Model.GameResData((Logic.Enums.BaseResType)costBasResType, costResId, costResCount, 0);
            return Logic.UI.Tips.View.ConfirmSubstituteEquipmentTipsView.Open(roleInstanceID, equipmentID, costGameResData).gameObject;
        }

        public static bool GetConsumeTipEnable(int consumeTipTypeValue)
        {
            return Logic.ConsumeTip.Model.ConsumeTipProxy.instance.GetConsumeTipEnable((ConsumeTipType)consumeTipTypeValue);
        }

        public static GameObject OpenConfirmBuyShopItemTipsView(string itemName, int costResTypeValue, int costResID, int costResCount, System.Action onClickBuyButtonHandler, int consumeTipTypeValue)
        {
            GameResData costGameResData = new GameResData();
            costGameResData.type = (BaseResType)costResTypeValue;
            costGameResData.id = costResID;
            costGameResData.count = costResCount;
            ConsumeTipType consumeTipType = (ConsumeTipType)consumeTipTypeValue;
            return Logic.UI.Tips.View.ConfirmBuyShopItemTipsView.Open(itemName, costGameResData, onClickBuyButtonHandler, consumeTipType).gameObject;
        }

        public static int GetEquipCellNum()
        {
            return Logic.Game.Model.GameProxy.instance.EquipCellNum;
        }

        public static bool IsFunctionOpen(Logic.Enums.FunctionOpenType type, bool showTip)
        {
            return Logic.FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(type, showTip);
        }

        public static void OpenFunction(int functionOpenTypeValue, bool loadParent, bool showTip)
        {
            Logic.FunctionOpen.Model.FunctionOpenProxy.instance.OpenFunction((FunctionOpenType)functionOpenTypeValue, null, loadParent, showTip);
        }

        public static int GetLayerOfWorldTree(int dungeonId)
        {
            Logic.UI.WorldTree.Model.WorldTreeDungeonInfo worldTreeDungeonInfo = Logic.UI.WorldTree.Model.WorldTreeProxy.instance.GetWorldTreeInfoByID(dungeonId);
            if (worldTreeDungeonInfo != null)
                return worldTreeDungeonInfo.orderNumber;
            return 0;
        }
        public static void OpenGoodsJumpPath(int type, int id, int star)
        {
            Logic.UI.GoodsJump.View.GoodsJumpPathView.Open(type, id, star);
        }

        public static void GetItemDesButton(GameObject go, int itemDataID, bool isLongPress = true)
        {
            ItemDesButton itemDesButton = ItemDesButton.Get(go);
            ShowDescriptionType type = isLongPress ? ShowDescriptionType.longPress : ShowDescriptionType.click;
            itemDesButton.SetItemInfo(itemDataID, type);
        }
        public static void GetRoleDesButton(GameObject go, int id, int star, bool isShowBase, bool isLongPress = true)
        {
            RoleDesButton btn = RoleDesButton.Get(go);
            ShowDescriptionType type = isLongPress ? ShowDescriptionType.longPress : ShowDescriptionType.click;
            if (isShowBase)
            {
                btn.SetRoleInfo(new GameResData(BaseResType.Hero, id, 0, star), type);
            }
            else
            {
                if (GameProxy.instance.IsPlayer((uint)id))
                {
                    btn.SetRoleInfo(GameProxy.instance.PlayerInfo, type);

                }
                else
                {
                    btn.SetRoleInfo(Logic.Hero.Model.HeroProxy.instance.GetHeroInfo((uint)id), type);

                }
            }
        }
		public static void GetRoleDesButton(GameObject go, LuaTable roleInfo,bool isPlayer, bool isLongPress = true)
		{
			RoleDesButton btn = RoleDesButton.Get(go);
			ShowDescriptionType type = isLongPress ? ShowDescriptionType.longPress : ShowDescriptionType.click;

			if (isPlayer)
			{
				btn.SetRoleInfo(new PlayerInfo(roleInfo), type);
				
			}
			else
			{
				btn.SetRoleInfo(new HeroInfo(roleInfo), type);
				
			}

		}
        public static void SetRoleDesBtnType(GameObject go, int tp)
        {
            RoleDesButton btn = RoleDesButton.Get(go);
            btn.SetType((Logic.Enums.ShowDescriptionType)tp);
        }

        public static void SetRoleDesButton(GameObject go, bool enabled)
        {
            RoleDesButton.Get(go).enabled = enabled;
        }

        public static void GetEquipDesButton(GameObject go, int id, bool isShowBase)
        {
            EquipmentDesButton btn = EquipmentDesButton.Get(go);
            if (isShowBase)
            {
                btn.SetEquipInfo(id);
            }
            else
            {
                btn.SetEquipInfo(Logic.Equipment.Model.EquipmentProxy.instance.GetEquipmentInfoByInstanceID(id));
            }

        }
        public static bool IsDungeonPassed(int dungeonID)
        {
            Logic.Dungeon.Model.DungeonInfo dungeonInfo = Logic.Dungeon.Model.DungeonProxy.instance.GetDungeonInfo(dungeonID);
            return dungeonInfo == null || dungeonInfo.star > 0;
        }
        public static bool IsFriend(int id)
        {
            return Logic.UI.Friend.Model.FriendProxy.instance.isFriend(id);
        }
        public static void FriendAddReq(string name)
        {
            Logic.UI.Friend.Controller.FriendController.instance.CLIENT2LOBBY_FriendAddReq_REQ(name);
        }

        public static int GetTotalStarCountOfDungeonType(int dungeonType)
        {
            return Logic.Dungeon.Model.DungeonProxy.instance.GetTotalStarCountOfDungeonType((DungeonType)dungeonType);
        }

        public static void OpenActivateProfessionView(int playerDataID)
        {
            Logic.UI.ChangeProfession.View.ActivateProfessionView.Open(Logic.Player.Model.PlayerData.GetPlayerData((uint)playerDataID));
        }

        public static void OpenPVEEmbattleView(System.Action<int> callBack)
        {
            Logic.UI.PVEEmbattle.View.PVEEmbattleView.Open(callBack);
        }

        public static void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
        public static void ShowSdkPay(string pay)
        {
            PlatformProxy.instance.ShowSdkPay(pay);
        }

        public static void OpenTrainingFormationView(int formationTeamType)
        {
            //Logic.UI.TrainFormation.View.TrainFormationView.Open((FormationTeamType)formationTeamType);
        }
        //pvp竞技场比赛
        public static void SendArenaChanllengReq()
        {
            //			if (PvpProxy.instance.PvpInfo.remainChallengeTimes == 0)
            //			{
            //				CommonErrorTipsView.Open(Localization.Get("ui.pvp_formation_view.notEnoughChallengeTimes"));
            //				return;
            //			}
            PvpFighterInfo challengeFighter = PvpProxy.instance.ChallengeFighter;
            PvpController.instance.CLIENT2LOBBY_RANK_ARENA_CHANLLENGE_REQ(challengeFighter);
        }
        //账号信息
        public static void OpenAccountInfoView()
        {
            Logic.UI.AccountInfo.View.AccountInfoView.Open();
        }
        public static void OpenTaskView()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Task);
        }
        public static void OpenMailView()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Mail);
        }
        public static void OpenFriendView()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Friend);
        }
        public static void OpenSignInView()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_SignIn);
        }
        public static void OpenIllustrationView()
        {
            Logic.UI.IllustratedHandbook.View.IllustratedHandbookView.Open();
        }
        public static void OpenTaskDailyView()
        {
            if (FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_dailyTask, true))
            {
                Logic.UI.Task.View.TaskDailyView.Open();
            }
        }

        public static void OpenSelectChapter()
        {
            if (FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.Dungeon_SelectChapter_View))
            {
                //				DungeonType lastSelectedDungeonType = Logic.Chapter.Model.ChapterProxy.instance.LastSelectedDungeonType;
                //				if (lastSelectedDungeonType == DungeonType.Invalid)
                //					lastSelectedDungeonType = DungeonType.Easy;
                //				int lastSelectedDungeonDataID = Logic.Dungeon.Model.DungeonProxy.instance.GetLastUnlockDungeonID(lastSelectedDungeonType);
                //				FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View, lastSelectedDungeonDataID);

                FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.Dungeon_SelectChapter_View);
            }
        }
        public static void OpenDailyDungeonView()
        {
            if (!FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_DailyDungeon, true))
            {
                return;
            }
            UIMgr.instance.Open<Logic.UI.DailyDungeon.View.DailyDungeonView>(Logic.UI.DailyDungeon.View.DailyDungeonView.PREFAB_PATH);
        }
        public static void OpenFightCenterView()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_FightCenter);
        }
        public static void OpenEquipmentsBrowseView()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_Equipment);
        }
        public static void OpenWorldBossView()
        {
            FunctionOpenProxy.instance.OpenFunction(FunctionOpenType.MainView_WorldBoss);
        }
        public static void OpenDungeonDetailView(int dungeonID)
        {
            Logic.UI.DungeonDetail.View.DungeonDetailView.Open(dungeonID);
        }
        public static void CLIENT2LOBBY_PVE_FIGHT_REQ(int dungeonID)
        {
            Logic.Dungeon.Controller.DungeonController.instance.CLIENT2LOBBY_PVE_FIGHT_REQ(dungeonID);
        }
        public static void ShowIllustrationDetailView(int heroId, int heroStar)
        {
            RoleInfo info = null;
            HeroData data = HeroData.GetHeroDataByID(heroId);
            if (data.hero_type == 2)
            {
                info = new PlayerInfo((uint)0, (uint)heroId, (uint)0, (uint)0, (uint)0, 0, "");
                info.advanceLevel = heroStar;
            }
            else
            {
                info = new HeroInfo(0, heroId, 1, 0, heroStar, GlobalData.GetGlobalData().playerLevelMax);
            }
            IllustratedHandbookProxy.instance.CheckedDetailRoleInfo = info;
            Logic.UI.IllustratedHandbook.View.IllustrationDetailView.Open(info);
        }
        public static string[] GetDropGoodsPathDatas(int type, int id, int star, int limitCount)
        {
            List<PathData> pathList = DropMessageData.GetPathDatas(new GameResData((BaseResType)type, id, 0, star), limitCount);
			if (pathList == null)
				return null;
            string[] path = new string[pathList.Count];
            for (int i = 0; i < pathList.Count; i++)
            {
                PathData d = pathList[i];
                path[i] = string.Format("{0},{1}", (int)d.type, d.id);
            }
            return path;
        }
        public static void ClickIllustrationSkillDisplayHandler(LuaTable roleInfoLuaTable, bool isPlayer)
        {
            RoleInfo _roleInfo = null;
            if (isPlayer)
                _roleInfo = new PlayerInfo(roleInfoLuaTable);
            else
                _roleInfo = new HeroInfo(roleInfoLuaTable);
            Dictionary<RoleAttributeType, RoleAttribute> attrDic = RoleUtil.CalcRoleAttributesDic(_roleInfo);

            if (_roleInfo is PlayerInfo)
            {
                PlayerFightProtoData pfpd = new PlayerFightProtoData();
                pfpd.posIndex = 5;
                pfpd.attr = new HeroAttrProtoData();
                pfpd.attr.hpUp = pfpd.attr.hp = (int)attrDic.GetValue(RoleAttributeType.HP).value;
                pfpd.attr.magic_atk = (int)attrDic.GetValue(RoleAttributeType.MagicAtk).value;
                pfpd.attr.normal_atk = (int)attrDic.GetValue(RoleAttributeType.NormalAtk).value;
                pfpd.attr.normal_def = pfpd.attr.magic_def = (int)attrDic.GetValue(RoleAttributeType.Normal_Def).value;
                //pfpd.attr.magic_def = (int)attrDic.GetValue(RoleAttributeType.m).value;
                pfpd.attr.speed = (int)attrDic.GetValue(RoleAttributeType.Speed).value;
                pfpd.attr.hit = (int)attrDic.GetValue(RoleAttributeType.Hit).value;
                pfpd.attr.dodge = (int)attrDic.GetValue(RoleAttributeType.Dodge).value;
                pfpd.attr.crit = (int)attrDic.GetValue(RoleAttributeType.Crit).value;
                pfpd.attr.anti_crit = (int)attrDic.GetValue(RoleAttributeType.AntiCrit).value;
                pfpd.attr.block = (int)attrDic.GetValue(RoleAttributeType.Block).value;
                pfpd.attr.anti_block = (int)attrDic.GetValue(RoleAttributeType.AntiBlock).value;
                pfpd.attr.counter_atk = (int)attrDic.GetValue(RoleAttributeType.CounterAtk).value;
                pfpd.attr.crit_hurt_add = (int)attrDic.GetValue(RoleAttributeType.CritHurtAdd).value;
                pfpd.attr.crit_hurt_dec = (int)attrDic.GetValue(RoleAttributeType.CritHurtDec).value;
                pfpd.attr.armor = (int)attrDic.GetValue(RoleAttributeType.Armor).value;
                pfpd.attr.damage_add = (int)attrDic.GetValue(RoleAttributeType.DamageAdd).value;
                pfpd.attr.damage_dec = (int)attrDic.GetValue(RoleAttributeType.DamageDec).value;
                //pfpd.attr.hit = 100;
                FightPlayerInfo fightPlayerInfo = new FightPlayerInfo(_roleInfo as PlayerInfo, pfpd);
                FightProxy.instance.SetFightPlayerInfo(fightPlayerInfo);
                FightProxy.instance.SetFightHeroInfoList(new List<FightHeroInfo>());
            }
            else
            {
                HeroFightProtoData hfpd = new HeroFightProtoData();
                hfpd.posIndex = 5;
                hfpd.attr = new HeroAttrProtoData();
                hfpd.attr.hp = hfpd.attr.hpUp = (int)attrDic.GetValue(RoleAttributeType.HP).value;
                hfpd.attr.magic_atk = (int)attrDic.GetValue(RoleAttributeType.MagicAtk).value;
                hfpd.attr.normal_atk = (int)attrDic.GetValue(RoleAttributeType.NormalAtk).value;
                hfpd.attr.normal_def = hfpd.attr.magic_def = (int)attrDic.GetValue(RoleAttributeType.Normal_Def).value;
                //pfpd.attr.magic_def = (int)attrDic.GetValue(RoleAttributeType.m).value;
                hfpd.attr.speed = (int)attrDic.GetValue(RoleAttributeType.Speed).value;
                hfpd.attr.hit = (int)attrDic.GetValue(RoleAttributeType.Hit).value;
                hfpd.attr.dodge = (int)attrDic.GetValue(RoleAttributeType.Dodge).value;
                hfpd.attr.crit = (int)attrDic.GetValue(RoleAttributeType.Crit).value;
                hfpd.attr.anti_crit = (int)attrDic.GetValue(RoleAttributeType.AntiCrit).value;
                hfpd.attr.block = (int)attrDic.GetValue(RoleAttributeType.Block).value;
                hfpd.attr.anti_block = (int)attrDic.GetValue(RoleAttributeType.AntiBlock).value;
                hfpd.attr.counter_atk = (int)attrDic.GetValue(RoleAttributeType.CounterAtk).value;
                hfpd.attr.crit_hurt_add = (int)attrDic.GetValue(RoleAttributeType.CritHurtAdd).value;
                hfpd.attr.crit_hurt_dec = (int)attrDic.GetValue(RoleAttributeType.CritHurtDec).value;
                hfpd.attr.armor = (int)attrDic.GetValue(RoleAttributeType.Armor).value;
                hfpd.attr.damage_add = (int)attrDic.GetValue(RoleAttributeType.DamageAdd).value;
                hfpd.attr.damage_dec = (int)attrDic.GetValue(RoleAttributeType.DamageDec).value;
                //hfpd.attr.hit = 100;
                FightHeroInfo fightHeroInfo = new FightHeroInfo(_roleInfo as HeroInfo, hfpd);
                List<FightHeroInfo> heroInfoList = new List<FightHeroInfo>();
                heroInfoList.Add(fightHeroInfo);
                FightProxy.instance.SetFightHeroInfoList(heroInfoList);
            }
            //enemy
            HeroInfo enemyHero = new HeroInfo(HeroData.HeroDataDictionary.First().Key);
            HeroFightProtoData ehfpd = new HeroFightProtoData();
            ehfpd.posIndex = 5;
            ehfpd.attr = new HeroAttrProtoData();
            ehfpd.attr.hp = ehfpd.attr.hpUp = 100000;
            //			ehfpd.attr.magic_atk = 1;
            //			ehfpd.attr.normal_atk = 1;
            ehfpd.attr.speed = 25;
            FightHeroInfo efightHeroInfo = new FightHeroInfo(enemyHero, ehfpd);
            List<FightHeroInfo> eheroInfoList = new List<FightHeroInfo>();
            eheroInfoList.Add(efightHeroInfo);
            FightProxy.instance.SetEnemyFightHeroInfoList(eheroInfoList);


            UIMgr.instance.CloseLayerBelow(Logic.UI.EUISortingLayer.FlyWord);
            FightController.instance.fightType = Logic.Enums.FightType.SkillDisplay;
            Logic.UI.LoadGame.Controller.LoadGameController.instance.SetDelayTime(0.5f, () =>
            {
                FightController.instance.ReadyFight();
            });
        }

        public static void IOSVerifySuccess()
        {
            PlatformProxy.instance.PaySuccess();
        }

        public static void TDGAMissionOnBegin(string missId, int talkDataMissionType)
        {
            Logic.TalkingData.Controller.TalkingDataController.instance.TDGAMissionOnBegin(missId, (Logic.TalkingData.Controller.TalkDataMissionType)talkDataMissionType);
        }

        public static void TDGAMissionOnCompleted(string missId, int talkDataMissionType)
        {
            Logic.TalkingData.Controller.TalkingDataController.instance.TDGAMissionOnCompleted(missId, (Logic.TalkingData.Controller.TalkDataMissionType)talkDataMissionType);
        }
		[NoToLua]
		public static void OpenRewardTipsView(List<GameResData> rewardList)
		{
			LuaTable tips_model = LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","tips_model")[0] as LuaTable;
			LuaTable tip_view =tips_model.GetLuaFunction("GetTipView").Call("common_reward_tips_view")[0] as LuaTable;
			tip_view.GetLuaFunction("CreateByCSharpGameResDataList").Call(rewardList);
		}
		public static void OpenCommonRuleTipsView(string title,string des)
		{
			CommonRuleTipsView.Open(title,des);
		}

		public static int GetChannelID ()
		{
			return PlatformProxy.instance.GetPlatformId();
		}
        public static void SendGameInfo(String infoJson)
        {
            PlatformProxy.instance.SendGameInfo(infoJson);
        }
    }
}