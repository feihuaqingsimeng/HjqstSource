BaseResType = {}

BaseResType.None = 0           --//
BaseResType.Resource = 1      --//no use
BaseResType.Hero = 2           --//
BaseResType.Equipment = 3      --//
BaseResType.Item = 4           --//

BaseResType.Hero_Exp = 5       --//
BaseResType.ArenaPoint = 6    --// 积分赛积分
BaseResType.Account_Exp = 7    --//
BaseResType.Gold = 8           --//
BaseResType.Diamond = 9        --//
BaseResType.Crystal = 10       --//shui jing
BaseResType.PveAction = 11     --//
BaseResType.TowerAction = 12   --//
BaseResType.PvpAction = 13     --//
BaseResType.Honor = 14         -- //
BaseResType.EquipSoul = 15 -- 器魂（装备分解获得）
BaseResType.ExpeditionPoint = 16--//远征币
BaseResType.WorldBossResource = 17 --//
BaseResType.FormationTrainPoint = 18 --//
BaseResType.ConsortiaPoint = 19     --//帮贡
                              --24英雄碎片
BaseResType.RMB = 100          --//

EquipmentType = {}
EquipmentType.None = 0              
EquipmentType.PhysicalWeapon = 1    
EquipmentType.MagicalWeapon = 4     
EquipmentType.Armor = 2             --fang ju
EquipmentType.Accessory = 3         --shi pin

EquipmentQuality = {}
EquipmentQuality.White = 1
EquipmentQuality.Green = 2
EquipmentQuality.Blue = 3
EquipmentQuality.Purple = 4
EquipmentQuality.Orange = 5
EquipmentQuality.Red = 6

EquipmentAttributeType = {}
EquipmentAttributeType.HP = 0                 --//
EquipmentAttributeType.NormalAtk = 1          --//
EquipmentAttributeType.MagicAtk = 2          -- //
EquipmentAttributeType.Def = 3               --//
EquipmentAttributeType.Speed = 4            --  //
EquipmentAttributeType.Hit = 5              -- //
EquipmentAttributeType.Dodge = 6            --  //shan bi
EquipmentAttributeType.Crit = 7             -- //bao ji
EquipmentAttributeType.AntiCrit = 8        -- //kang bao ji 
EquipmentAttributeType.Block = 9           --   //dang ge
EquipmentAttributeType.AntiBlock = 10       --  //po ji (kang dang ge)
EquipmentAttributeType.CounterAtk = 11       -- //fan ji
EquipmentAttributeType.CritHurtAdd = 12      -- //bao ji shang hai
EquipmentAttributeType.CritHurtDec = 13       --//bao ji shang hai jian mian
EquipmentAttributeType.Armor = 14            -- //po jia
EquipmentAttributeType.DamageAdd = 15        --//shang hai jian mian
EquipmentAttributeType.DamageDec = 16       --  //shang hai jia cheng
EquipmentAttributeType.HPPercent = 17       -- //生命百分比
EquipmentAttributeType.NormalAtkPercent = 18 -- //物理攻击百分比
EquipmentAttributeType.MagicAtkPercent = 19  --//魔法攻击百分比
EquipmentAttributeType.DefPercent = 20        -- //防御百分比(双防)


RoleAttributeType = {}     --//角色属性类型定义(主角与英雄共用)
RoleAttributeType.HP = 0               --  //生命力
RoleAttributeType.NormalAtk = 1        --  //物理攻击
RoleAttributeType.MagicAtk = 2         --  //魔法攻击
RoleAttributeType.Def = 3             --  //物理防御
RoleAttributeType.Speed = 4           --   //行动力
RoleAttributeType.Hit = 5             --   //命中
RoleAttributeType.Dodge = 6           --   //闪避
RoleAttributeType.Crit = 7             --  //暴击
RoleAttributeType.AntiCrit = 8         --  //抗暴击
RoleAttributeType.Block = 9           --   //格挡
RoleAttributeType.AntiBlock = 10        -- //破击(破格挡)
RoleAttributeType.CounterAtk = 11       -- //反击
RoleAttributeType.CritHurtAdd = 12      -- //暴击伤害
RoleAttributeType.CritHurtDec = 13      -- //暴击伤害减免
RoleAttributeType.Armor = 14             --//破甲
RoleAttributeType.DamageAdd = 15        --//伤害减免
RoleAttributeType.DamageDec = 16         --//伤害加成
RoleAttributeType.HPPercent = 17       -- //生命百分比
RoleAttributeType.NormalAtkPercent = 18 -- //物理攻击百分比
RoleAttributeType.MagicAtkPercent = 19  --//魔法攻击百分比
RoleAttributeType.DefPercent = 20        -- //防御百分比(双防)
RoleAttributeType.MAX = 21 

EUISortingLayer = {}
EUISortingLayer.MainUI = 0
EUISortingLayer.Tips = 1
EUISortingLayer.Notice = 2
EUISortingLayer.Select = 3
EUISortingLayer.FlyWord = 4
EUISortingLayer.Loading = 5
EUISortingLayer.Tutorial = 6

UIOpenMode = {}
UIOpenMode.Replace = 0
UIOpenMode.Overlay = 1

FormationState = {}
FormationState.Locked = 1
FormationState.NotInUse = 2
FormationState.InUse = 3

FormationTeamType = {}
FormationTeamType.pveFirstTeam = 101
FormationTeamType.pveSecondTeam = 102
FormationTeamType.pveThirdTeam = 103
--FormationTeamType.pvpTeam = 201
FormationTeamType.expeditionTeam = 301

FormationEffectType = {}
FormationEffectType.PhysicAtk = 1		--// 物理攻击%
FormationEffectType.MagicAtk = 2		--// 魔法攻击%
FormationEffectType.PhysicDef = 3		--// 物理防御%
FormationEffectType.MagicDef = 4		--// 魔法防御%
FormationEffectType.Hit = 5			--// 命中概率%
FormationEffectType.Dodge = 6			--// 闪避概率%
FormationEffectType.Crit = 7			--// 暴击概率%
FormationEffectType.AntiCrit = 8      -- // 防爆概率%
FormationEffectType.Block = 9			--// 格挡概率%
FormationEffectType.AntiBlock = 10     --// 破击概率%
FormationEffectType.CritHurtAdd = 11   --// 暴击伤害加成%
FormationEffectType.CritHurtDec = 12   --// 暴击伤害减免%
FormationEffectType.DamageAdd = 13     --// 伤害加成%
FormationEffectType.DamageDec = 14    -- // 伤害减免%
FormationEffectType.Weakness = 15     -- // 弱点攻击%
FormationEffectType.HPLimit = 16        -- // 血量上限增加%
FormationEffectType.TreatPercent = 17	--//  每一定时间回血比例%
FormationEffectType.TreatAdd = 18    -- // 治疗效果增加%


RoleType = {}
RoleType.Invalid = 0       
RoleType.Defence = 1       --//防御型
RoleType.Offence = 2       --//攻击型
RoleType.Mage = 3          --//魔法型
RoleType.Support = 4       --//辅助型
RoleType.Mighty = 5        --//全能型

RoleAttackAttributeType = {}
RoleAttackAttributeType.Invalid = 0         -- //无效值
RoleAttackAttributeType.PhysicalAttack = 1   --//物理攻击
RoleAttackAttributeType.MagicalAttack = 2    --//魔法攻击

DungeonType = {}
DungeonType.Invalid = 0                --无效值
DungeonType.Easy = 1                   --简单副本
DungeonType.Normal = 2                 --普通副本
DungeonType.Hard = 3                   --困难副本
DungeonType.ActivityNormal = 5         --活动普通副本
DungeonType.ActivityDamageOutput = 6   --活动伤害输出副本
DungeonType.WorldTree = 7              --世界树副本
DungeonType.ActivitySingle = 8         --活动单体技能副本
DungeonType.ActivityRange = 9          --活动范围技能副本
DungeonType.ActivityMirror = 10        --活动镜像副本
DungeonType.ActitityFloat = 11         --活动浮空伤害副本
DungeonType.BossSpecies = 20           --Boss亚种

--背包类型，与服务器对应
BagType = {}
BagType.HeroBag = 1           --英雄背包
BagType.EquipmentBag = 2      --装备背包

ItemType = {}
ItemType.Material = 1--1 材料
ItemType.Sweep = 2-- 扫荡卷
ItemType.ExpPotion = 3 -- 经验药水
ItemType.HeroExp = 5--5 英雄经验
ItemType.AccountExp = 7--7经验值
ItemType.Gold = 8--8金币
ItemType.Diamomd = 9--9钻石
ItemType.Crystal = 10--10 水晶
ItemType.PveAction = 11--11 行动力
ItemType.Honor = 14--14 荣誉
ItemType.ExpeditionPoint = 16--16远征币
ItemType.PetMaterial = 20--20 使魔材料
ItemType.Gem = 21--21宝石
ItemType.Enchant = 22--22附魔卷轴
ItemType.StarLevelUpGem = 23--23升星宝石
ItemType.HeroPiece = 24     --24英雄碎片
ItemType.RandomGift = 25    --25随机礼包
ItemType.EquipPiece = 26    --26装备碎片

RoleEquipPos = {}          -- 角色装备位置
RoleEquipPos.All = 0       -- 全部
RoleEquipPos.Weapon = 1    -- 武器
RoleEquipPos.Armor = 2     -- 防具
RoleEquipPos.Accessory = 3 -- 首饰

-- [[商店商品类型]] --
ShopItemType = {}
ShopItemType.HeroDrawCard = 11
ShopItemType.ArticleDrawCard = 12
ShopItemType.Diamond = 13
ShopItemType.Action = 14
ShopItemType.Gold = 15
ShopItemType.Goods = 16

---item 固定id
ITEM_ID = {}
ITEM_ID.smallExpSheep = 98	--//小经验羊
ITEM_ID.midExpSheep = 99	--//中经验羊
ITEM_ID.bigExpSheep = 100	--//大经验羊
ITEM_ID.smallExpMedicine = 10600	--//小经验药水 
ITEM_ID.midExpMedicine = 10601		--//中经验药水
ITEM_ID.bigExpMedicine = 10602		--//大经验药水


-- [[消费提示]] --
ConsumeTipType = {}
ConsumeTipType.None = 0
ConsumeTipType.DiamondDrawSingleHero = 1001	--钻石单抽英雄卡
ConsumeTipType.DiamondDrawTenHeroes = 1002    --钻石十抽英雄卡
ConsumeTipType.HonorDrawSingleHero = 1003		--荣誉单抽英雄卡
ConsumeTipType.DiamondBuyPveAction = 1014			--钻石购买体力值
ConsumeTipType.DiamondBuyWorldTreeCount = 1004	--钻石购买世界树果实
ConsumeTipType.DiamondBuyPvpCount = 1005			--钻石购买竞技场次数
ConsumeTipType.DiamondBuyCoin = 1006				--钻石购买金币
ConsumeTipType.DiamondBuyBlackmarket = 1007		--钻石购买黑市道具
ConsumeTipType.DiamondBuyFormationTrainPoint = 1008--钻石购买阵型培养点数(完)
ConsumeTipType.DiamondBuyWorldBossInspire = 1009	--钻石世界boss鼓舞(完)
ConsumeTipType.DiamondOpenCard = 1010				--钻石翻牌提示(完)
ConsumeTipType.DiamondResetExpeditionCount = 1011	--钻石重置远征次数(完)
ConsumeTipType.DiamondBuyEquipGrid = 1012			--钻石购买装备格子(完)
ConsumeTipType.DiamondBuyHeroGrid = 1013			--钻石购买英雄格子(完)
ConsumeTipType.GoldDrawSingleArticle = 1015			--金币单抽物品
ConsumeTipType.HonorDrawSingleArticle = 1016			--荣誉单抽物品
ConsumeTipType.GoldDrawTenArticles = 1017			    --金币十抽物品
ConsumeTipType.DiamondConsumeGuildDonate = 1019 	--公会捐献钻石消耗
ConsumeTipType.HeroDecompose = 1020               --英雄分解
ConsumeTipType.EquipDecompose = 1021               --装备分解
ConsumeTipType.ExploreRefresh = 1022               --探险刷新消费
ConsumeTipType.Turntable = 1023               --大转盘钻石消费

FunctionOpenType={}
FunctionOpenType.MainView_Task = 100     -- 主界面 任务按钮
FunctionOpenType.MainView_Mail = 200     --主界面 邮件按钮
FunctionOpenType.MainView_Friend = 300     --主界面 好友按钮
FunctionOpenType.MainView_SignIn = 400     --主界面 签到
FunctionOpenType.MainView_Activity = 500     --主界面 活动
FunctionOpenType.MainView_FirstCharge = 501     --主界面 首充活动
FunctionOpenType.MainView_Ranking = 600     --主界面 排行榜
FunctionOpenType.MainView_Player = 700     --主界面 主角
FunctionOpenType.PlayerChoice = 701     --使魔选择
FunctionOpenType.PlayerStrengthen = 703     --使魔强化
FunctionOpenType.PlayerTraining = 705     --使魔培养
FunctionOpenType.PlayerBreakthrough = 707     --使魔突破
FunctionOpenType.PlayerWearEquip = 709     --使魔穿戴
FunctionOpenType.MainView_Hero = 800    --主界面 培养按钮
FunctionOpenType.HeroStrengthen = 801     --英雄强化
FunctionOpenType.HeroAdvance = 803     --英雄升星
FunctionOpenType.HeroBreakthrough = 805     --英雄突破
FunctionOpenType.HeroDecompose = 807     --英雄分解
FunctionOpenType.MainView_FormationBtn = 900     --主界面 阵型按钮
FunctionOpenType.FormationTraining = 901     --更换阵型
FunctionOpenType.HeroSecondTeam = 905     --英雄二队
FunctionOpenType.HeroThirdTeam = 906     --英雄三队
FunctionOpenType.MainView_Equipment = 1000     --主界面 装备按钮
FunctionOpenType.EquipTraining = 1001     --装备培养

FunctionOpenType.EquipRecast = 1004     --装备重铸
FunctionOpenType.EquipEnchant = 1006     --装备附魔
FunctionOpenType.EquipUpStar = 1008     --装备升星
FunctionOpenType.EquipGemInsert = 1010     --装备镶嵌
FunctionOpenType.EquipAdvance = 1012		                  --//@[装备进阶]
FunctionOpenType.EquipInherit = 1013      --装备继承
FunctionOpenType.MainView_Shop = 1100     --主界面 商店
FunctionOpenType.MainView_BlackMarket = 1200     --主界面 黑市
FunctionOpenType.BlackMarket_Arena = 1202     --黑市 竞技场黑市
FunctionOpenType.BlackMarket_Expedition = 1203     --黑市 远征黑市
FunctionOpenType.BlackMarket_WorldBoss = 1204     --黑市 世界boss黑市
FunctionOpenType.BlackMarket_PvpRace = 1205     --黑市 积分黑市
FunctionOpenType.MainView_illustrate = 1300     -- 主界面 图鉴
FunctionOpenType.MainView_illustrate_hero_detial_view = 13004     --  英雄图鉴详情界面
FunctionOpenType.MainView_Consortia = 1400     -- 主界面 公会按钮
FunctionOpenType.MainView_Explore = 1500     -- 主界面 探险按钮
FunctionOpenType.MainView_FightCenter = 1600     -- 主界面 战斗中心按钮
FunctionOpenType.FightCenter_Arena = 1601     -- 战斗中心 竞技场
FunctionOpenType.FightCenter_WorldTree = 1604     -- 战斗中心 世界树
FunctionOpenType.FightCenter_Expedition = 1606     -- 战斗中心 远征
FunctionOpenType.FightCenter_PvpRace = 1608         --战斗中心 竞技场积分赛
FunctionOpenType.FightCenter_MineBattle = 1610      --战斗中心 矿战
FunctionOpenType.FightCenter_PvpRaceMatch = 16081        --积分赛匹配
FunctionOpenType.MainView_DailyDungeon = 1700     -- 主界面 每日副本
FunctionOpenType.MainView_PVE = 1800     -- 主界面 pve副本
FunctionOpenType.PVE_Normal = 1801     --  pve副本 普通难度
FunctionOpenType.PVE_Hard = 1802     --  pve副本 困难难度
FunctionOpenType.SingleSweep = 1803     --  pve副本 单次扫荡
FunctionOpenType.TenSweep = 1805     --  pve副本 十次扫荡
FunctionOpenType.DoubleSpeed = 1807     --  pve副本 两倍速
FunctionOpenType.AutoFight = 1808     -- pve 自动战斗
FunctionOpenType.FightPause = 1809     -- 战斗暂停
FunctionOpenType.MainView_WorldBoss = 1900    --主界面世界boss
FunctionOpenType.WorldBoss_inspireBtn = 1902   --世界boss鼓舞按钮
FunctionOpenType.MainView_Chat = 2000       --主界面 聊天按钮
FunctionOpenType.MainView_Pack = 2100       --主界面 背包按钮
FunctionOpenType.Boss_List_View = 2200       --副本选择界面 Boss列
FunctionOpenType.MainView_dailyTask = 2300       --每日任务
FunctionOpenType.MainView_compose = 2400       --合成
FunctionOpenType.GoldenTouchView = 2500       --点金手

ChatType={}
ChatType.World = 1		--世界聊天
ChatType.Guild = 4		--公会聊天
ChatType.Private = 2	--私人聊天
ChatType.System = 3		--系统公告

RedPointType = {}

RedPointType.None = 0
RedPointType.RedPoint_TaskAchievement = 1
RedPointType.RedPoint_TaskDaily = 2
RedPointType.RedPoint_TaskMain = 3
RedPointType.RedPoint_TaskAll = 4
RedPointType.RedPoint_Mail = 5
RedPointType.RedPoint_Chat = 6
RedPointType.RedPoint_FriendAll = 7
RedPointType.RedPoint_MyFriend = 8
RedPointType.RedPoint_FriendRequest = 9
RedPointType.RedPoint_Formation_specific = 10	--//具体阵型上显示
RedPointType.RedPoint_Formation = 11		--//阵型二级界面提示
RedPointType.RedPoint_Hero_New = 12     --//获得新英雄提示
RedPointType.RedPoint_VIP_Benefit = 13  -- //VIP福利
RedPointType.RedPoint_Shop_Has_Free_Item = 14   --//商店中有冷却好的免费抽卡
RedPointType.RedPoint_Activity = 15   --//可领取活动
RedPointType.RedPoint_Consortia_apply = 16  -- //公会申请提示
RedPointType.RedPoint_HeroAdvance = 17      --单个英雄进阶提示
RedPointType.RedPoint_HeroBreakthrough = 18 --单个英雄突破提示
RedPointType.RedPoint_HeroHasFitEquipment = 19 -- 英雄有可穿戴装备提示
RedPointType.RedPoint_All_Hero_advance_breakthrough = 20 --所有英雄进阶和突破提示
RedPointType.RedPoint_New_Equip = 21     ---获得新装备
RedPointType.RedPoint_SignIn = 22    ---签到提示
RedPointType.RedPoint_New_Item = 23    ---获得新类型道具提示
RedPointType.RedPoint_hero_relationship = 24   --单个阵上英雄可羁绊提示
RedPointType.RedPoint_all_hero_relationship = 25   --所有阵上英雄可羁绊提示
RedPointType.RedPoint_Small_Exp_Potion = 26        --小经验药水[有小经验药水，并且在培养界面中，当前选择英雄可升级]
RedPointType.RedPoint_Middle_Exp_Potion = 27       --中经验药水[有中经验药水，并且在培养界面中，当前选择英雄可升级]
RedPointType.RedPoint_Big_Exp_Potion = 28          --大经验药水[有大经验药水，并且在培养界面中，当前选择英雄可升级]

RedPointType.RedPoint_AnyInFormationHeroHasFitEquipment = 30 -- 上阵英雄有可穿戴装备提示
RedPointType.RedPoint_MineBattle = 31 -- 矿战红点
RedPointType.RedPoint_In_Pve_Formation_Hero_Can_Use_Exp_Potion = 32          --PVE上阵英雄中有可以吃经验药水升级的
RedPointType.RedPoint_PVP_Challenge_Reward = 33
RedPointType.RedPoint_Seven_Hilarity = 2600 --七日狂欢

ChannelType = {}
ChannelType.shunwang = 1
ChannelType.vivo = 2
ChannelType.qihoo360 = 3
ChannelType.huawei = 4
ChannelType.oppo = 5
ChannelType.uc = 6
  
--远征副本类型ExpeditionDungeonType = {}
ExpeditionDungeonType.Normal = 1
ExpeditionDungeonType.SmallTreasureBox = 2
ExpeditionDungeonType.BigTreasureBox = 3
--任务类型
TaskType = {}
TaskType.Invalid = 0                      --//无效值
TaskType.AccountLevelRequirement = 1      --//账号等级 等级要求 0
TaskType.PassDungeonTimes = 2            -- //通关副本 副本ID   次数
--TaskType.OwnHero = 3                  -- //拥有英雄 英雄ID(4项组合) 英雄数量     //暂时不使用次枚举
TaskType.ProfessionIDAndLevel = 4       -- //职业等级 职业ID 等级
TaskType.VIPLevelRequirement = 5         --//VIP等级 等级
TaskType.MultipleHeroLevelRequirement = 6-- //多个高等级英雄 英雄数量 英雄等级
TaskType.MultipleHeroStarRequirement = 7 -- //多个高星级英雄 英雄数量 英雄星级
TaskType.OwnResouce = 8                  -- //拥有资源 资源ID(4项组合)
TaskType.FirstTopUpInTime = 9            -- //指定时间首充奖励 时间
TaskType.SingleTopUp = 10                -- //单次充值总额 充值数值
TaskType.accumulateTopUp = 50		--	//累计充值总额	充值数值
TaskType.thousandTower = 11				--//千层塔达到层数
TaskType.heroBreakThrough = 12				--//伙伴突破次数
TaskType.MultipleEquipmentStarRequirement = 13--//多个高星级装备  装备数量，装备星级
TaskType.accumulateCostDiamind = 14			--	//累计消耗钻石  钻石数量
TaskType.singleUseHonor = 15				--	//一次性消费荣誉  荣誉数量
TaskType.accumulateOwnGold = 16				--	//累计拥有金币  金币数量
TaskType.accumulateTenLottery = 17			--	//累计十连抽 次数
TaskType.assignDungeonStar = 18			--	//指定副本星级
TaskType.assignDungeonDifficultyTotalStar = 19--	//指定难度总星星数
TaskType.accumulatePvpVictoryCount = 20		--	//竞技场累计胜利次数
TaskType.accumulatePvpFightCount = 21		--	//竞技场累计战斗次数
TaskType.promoteAccountLevel = 23			--	//提升账号等级
TaskType.strengthenEquipCount = 24			--	//强化装备次数
TaskType.boostNewDungeonCount = 25		--	//推进新副本次数
TaskType.dailyLogin = 26						--//每日登陆
TaskType.costPveAction = 27				--		//消耗体力
TaskType.dailyDungeonFight = 28				--	//参与每日副本
TaskType.pvpFightCount = 29					--	//竞技场战斗次数
TaskType.TenLotteryCount = 30				--	//十连抽次数
TaskType.thousandTowerFight = 31			--	//参与千层塔
TaskType.clearDungeon = 32					--	//通关某个副本

--图鉴查看类型
IllustrationType = {}
IllustrationType.hero = 1 --查看英雄
IllustrationType.equip = 2 -- 查看装备
IllustrationType.item = 3 ---查看道具

--黑市
BlackmarketType = {}
BlackmarketType.limitActivity = 5 --显示活动

-- [[角色品质]] --
RoleQuality = {}
RoleQuality.White = 1      --白
RoleQuality.Green = 2      --绿
RoleQuality.Blue = 3       --蓝
RoleQuality.Purple = 4     --紫
RoleQuality.Orange = 5     --橙
RoleQuality.Red = 6        --红

-- [[道具品质]] --
ItemQuality = {}
ItemQuality.White = 1      --白
ItemQuality.Green = 2      --绿
ItemQuality.Blue = 3       --蓝
ItemQuality.Purple = 4     --紫
ItemQuality.Orange = 5     --橙
ItemQuality.Red = 6        --红

-- [[ 通用品质 ]] --
Quality = {}
Quality.White = 1      --白
Quality.Green = 2      --绿
Quality.Blue = 3       --蓝
Quality.Purple = 4     --紫
Quality.Orange = 5     --橙
Quality.Red = 6        --红

--渠道类型
PlatformType = {}
PlatformType.shunwang = 1
PlatformType.vivo = 2
PlatformType.qihoo360 = 3
PlatformType.huawei = 4
PlatformType.oppo = 5
PlatformType.uc = 6
PlatformType.iOS = 99

--talkingdata mission type
TalkDataMissionType = {}    
TalkDataMissionType.Task = 1    --任务
TalkDataMissionType.Level = 2   --关卡
TalkDataMissionType.Tutorial = 3 -- 新手引导

--装备宝石镶嵌 选中状态
GemSelectState = {}
GemSelectState.NoSelect = 0  -- 0 未选中
GemSelectState.SelectEquipSlot = 1 --1 选中已装备槽
GemSelectState.SelectUnUseGem = 2 --2 选中未镶嵌宝石
--好友
FriendFuncType = {}
FriendFuncType.MyFriend = 1         --我的好友
FriendFuncType.RecommendFriend = 2  --好友推荐
FriendFuncType.RequestFriend = 3    --好友请求
--播声音
PlayAudioType = {}
PlayAudioType.none = 0--无限制，允许有重复的声音
PlayAudioType.SingleInGroup = 1--小组中只播放一个（注:正在播放不重置）
--播声音界面类型
AudioViewType = {}
AudioViewType.mainView = 1--主界面英雄
AudioViewType.taskView = 2 --任务界面
AudioViewType.shopView = 3-- 商店
AudioViewType.tutorialView = 4 -- 新手引导
