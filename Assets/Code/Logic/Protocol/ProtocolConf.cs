using Logic.Protocol.Model;
using LuaInterface;
using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

namespace Logic.Protocol
{
	public class EmptyMessage:ProtoBuf.IExtensible
	{
		public IExtension GetExtensionObject(bool createIfMissing)
		{
			return null;
		}
	}

    /// <summary>
    /// 协议配置文件
    /// </summary>
    public class ProtocolConf : SingletonMono<ProtocolConf>
    {
        public static int PACKGE_LEN = 4;
        public static int PROTOCOL_HEAD_LENGTH = 40;

        private Dictionary<int, System.Type> _protocolDic;
        private Dictionary<System.Type, int> _protocolDicReverse;

        private Dictionary<int, System.Type> _loginServerProtocolDic;
        private Dictionary<System.Type, int> _loginServerProtocolDicReverse;

        private List<int> _showMasks;

        private List<int> _luaProcotols;
        void Awake()
        {
            _protocolDic = new Dictionary<int, System.Type>();
            _protocolDicReverse = new Dictionary<Type, int>();
            _loginServerProtocolDic = new Dictionary<int, System.Type>();
            _loginServerProtocolDicReverse = new Dictionary<Type, int>();
            _showMasks = new List<int>();
            _luaProcotols = new List<int>();
            instance = this;
        }

        void Start()
        {
            #region server
            AddProtocol(MSG.ErrorResp, typeof(ErrorResp));
            AddProtocol(MSG.GMReq, typeof(GMReq), false);
            AddProtocol(MSG.ServerTimeSynResp, typeof(ServerTimeSynResp), false);
            AddProtocol(MSG.UserLogoutReq, typeof(UserLogoutReq), false);
            #endregion server

            #region login
            AddProtocol(MSG.LoginReq, typeof(LoginReq));
            AddProtocol(MSG.LoginResp, typeof(LoginResp));
            AddProtocol(MSG.PalyerRoleResp, typeof(PlayerRoleResp));
            AddProtocol(MSG.CreateRoleReq, typeof(CreateRoleReq));
            AddProtocol(MSG.CreateRoleResp, typeof(CreateRoleResp));

            AddProtocol(MSG.BaseResourceSyn, typeof(BaseResourceSyn));
            AddProtocol(MSG.RoleLvAndExpResp, typeof(RoleLvAndExpResp));
            AddProtocol(MSG.RedPointPromptResp, typeof(RedPointPromptResp));
            AddProtocol(MSG.UserLogoutResp, typeof(UserLogoutResp));
            AddProtocol(MSG.ReconnectReq, typeof(ReconnectReq));
            AddProtocol(MSG.ReconnectResp, typeof(ReconnectResp));
            AddProtocol(MSG.ClientActiveReq, typeof(ClientActiveReq), false);

            AddProtocol(MSG.RoleHeadReq, typeof(RoleHeadReq), false);
            AddProtocol(MSG.RoleHeadResp, typeof(RoleHeadResp));
            AddProtocol(MSG.RoleNameReq, typeof(RoleNameReq));
            AddProtocol(MSG.RoleNameResp, typeof(RoleNameResp));
            AddProtocol(MSG.GetCreateTimeReq, typeof(GetCreateTimeReq),false);
            AddProtocol(MSG.GetCreateTimeResp, typeof(StringProto));
            #endregion login
			AddProtocol(MSG.RefreshDayTimesResp, typeof(EmptyMessage));

            #region guide
            AddProtocol(MSG.GuideReq, typeof(GuideReq), false);
            #endregion guide

            #region player
            AddProtocol(MSG.GetAllPlayerReq, typeof(GetAllPlayerReq));
            AddProtocol(MSG.GetAllPlayerResp, typeof(GetAllPlayerResp));
            AddProtocol(MSG.PlayerUpdateResp, typeof(PlayerUpdateResp));
            AddProtocol(MSG.PlayerTransferReq, typeof(PlayerTransferReq));
            AddProtocol(MSG.PlayerTransferResp, typeof(PlayerTransferResp));
            AddProtocol(MSG.PlayerChangeReq, typeof(PlayerChangeReq));
            AddProtocol(MSG.PlayerChangeResp, typeof(PlayerChangeResp));
            AddProtocol(MSG.PlayerAggrReq, typeof(PlayerAggrReq));
            AddProtocol(MSG.PlayerAggrResp, typeof(PlayerAggrResp));
            AddProtocol(MSG.PlayerBreakReq, typeof(PlayerBreakReq));
            AddProtocol(MSG.PlayerBreakResp, typeof(PlayerBreakResp));
            AddProtocol(MSG.TalentActivateReq, typeof(TalentActivateReq));
            AddProtocol(MSG.TalentActivateResp, typeof(TalentActivateResp));
            AddProtocol(MSG.TalentUpgradeReq, typeof(TalentUpgradeReq));
            AddProtocol(MSG.TalentUpgradeResp, typeof(TalentUpgradeResp));
            AddProtocol(MSG.TalentChooseRep, typeof(TalentChooseRep), false);
            AddProtocol(MSG.TalentChooseResp, typeof(TalentChooseResp));
            AddProtocol(MSG.TalentSynResp, typeof(TalentSynResp));
            #endregion player

            #region hero
            AddProtocol(MSG.GetAllHeroReq, typeof(GetAllHeroReq));
            AddProtocol(MSG.GetAllHeroResp, typeof(GetAllHeroResp));

            AddProtocol(MSG.HeroUpdateResp, typeof(HeroUpdateResp));
            AddProtocol(MSG.HeroAggrReq, typeof(HeroAggrReq));
            AddProtocol(MSG.HeroAggrResp, typeof(HeroAggrResp));

            AddProtocol(MSG.HeroBreakReq, typeof(HeroBreakReq));
            AddProtocol(MSG.HeroBreakResp, typeof(HeroBreakResp));

           // AddProtocol(MSG.HeroComposeReq, typeof(HeroComposeReq));
           // AddProtocol(MSG.HeroComposeResp, typeof(HeroComposeResp));

            AddProtocol(MSG.HeroAdvanceReq, typeof(HeroAdvanceReq));
            AddProtocol(MSG.HeroAdvanceResp, typeof(HeroAdvanceResp));
            #endregion hero

            #region equip
            AddProtocol(MSG.GetAllEquipReq, typeof(GetAllEquipReq));
            AddProtocol(MSG.GetAllEquipResp, typeof(GetAllEquipResp));
            AddProtocol(MSG.EquipUpdateResp, typeof(EquipUpdateResp));
            AddProtocol(MSG.EquipWearOffReq, typeof(EquipWearOffReq));
            AddProtocol(MSG.EquipWearOffResp, typeof(EquipWearOffResp));
            AddProtocol(MSG.EquipUpgradeReq, typeof(IntProto));
            //AddProtocol(MSG.EquipUpgradeResp, typeof(IntProto));
            AddProtocol(MSG.EquipRecastReq, typeof(IntProto));
            //AddProtocol(MSG.EquipRecastResp, typeof(EquipRecastResp));
            AddProtocol(MSG.EquipRecastAffirmReq, typeof(EquipRecastAffirmReq));

            AddProtocol(MSG.EquipSellReq, typeof(IntProto));
            AddProtocol(MSG.EquipSellResp, typeof(IntProto));
            #endregion equip

            #region pack
            //            AddProtocol(MSG.TeamResp, typeof(TeamResp));
            //
            //            AddProtocol(MSG.PveTeamChangeReq, typeof(PveTeamChangeReq), false);
            //            AddProtocol(MSG.PveTeamChangeResp, typeof(PveTeamChangeResp));

            AddProtocol(MSG.PackReq, typeof(PackReq));
            AddProtocol(MSG.PackResp, typeof(PackResp));

            AddProtocol(MSG.BuyPackCellReq, typeof(BuyPackCellReq));
            AddProtocol(MSG.BuyPackCellResp, typeof(BuyPackCellResp));

            AddProtocol(MSG.IllustrationReq, typeof(IllustrationReq));
            AddProtocol(MSG.IllustrationResp, typeof(IllustrationResp));
            AddProtocol(MSG.IllustrationSynResp, typeof(IllustrationSynResp));

            #endregion pack

            #region pve
            AddProtocol(MSG.SynPveActionReq, typeof(SynPveActionReq));
            AddProtocol(MSG.SynPveActionResp, typeof(SynPveActionResp));
            AddProtocol(MSG.PveInfoReq, typeof(PveInfoReq));
            AddProtocol(MSG.PveInfoResp, typeof(PveInfoResp));
            AddProtocol(MSG.PveFightReq, typeof(PveFightReq));
            AddProtocol(MSG.PveFightResp, typeof(PveFightResp));
            AddProtocol(MSG.PveFightOverReq, typeof(PveFightOverReq));
            AddProtocol(MSG.PveFightOverResp, typeof(PveFightOverResp));

            AddProtocol(MSG.ActivityPveReq, typeof(ActivityPveReq));
            AddProtocol(MSG.ActivityPveResp, typeof(ActivityPveResp));
            AddProtocol(MSG.ActivityPveChallengeReq, typeof(ActivityPveChallengeReq));
            AddProtocol(MSG.ActivityPveChallengeResp, typeof(ActivityPveChallengeResp));
            AddProtocol(MSG.ActivityPveOverReq, typeof(ActivityPveOverReq));
            AddProtocol(MSG.ActivityPveOverResp, typeof(ActivityPveOverResp));
//            AddProtocol(MSG.ActivityPveDrawReq, typeof(ActivityPveDrawReq));
//            AddProtocol(MSG.ActivityPveDrawResp, typeof(ActivityPveDrawResp));
			// wangxf
			AddProtocol(MSG.ActivityPveAwardReq, typeof(IntProto));
			AddProtocol(MSG.ActivityPveAwardResp, typeof(IntProto));
            AddProtocol(MSG.PveMopUpReq, typeof(PveMopUpReq));
            AddProtocol(MSG.PveMopUpResp, typeof(PveMopUpResp));

            AddProtocol(MSG.WorldTreeReq, typeof(WorldTreeReq));
            AddProtocol(MSG.WorldTreeResp, typeof(WorldTreeResp));
            AddProtocol(MSG.WorldTreeFruitSynReq, typeof(WorldTreeFruitSynReq));
            AddProtocol(MSG.WorldTreeFruitSynResp, typeof(WorldTreeFruitSynResp));
            AddProtocol(MSG.WorldTreeChallengeReq, typeof(WorldTreeChallengeReq));
            AddProtocol(MSG.WorldTreeChallengeResp, typeof(WorldTreeChallengeResp));
            AddProtocol(MSG.WorldTreeSettleReq, typeof(WorldTreeSettleReq));
            AddProtocol(MSG.WorldTreeSettleResp, typeof(WorldTreeSettleResp));

            AddProtocol(MSG.WorldBossTimeResp, typeof(WorldBossTimeResp));
            AddProtocol(MSG.WorldBossReq, typeof(WorldBossReq));
            AddProtocol(MSG.WorldBossResp, typeof(WorldBossResp));
            AddProtocol(MSG.WorldBossChallengeReq, typeof(WorldBossChallengeReq));
            AddProtocol(MSG.WorldBossChallengeResp, typeof(WorldBossChallengeResp));
            AddProtocol(MSG.WorldBossInspireReq, typeof(WorldBossInspireReq));
            AddProtocol(MSG.WorldBossInspireResp, typeof(WorldBossInspireResp));
            AddProtocol(MSG.WorldBossReviveReq, typeof(WorldBossReviveReq));
            AddProtocol(MSG.WorldBossReviveResp, typeof(WorldBossReviveResp));
            AddProtocol(MSG.WorldBossHurtSynReq, typeof(WorldBossHurtSynReq), false);
            AddProtocol(MSG.WorldBossHurtSynResp, typeof(WorldBossHurtSynResp));
            AddProtocol(MSG.WorldBossKilledResp, typeof(WorldBossKilledResp));
            AddProtocol(MSG.WorldBossActivityEndResp, typeof(WorldBossActivityEndResp));
            AddProtocol(MSG.WorldBossSettleReq, typeof(WorldBossSettleReq));
            AddProtocol(MSG.WorldBossSettleResp, typeof(WorldBossSettleResp));

            AddProtocol(MSG.PveTenMopUpReq, typeof(PveTenMopUpReq));
            AddProtocol(MSG.PveTenMopUpResp, typeof(PveTenMopUpResp));


            #endregion pve

            #region shopping
            AddProtocol(MSG.PurchaseGoodsReq, typeof(PurchaseGoodsReq));
            AddProtocol(MSG.PurchaseGoodsResp, typeof(PurchaseGoodsResp));

            AddProtocol(MSG.PurchaseDrawCardGoodsResp, typeof(PurchaseDrawCardGoodsResp));
            AddProtocol(MSG.DrawCardGoodsReq, typeof(DrawCardGoodsReq));
            AddProtocol(MSG.DrawCardGoodsResp, typeof(DrawCardGoodsResp));
            AddProtocol(MSG.DrawCardGoodsUpdateResp, typeof(DrawCardGoodsUpdateResp));

            AddProtocol(MSG.LimitGoodsReq, typeof(LimitGoodsReq));
            AddProtocol(MSG.LimitGoodsResp, typeof(LimitGoodsResp));
            AddProtocol(MSG.LimitGoodsUpdateResp, typeof(LimitGoodsUpdateResp));

            AddProtocol(MSG.OtherGoodsReq, typeof(OtherGoodsReq));
            AddProtocol(MSG.OtherGoodsResp, typeof(OtherGoodsResp));
            AddProtocol(MSG.OtherGoodsUpdateResp, typeof(OtherGoodsUpdateResp));

            AddProtocol(MSG.BlackMarketReq, typeof(BlackMarketReq));
            AddProtocol(MSG.BlackMarketResp, typeof(BlackMarketResp));
            AddProtocol(MSG.BlackMarketUpdateResp, typeof(BlackMarketUpdateResp));
            AddProtocol(MSG.PurchaseBlackGoodsReq, typeof(PurchaseBlackGoodsReq));
            AddProtocol(MSG.PurchaseBlackGoodsResp, typeof(PurchaseBlackGoodsResp));

            #endregion shopping

            #region vip
            AddProtocol(MSG.VipInfoReq, typeof(VipInfoReq));
            AddProtocol(MSG.VipInfoResp, typeof(VipInfoResp));
            AddProtocol(MSG.VipGiftBagReq, typeof(VipGiftBagReq));
            AddProtocol(MSG.VipGiftBagResp, typeof(VipGiftBagResp));
            #endregion vip

            #region item
            AddProtocol(MSG.GetAllItemReq, typeof(GetAllItemReq));
            AddProtocol(MSG.GetAllItemResp, typeof(GetAllItemResp));
            AddProtocol(MSG.ItemUpdateResp, typeof(ItemUpdateResp));
            AddProtocol(MSG.ExpPotionReq, typeof(ExpPotionReq));
            AddProtocol(MSG.ExpPotionResp, typeof(ExpPotionResp));
            AddProtocol(MSG.OpenGiftBagReq, typeof(OpenGiftBagReq));
            AddProtocol(MSG.OpenGiftBagResp, typeof(OpenGiftBagResp));
			AddProtocol(MSG.BuySweepCouponsReq, typeof(IntProto));
			AddProtocol(MSG.BuySweepCouponsResp, typeof(IntProto));
            #endregion item

            #region pvp arena
            AddProtocol(MSG.GetRankArenaReq, typeof(GetRankArenaReq));
            AddProtocol(MSG.GetRankArenaResp, typeof(GetRankArenaResp));
            //            AddProtocol(MSG.RankArenaTeamChangeReq, typeof(RankArenaTeamChangeReq), false);
            //            AddProtocol(MSG.RankArenaTeamChangeResp, typeof(RankArenaTeamChangeResp));
            AddProtocol(MSG.GetRankArenaReportReq, typeof(GetRankArenaReportReq));
            AddProtocol(MSG.GetRankArenaReportResp, typeof(GetRankArenaReportResp));
            AddProtocol(MSG.RankArenaReportUpdateResp, typeof(RankArenaReportUpdateResp));
            AddProtocol(MSG.RefreshOpponentsReq, typeof(RefreshOpponentsReq));
            AddProtocol(MSG.RefreshOpponentsResp, typeof(RefreshOpponentsResp));
            AddProtocol(MSG.GetRankArenaRewardReq, typeof(GetRankArenaRewardReq));
            AddProtocol(MSG.GetRankArenaRewardResp, typeof(GetRankArenaRewardResp));
			AddProtocol(MSG.SynPvpActionResp,typeof(IntProto));
           // AddProtocol(MSG.GetRankingListReq, typeof(GetRankingListReq));
            //AddProtocol(MSG.GetRankingListResp, typeof(GetRankingListResp));
            AddProtocol(MSG.RankArenaChallengeReq, typeof(RankArenaChallengeReq));
            AddProtocol(MSG.RankArenaChallengeResp, typeof(RankArenaChallengeResp));
            AddProtocol(MSG.RankArenaChallengeOverReq, typeof(RankArenaChallengeOverReq));
            AddProtocol(MSG.RankArenaChallengeOverResp, typeof(RankArenaChallengeOverResp));
            //            AddProtocol(MSG.RankArenaTeamResp, typeof(RankArenaTeamResp));
            #endregion

            #region task
            AddProtocol(MSG.GetTasksReq, typeof(GetTasksReq));
            AddProtocol(MSG.GetTasksResp, typeof(GetTasksResp));
            AddProtocol(MSG.TaskUpdateResp, typeof(TaskUpdateResp));
            AddProtocol(MSG.GetTaskRewardReq, typeof(GetTaskRewardReq));
            AddProtocol(MSG.GetTaskRewardResp, typeof(GetTaskRewardResp));
            #endregion

            #region expedition远征
            AddProtocol(MSG.ExpeditionReq, typeof(ExpeditionReq));
            AddProtocol(MSG.ExpeditionResp, typeof(ExpeditionResp));
            AddProtocol(MSG.ExpeditionChallengeReq, typeof(ExpeditionChallengeReq));
            AddProtocol(MSG.ExpeditionChallengeResp, typeof(ExpeditionChallengeResp));
            //            AddProtocol(MSG.ExpeditionTeamChangeReq, typeof(ExpeditionTeamChangeReq), false);
            //            AddProtocol(MSG.ExpeditionTeamChangeResp, typeof(ExpeditionTeamChangeResp));
            AddProtocol(MSG.ExpeditionSettleReq, typeof(ExpeditionSettleReq));
            AddProtocol(MSG.ExpeditionSettleResp, typeof(ExpeditionSettleResp));
            AddProtocol(MSG.GetExpeditionRewardReq, typeof(GetExpeditionRewardReq));
            AddProtocol(MSG.GetExpeditionRewardResp, typeof(GetExpeditionRewardResp));
            AddProtocol(MSG.ResetExpeditionReq, typeof(ResetExpeditionReq));
            AddProtocol(MSG.SynExpeditionResp, typeof(SynExpeditionResp));

            #endregion
            #region mail
            AddProtocol(MSG.MailReq, typeof(MailReq));
            AddProtocol(MSG.MailResp, typeof(MailResp));
            AddProtocol(MSG.MailAttachmentReq, typeof(MailAttachmentReq));
            AddProtocol(MSG.MailAttachmentResp, typeof(MailAttachmentResp));
            AddProtocol(MSG.MailDelReq, typeof(MailDelReq));
            AddProtocol(MSG.MailDelResp, typeof(MailDelResp));
            AddProtocol(MSG.MailSynResp, typeof(MailSynResp));
			AddProtocol(MSG.MailReadReq, typeof(IntProto));
			AddProtocol(MSG.MailReadResp, typeof(EmptyMessage));

            #endregion
            #region chat
            AddProtocol(MSG.ChatReq, typeof(ChatReq));
            AddProtocol(MSG.ChatResp, typeof(ChatResp));
            AddProtocol(MSG.ChatInfoReq, typeof(ChatInfoReq));
            AddProtocol(MSG.ChatInfoResp, typeof(ChatInfoResp));
            #endregion

            #region friend
            AddProtocol(MSG.FriendListReq, typeof(FriendListReq));
            AddProtocol(MSG.FriendListResp, typeof(FriendListResp));
            AddProtocol(MSG.FriendAddReq, typeof(FriendAddReq));
            AddProtocol(MSG.FriendAddResp, typeof(FriendAddResp));
            AddProtocol(MSG.FriendAddAnswerReq, typeof(FriendAddAnswerReq));
            AddProtocol(MSG.FriendAddAnswerResp, typeof(FriendAddAnswerResp));
            AddProtocol(MSG.FriendDelReq, typeof(FriendDelReq));
            AddProtocol(MSG.FriendDelResp, typeof(FriendDelResp));
            AddProtocol(MSG.FriendPresentVimReq, typeof(FriendPresentVimReq));
            AddProtocol(MSG.FriendPresentVimResp, typeof(FriendPresentVimResp));
            AddProtocol(MSG.FriendGetVimReq, typeof(FriendGetVimReq));
            AddProtocol(MSG.FriendGetVimResp, typeof(FriendGetVimResp));
            AddProtocol(MSG.FriendGiftBagReq, typeof(FriendGiftBagReq));
            AddProtocol(MSG.FriendGiftBagResp, typeof(FriendGiftBagResp));
            AddProtocol(MSG.FriendLookUpTeamReq, typeof(FriendLookUpTeamReq), false);
            AddProtocol(MSG.FriendLookUpTeamResp, typeof(FriendLookUpTeamResp));
            AddProtocol(MSG.FriendMsgListReq, typeof(FriendMsgListReq));
            AddProtocol(MSG.FriendMsgListResp, typeof(FriendMsgListResp));
            AddProtocol(MSG.FriendRecommendListReq, typeof(FriendRecommendListReq));
            AddProtocol(MSG.FriendRecommendListResp, typeof(FriendRecommendListResp));
            AddProtocol(MSG.FriendFightReq, typeof(IntProto));
            AddProtocol(MSG.FriendFightResp, typeof(FriendFightResp));

            #endregion

            #region team
            AddProtocol(MSG.TeamInfoReq, typeof(TeamInfoReq));
            AddProtocol(MSG.TeamInfoResp, typeof(TeamInfoResp));
            AddProtocol(MSG.LineupUpgradeReq, typeof(LineupUpgradeReq));
            AddProtocol(MSG.LineupUpgradeResp, typeof(LineupUpgradeResp));
            AddProtocol(MSG.LineupAddResp, typeof(LineupAddResp));
            AddProtocol(MSG.TeamChangeReq, typeof(TeamChangeReq), false);
            AddProtocol(MSG.TeamChangeResp, typeof(TeamChangeResp));
            AddProtocol(MSG.TeamAddResp, typeof(TeamAddResp));
            AddProtocol(MSG.LineupPointBuyReq, typeof(LineupPointBuyReq));
            AddProtocol(MSG.LineupPointBuyResp, typeof(LineupPointBuyResp));
            AddProtocol(MSG.LineupPointSynReq, typeof(LineupPointSynReq));
            AddProtocol(MSG.LineupPointSynResp, typeof(LineupPointSynResp));
            AddProtocol(MSG.LineupAttrActiveResp, typeof(IntProto));
            AddProtocol(MSG.LineupAttrActiveReq, typeof(IntProto));
            #endregion

            #region team
            AddProtocol(MSG.SignInReq, typeof(SignInReq));
            AddProtocol(MSG.SignInResp, typeof(SignInResp));
            #endregion

            #region PVP
            AddProtocol(MSG.RealTimeFightDataResp, typeof(RealTimeFightDataResp));
            AddProtocol(MSG.FightCmdSynResp, typeof(FightCmdSynResp));
            AddProtocol(MSG.FightStartReq, typeof(FightStartReq));
            AddProtocol(MSG.SkillReq, typeof(SkillReq));
            AddProtocol(MSG.FightOverResp, typeof(FightOverResp));
            AddProtocol(MSG.PointPvpChallengeResp, typeof(PointPvpChallengeResp));
            AddProtocol(MSG.PointPvpSettleReq, typeof(PointPvpSettleReq));
            AddProtocol(MSG.PointPvpSettleResp, typeof(PointPvpSettleResp));
            #endregion

            #region mine
            AddProtocol(MSG.RobMineResp, typeof(RobMineResp));
            AddProtocol(MSG.PlunderMineResp, typeof(PlunderMineResp));
            AddProtocol(MSG.MineFightOverReq, typeof(MineFightOverReq));
            AddProtocol(MSG.MineFightOverResp, typeof(MineFightOverResp));
            #endregion
            InitLuaProcotol();
        }

        public void AddMaskId(int id)
        {
            _showMasks.Add(id);
        }

        private void AddProtocol(MSG protocolId, System.Type type, bool showMask = true)
        {
            int protocolIdInt = (int)protocolId;
            _protocolDic[protocolIdInt] = type;
            _protocolDicReverse[type] = protocolIdInt;
            if (showMask)
                _showMasks.Add(protocolIdInt);
        }

        private void AddLoginServerProtocol(EProtocolId protocolId, System.Type type)
        {
            int protocolIdInt = (int)protocolId;
            _loginServerProtocolDic[protocolIdInt] = type;
            _loginServerProtocolDicReverse[type] = protocolIdInt;
        }

        public static System.Type GetTypeByID(int protocolId)
        {
            if (instance._protocolDic.ContainsKey(protocolId))
            {
                return instance._protocolDic[protocolId];
            }
            Debugger.LogError("ProtocolConf can not find protocol id:" + protocolId);
            return null;
        }

        public static int GetIdByType(Type t)
        {
            if (instance._protocolDicReverse.ContainsKey(t))
            {
                return instance._protocolDicReverse[t];
            }
            Debugger.LogError("ProtocolConf can not find protocol Type:" + t);
            return ushort.MinValue;
        }


        public static System.Type GetLoginServerTypeByID(int protocolId)
        {
            if (instance._loginServerProtocolDic.ContainsKey(protocolId))
            {
                return instance._loginServerProtocolDic[protocolId];
            }
            Debugger.LogError("_loginServerProtocolDic ProtocolConf can not find protocol id:" + protocolId);
            return null;
        }

        public static int GetLoginServerIdByType(Type t)
        {
            if (instance._loginServerProtocolDicReverse.ContainsKey(t))
            {
                return instance._loginServerProtocolDicReverse[t];
            }
            Debugger.LogError("_loginServerProtocolDicReverse ProtocolConf can not find protocol Type:" + t);
            return ushort.MinValue;
        }

        private void InitLuaProcotol()
        {
            int[] protocols = LuaScriptMgr.Instance.LuaTableToArrayInt("gamedataTable.procotolIdList");
            //int[] procotols = LuaScriptMgr.Instance.LuaTableToArray<int>("gamedataTable.procotolIdList");
            if (protocols != null)
            {
                for (int i = 0, length = protocols.Length; i < length; i++)
                {
                    _luaProcotols.Add(protocols[i]);
                }
            }
        }

        public bool ExistInLuaProcotols(int procotolId)
        {
            //Debugger.Log("ttttt:" + procotolId.ToString());
            //foreach (var p in _luaProcotols)
            //{
            //    Debugger.Log("kkkkk:" + p.ToString());
            //}
            if (_luaProcotols.Contains(procotolId))
            {
                return true;
            }
            return false;
        }

        public static bool NeedShowMask(int protocolId)
        {
            return instance._showMasks.Contains(protocolId);
        }

    }

    public enum EProtocolId : ushort
    {

        ACCOUNT_LOGIN_REQ = (ushort)(((ushort)0x0F00 << 1) | 0X0001),
        PLATFORM_LOGIN_REQ = (ushort)(((ushort)0x0F01 << 1) | 0X0001),
        SERVER_LIST_REQ = (ushort)(((ushort)0x0F10 << 1) | 0X0001),
        SERVER_CHOOSE_REQ = (ushort)(((ushort)0x0F11 << 1) | 0X0001),

        LOGIN_RET = (ushort)((ushort)0x0F00 << 1),
        SERVER_LIST_RET = (ushort)((ushort)0x0F10 << 1),
        SERVER_CHOOSE_RET = (ushort)((ushort)0x0F11 << 1),
    }
}
