--gamedataTable = {}
--管理器--
gamedataTable = {}
--local this = gamedataTable

function gamedataTable.InitGamedata()
    --gamedataTable = {}
    GetLuaProcotolIdList()
    InitLuaPaths()
    InitIgnorePaths()
    InitPreFightPaths()
    InitLoadTips()
    gamedataTable.aesKey = '1234567812345678'
    print("init game data ..")
end

function gamedataTable.DestroyGameData()
  	gamedataTable = nil
end

function GetLuaProcotolIdList()
	gamedataTable.procotolIdList =
	{
        MSG.PackResp,
        MSG.BuyPackCellResp,
      --活动
        MSG.ActivityListResp,
        MSG.ActivityRewardResp,
        MSG.ActivityJoinResp,
        MSG.ActivityUpdateResp,
        MSG.VipDailyGiftResp,
        MSG.OnlineGiftResp,
        MSG.OnlineGiftSynResp,
        MSG.SevenDayInfoResp,
        MSG.BuySevenDayGoodsResp,
        MSG.GetSevenDayCompleteAwardResp,
        MSG.GetSevenDayTaskAwardResp,
        --礼包码
        MSG.GiftCodeUseResp,
        --转盘
        MSG.LuckyLetteInfoResp,
        MSG.UseLuckyLetteResp,
        MSG.UseLuckyRouletteTenResp,
        --探险
        MSG.ExploreListResp,
        MSG.ExploreTaskResp,
        MSG.ExploreTaskRewardResp,
        MSG.ExploreTaskRefreshResp,
        MSG.ExploreTaskSynResp,
        MSG.BaseResourceSyn,
        
        -- [[ Player ]] --
        MSG.PlayerAggrResp,
        MSG.PlayerBreakResp,
        MSG.PlayerChangeResp,
        MSG.PlayerTransferResp,
        MSG.PlayerAdvanceResp,
        -- [[ Player ]] --
        
        MSG.GetAllHeroResp,
        MSG.HeroUpdateResp,
        MSG.HeroAggrResp,
        MSG.HeroBreakResp,
        MSG.HeroAdvanceResp,
        MSG.HeroRelationResp,
        MSG.HeroComposeResp,
        MSG.HeroPieceComposeResp,
        MSG.HeroDeComposeResp,
        MSG.HeroLockResp,
        MSG.HeroUnLockResp,
        MSG.SendHeroLockId,
        
        --equip
        MSG.GetAllEquipResp,
        MSG.EquipUpdateResp,
        MSG.EquipWearOffResp,
        MSG.EquipAggrResp,
        MSG.EquipSellResp,
        MSG.EquipUpgradeResp,
        MSG.EquipRecastResp,
        MSG.EquipRecastAffirmResp,
        MSG.EquipInlayGemResp,
        MSG.InlayGemSlotUnlockResp,
        MSG.InlayGemComposeResp,
        MSG.EnchantingScrollComposeResp,
        MSG.EquipEnchantResp,
        MSG.StarGemComposeResp,
        MSG.EquipStarResp,
        MSG.EquipInheritResp,
         --装备合成
        MSG.EquipComposeResp,
        MSG.EquipPieceComposeResp,
        --装备分解
        MSG.EquipDeComposeResp,
        
        -- [[ SHOP ]] --
        MSG.PurchaseGoodsResp,
        MSG.PurchaseDrawCardGoodsResp,
        MSG.DrawCardGoodsResp,
        MSG.DrawCardGoodsUpdateResp,
        MSG.LimitGoodsResp,
        MSG.LimitGoodsUpdateResp,
        MSG.OtherGoodsResp,
        MSG.OtherGoodsUpdateResp,
        MSG.DiamondGoodsResp,
        MSG.AppStoreVerifyResp,
        MSG.PurchaseDrawAwardResp,
        MSG.MonthsCardInfoResp,
        
        -- [[ VIP ]] --
        MSG.VipInfoResp,
        MSG.VipGiftBagResp,
        
        MSG.IllustrationResp,
        MSG.IllustrationSynResp,
        
        MSG.RoleLvAndExpResp,
        --阵型
        MSG.TeamAddResp,
        MSG.TeamInfoResp,
        MSG.LineupAttrActiveResp,
        MSG.LineupAddResp,
        MSG.LineupPointSynResp,
        MSG.LineupPointBuyResp,
        MSG.LineupUpgradeResp,
        --player
        MSG.GetAllPlayerResp,
        MSG.PlayerUpdateResp,
        MSG.PlayerTransferResp,
        MSG.PlayerChangeResp,
        MSG.TalentSynResp,
        MSG.TalentActivateResp,
        MSG.TalentUpgradeResp,
        MSG.TalentChooseResp,
        --item
        MSG.GetAllItemResp,
        MSG.ItemUpdateResp,
        MSG.RankListResp,
        MSG.ExpPotionResp,
        MSG.OpenGiftBagResp,
        --黑市
        MSG.BlackMarketResp,
        MSG.BlackMarketUpdateResp,
        MSG.PurchaseBlackGoodsResp,
        --公会
        MSG.GuildResp,
        MSG.GuildListResp,
        MSG.GuildMemberListResp,
        MSG.GuildReqListResp,
        MSG.GuildCreateResp,
        MSG.GuildDismissResp,
        MSG.GuildAddResp,
        MSG.GuildAnswerResp,
        MSG.GuildKickResp,
        MSG.GuildExitResp,
        MSG.GuildNoticeResp,
        MSG.GuildSignResp,
        MSG.GuildPresentResp,
        MSG.GuildSynResp,
        MSG.KickSynResp,
        MSG.GuildShopInfoResp,
        MSG.GuildShopBuyResp,
        MSG.GuildShopSynInfoResp,
        MSG.GuildAutoPassResp,
        MSG.GuildAutoPassInfoResp,
        -- [[ PVE ]] --
        MSG.PveInfoResp,
        --MSG.PveFightResp,
        --MSG.PveFightOverResp,
        --MSG.PveMopUpResp,
        --MSG.PveTenMopUpResp,
        
        MSG.StarsAwardInfoResp,
        MSG.GetStarsAwardResp,
        -- [[ PVE ]] --

        -- [[ 聊天 ]] --
        MSG.ChatResp,
        MSG.ChatInfoResp,
        MSG.CheckRoleGuildResp,
        
        --[[pvp race]]--
        MSG.GetPvpInfoResp,
        --MSG.PointPvpChallengeResp,
        --MSG.PointPvpSettleResp,
        
        --登陆获取订单号
        MSG.PayOrderNoResp,
        --login
        MSG.VerifyCDKEYResp,
        --远征
        MSG.ExpeditionResp,
        MSG.SynExpeditionResp,
        --竞技场
        MSG.GetWinTimesAwardResp,
        --矿战
        MSG.GetMineMapResp,
        MSG.GetMineInfoResp,
        MSG.GetOccRoleInfoResp,
        MSG.GetOwnInfoResp,
        MSG.OccMineResp,
        MSG.AbandonMineResp,
        MSG.GetMineAwardResp,
        MSG.ChangeMineInfoResp,
        --好友
        MSG.RoleInfoLookUpResp,
       --点金手
       MSG.GoldHandInfoResp,
       MSG.GoldHandUseResp,
	}
end

function InitLuaPaths()
  gamedataTable.paths = {'lua/user/common/'}
end

function InitIgnorePaths()
  gamedataTable.ignorePaths = {'shader/','material/','fonts/','sprite/main_ui','sprite/equipment_icon','sprite/item_icon','sprite/head_icon','ui/tutorial/'}
end

function InitPreFightPaths()
  gamedataTable.preFightPaths = {'sprite/skill_head'}
end

function InitLoadTips()
  gamedataTable.loadTips = {
    '英雄通过强化可获得更强大了力量！',
    '每一次转职新的使魔，都是一次重生，潜力UP！',
    '技能这种东西吧，该用的时候还是要用，别省！',
    '出于诚信考虑，赏赐给部下的装备不要轻易收回。',
    '10连抽之后，腰不酸了，腿不疼了，推图也有劲了！',
    '把敌人打浮空后，就狠狠的追击吧！',
    '有红点代表有奖励可以领取，记得经常回去看看哦！',
    '不强化，不升级，不调整阵型？快给我去面壁思过！',
	'装备记得要经常强化哟！',
	'咩咩羊可是好东西，强化英雄可是一级棒！',
    '用不着的英雄就没必要培养了，你知道的吧？',
    }
end