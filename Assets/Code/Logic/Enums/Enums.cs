using UnityEngine;
using System.Collections;
namespace Logic.Enums
{
    public enum LayerType
    {
        None = 0,
        UI = 5,
        Fight = 8,
        Scene = 9,
        FightCombo = 10,
        Closeup = 11,//特写
        SceneFront = 12,//前景
        UI3D = 13,//3d ui
        UIHide = 14, //UI隐藏
    }

    public enum FightOverType
    {
        None = 0,
        Timeout = 1,//超时
        Normal = 2,//正常        
        ForceOver = 3,//用户退出
    }

    public enum FightStatus
    {
        None = 0,
        Normal = 1,
        FloatWaiting = 2,//浮空连击等待状态
        TumbleWaiting = 3,//倒地连击等待状态
        FloatComboing = 4,//浮空连击
        TumbleComboing = 5,//倒地连击
        GameOver = 6//战斗结束
    }

    public enum FightType
    {
        None = 0,
        PVE = 1,         //普通副本
        Arena = 2,       //竞技场
        DailyPVE = 3,    //每日副本
        Expedition = 4,	 //远征
        WorldTree = 5,   //世界树
        WorldBoss = 6,   //世界Boss
        PVP = 7,         //PVP
        FirstFight = 8,  //第一场战斗
        SkillDisplay = 9,//技能展示
        ConsortiaFight = 10,//公会战
        FriendFight = 11,//好友战
        MineFight = 12,//矿战
#if UNITY_EDITOR
        Imitate = 100,//模拟战场
#endif
    }

    public enum Status
    {
        None = 0,
        Idle = 1,
        Skill = 2,
        GetHit = 3,//受击
        BootSkill = 4,//引导技能
        Dead = 6,
    }

    public enum SkillType
    {
        None = 0,
        Hit = 1,//普通攻击
        Skill = 2,//技能
        Aeon = 3,//召唤兽
        Passive = 4,//被动技能
    }

    //public enum SkillDesType
    //{
    //    None = 0,
    //    Normal = 1,//普攻
    //    Float = 2,//float only
    //    Tumble = 3,//tumble only
    //    FloatAndNormal = 4,//float and normal
    //    TumbleAndNormal = 5,//tumble and normal
    //    Attack = 6,//攻击
    //    Defense = 7,//防御
    //    Treat = 8,//加血
    //    AOE = 9,//AOE
    //}

    public enum AttackableType
    {
        None = 0,
        Normal = 1,
        Float = 2,//float only
        Tumble = 3,//tumble only
        FloatAndNormal = 4,//float and normal
        TumbleAndNormal = 5,//tumble and normal
        All = 6,//float tumble and normal
    }

    public enum AnimType
    {
        None = 0,
        Root = 1,//原地
        Trace = 2,//移动
        Run = 3,//跑步
    }

    public enum EffectType
    {
        None = 0,
        Root = 1,//原地
        Trace = 2,//移动
        TargetArea = 3,//指定区域
        LockPart = 4,//锁定角色部位
        LockTarget = 5,//锁定目标（也可以是骨骼）
        ChangeColor = 6,//变色
        ShakeScreen = 7,//振屏
        BlackScreen = 8,//黑屏
        CurveTrace = 9,//曲线移动
        MoveTargetPos = 10,//以移动技能终点位置为中心
        FullScreen = 11,//全屏特效
        BGEffect = 12,//背景特效
    }

    //技能目标类型
    public enum TargetType
    {
        None = 0,
        Ally = 1,//友军
        Enemy = 2,//敌军
    }

    //范围类型
    public enum RangeType
    {
        None = 0,
        CurrentSingle = 1,//当前目标单体
        CurrentRow = 2,//当前目标所在横行
        CurrentColumn = 3,//当前目标所在的列
        All = 4,//全体
        CurrentAndBehindFirst = 5,//当前所在目标以及后面1格（共2格）
        CurrentAndNearCross = 6,//当前目标以及相邻的十字型
        CurrentBehindFirst = 7,//当前目标后面第1格（共1格）
        CurrentBehindSecond = 8,//当前目标后面第2格（共1格）
        CurrentIntervalOne = 9,//当前目标横向间隔1格位置（上下2个点，最多打1人)
        CurrentAndRandomTwo = 10,//以当前目标为起点的随机闪电链，3人
        RandomN = 11,//全体目标中随机N个
        Cross = 12,//绝对位置，正中十字
        FirstColum = 13,//绝对位置，第一列
        SecondColum = 14,//绝对位置，第二列
        ThirdColum = 15,//绝对位置，第三列
        FirstRow = 16,//绝对位置，第一行
        SecondRow = 17,//绝对位置，第二行
        ThirdRow = 18,//绝对位置，第三行
        ExceptMidpoint = 19,//绝对位置，除中间1格的其它8格
        Midpoint = 20,//绝对位置，正中间1格
        LeadingDiagonal = 21,//绝对位置，上对角线(主对角线)
        SecondaryDiagonal = 22,//绝对位置，下对角线(次对角线)
        AllAbsolutely = 23,//绝对位置，全体
        Weakness = 24,//弱点攻击
        LowestHP = 25,//最低血量
        RandomSingle = 26,//随机单体
        CurrentAndBehindTowColum = 27,//当前目标及后两列
        BehindTowColum = 28,//固定后两列
        //RandomSingleFriendFromExcption = 29,//随机异常中的友军单体
    }

    //技能效果类型
    public enum MechanicsType
    {
        None = 0,
        Damage = 1,//伤害
        Treat = 3,//治疗
        Poisoning = 4,//中毒（持续伤害，数值随等级提升）
        LastTreat = 6,//持续治疗（数值随等级提升）
        IgnoreDefenseDamage = 7,//无视防御伤害百分比
        Swimmy = 8,//眩晕
        Float = 9,//浮空
        Tumble = 10,//倒地
        Invincible = 11,//无敌
        Silence = 12,//沉默
        Blind = 13,//致盲
        Frozen = 14,//冰冻（暂停动作)
        TreatPercent = 15,//恢复生命值(百分比，概率)
        Ignite = 16,//点燃（持续伤害，数值随等级提升）
        Bleed = 17,//流血（持续伤害，数值随等级提升）
        Sleep = 18,//睡眠（暂停动作)
        Landification = 19,//石化（暂停动作)
        Tieup = 20,//禁锢（暂停动作)
        Reborn = 21,//重生
        Disperse = 22,//驱散
        Immune = 23,//免疫特殊状态
        ReboundTime = 24,//反伤(时间随等级提升)
        ReboundValue = 25,//反伤(数值随等级提升)
        RandomMechanics = 26,//随机效果
        DrainDamage = 27,//伤害并吸血
        ImmunePhysicsAttack = 28,//免疫物理伤害
        ImmuneMagicAttack = 29,//免疫魔法伤害
        Tag = 30,//标记
        GeneralSkillHit = 31,//通用技能必定命中(次数)
        GeneralSkillCrit = 32,//通用技能必定暴击(次数)
        AccumulatorTag = 33,//蓄力
        ImmediatePercentDamage = 34,//直接百分比伤害
        SwimmyExtraDamage = 35,//对眩晕状态目标造成额外伤害
        LandificationExtraDamage = 36,//对石化状态目标造成额外伤害
        BleedExtraDamage = 37,//对流血状态目标造成额外伤害
        FrozenExtraDamage = 38,//对冰冻状态目标造成额外伤害
        PoisoningExtraDamage = 39,//对中毒状态目标造成额外伤害
        TagExtraDamage = 40,//对标记状态目标造成额外伤害
        IgniteExtraDamage = 41,//对点燃状态目标造成额外伤害
        ShieldTime = 103,//护盾(时间随等级提升)
        ShieldValue = 104,//护盾(数值随等级提升)
        DrainTime = 105,//吸血(时间随等级提升)
        DrainValue = 106,//吸血(数值随等级提升)
        ForceKill = 107,//即死
        PhysicsDefensePercentTime = 109,//物理防御百分比(时间随等级提升)
        PhysicsDefensePercentValue = 110,//物理防御百分比(数值随等级提升)
        MagicDefensePercentTime = 111,//魔法防御百分比(时间随等级提升)
        MagicDefensePercentValue = 112,//魔法防御百分比(数值随等级提升)
        PhysicsAttackPercentTime = 113,//物理攻击百分比(时间随等级提升)
        PhysicsAttackPercentValue = 114,//物理攻击百分比(数值随等级提升)
        MagicAttackPercentTime = 115,//魔法攻击百分比(时间随等级提升)
        MagicAttackPercentValue = 116,//魔法攻击百分比(数值随等级提升)
        HPLimitPercentTime = 117,//生命上限百分比(时间随等级提升)
        HPLimitPercentValue = 118,//生命上限百分比(数值随等级提升)
        SpeedPercentTime = 119,//行动力百分比(时间随等级提升)
        SpeedPercentValue = 120,//行动力百分比(数值随等级提升)
        PhysicsDefenseFixedTime = 121,//物理防御固定值(时间随等级提升)
        PhysicsDefenseFixedValue = 122,//物理防御固定值(数值随等级提升)
        MagicDefenseFixedTime = 123,//魔法防御固定值(时间随等级提升)
        MagicDefenseFixedValue = 124,//魔法防御固定值(数值随等级提升)
        PhysicsAttackFixedTime = 125,//物理攻击固定值(时间随等级提升)
        PhysicsAttackFixedValue = 126,//物理攻击固定值(数值随等级提升)
        MagicAttackFixedTime = 127,//魔法攻击固定值(时间随等级提升)
        MagicAttackFixedValue = 128,//魔法攻击固定值(数值随等级提升)
        HPLimitFixedTime = 129,//生命上限固定值(时间随等级提升)
        HPLimitFixedValue = 130,//生命上限固定值(数值随等级提升)
        SpeedFixedTime = 131,//行动力固定值(时间随等级提升)
        SpeedFixedValue = 132,//行动力固定值(数值随等级提升)
        HitTime = 133,//命中固定值(时间随等级提升)
        HitValue = 134,//命中固定值(数值随等级提升)
        DodgeTime = 135,//闪避固定值(时间随等级提升)
        DodgeValue = 136,//闪避固定值(数值随等级提升)
        CritTime = 137,//暴击固定值(时间随等级提升)
        CritValue = 138,//暴击固定值(数值随等级提升)
        AntiCritTime = 139,//防爆固定值(时间随等级提升)
        AntiCritValue = 140,//防爆固定值(数值随等级提升)
        BlockTime = 141,//格挡固定值(时间随等级提升)
        BlockValue = 142,//格挡固定值(数值随等级提升)
        AntiBlockTime = 143,//破击固定值(时间随等级提升)
        AntiBlockValue = 144,//破击固定值(数值随等级提升)
        CounterAtkTime = 145,//反击固定值(时间随等级提升)
        CounterAtkValue = 146,//反击固定值(数值随等级提升)
        CritHurtAddTime = 147,//暴击伤害加成固定值(时间随等级提升)
        CritHurtAddValue = 148,//暴击伤害加成固定值(数值随等级提升)
        CritHurtDecTime = 149,//暴击伤害减免固定值(时间随等级提升)
        CritHurtDecValue = 150,//暴击伤害减免固定值(数值随等级提升)
        ArmorTime = 151,//破甲固定值(时间随等级提升)
        ArmorValue = 152,//破甲固定值(数值随等级提升)
        DamageDecTime = 153,//伤害减免固定值(时间随等级提升)
        DamageDecValue = 154,//伤害减免固定值(数值随等级提升)
        DamageAddTime = 155,//伤害加成固定值(时间随等级提升)
        DamageAddValue = 156,//伤害加成固定值(数值随等级提升)
        PhysicsDamageAddTime = 157,//增加物理伤害(时间随等级提升)
        PhysicsDamageAddValue = 158,//增加物理伤害(数值随等级提升)
        MagicDamageAddTime = 159,//增加魔法伤害(时间随等级提升)
        MagicDamageAddValue = 160,//增加魔法伤害(数值随等级提升)
        Transform = 1001,//变换
    }

    public enum MechanicsValueType
    {
        None = 0,
        Time = 1,
        Probabiblity = 2,
        FixedValue = 3,
        PercentValue = 4,
        Extra = 5,//额外伤害
    }

    public enum SkillLevelBuffAddType
    {
        None = 0,
        Value = 1,//数值
        Time,//时间
        Count = 3,//次数
    }

    public enum BuffAddType
    {
        None = 0,
        Fixed = 1,//固定值
        Percent = 2,//百分比
    }

    public enum BuffType
    {
        None = 0,
        Swimmy = 1,//眩晕
        Invincible = 2,//无敌
        Silence = 3,//沉默
        Blind = 4,//致盲
        Float = 5,//浮空
        Tumble = 6,//倒地
        Poisoning = 7,//中毒
        Treat = 8,//持续治疗
        Speed = 9,//行动力(cd时间)       
        Shield = 10,//护盾
        Drain = 11,//吸血
        PhysicsDefense = 12,//物理防御
        MagicDefense = 13,//魔法防御        
        PhysicsAttack = 14,//物理攻击
        MagicAttack = 15,//魔法攻击
        HPLimit = 16,//生命上限
        Hit = 17,//命中
        Dodge = 18,//闪避
        Crit = 19,//暴击
        AntiCrit = 20,//防爆
        Block = 21,//格挡
        AntiBlock = 22,//破击
        CounterAtk = 23,//反击
        CritHurtAdd = 24,//暴击伤害加成
        CritHurtDec = 25,//暴击伤害减免
        Armor = 26,//破甲
        DamageDec = 27,//伤害减免
        DamageAdd = 28,//伤害增加
        Frozen = 29,//冰冻（暂停动作)
        TreatPercent = 30,//恢复百分比
        Ignite = 31,//点燃（持续伤害，数值随等级提升）
        Bleed = 32,//流血（持续伤害，数值随等级提升）
        Sleep = 33,//睡眠（暂停动作)
        Landification = 34,//石化（暂停动作)
        Tieup = 35,//禁锢（暂停动作)
        GeneralSkillPhysicsAttack = 36,//通用技能物理攻击(次数)
        GeneralSkillMagicAttack = 37,//通用技能魔法攻击(次数)
        TargetSkillPhysicsAttack = 38,//指定技能物理攻击(次数)
        TargetSkillMagicAttack = 39,//指定技能魔法攻击(次数)
        Immune = 40,//免疫特殊状态
        Rebound = 41,//反伤
        DamageImmuneTime = 42,//伤害免疫时间
        DamageImmuneCount = 43,//伤害免疫次数
        Weakness = 44,//弱点攻击
        TreatAdd = 45,//治疗效果增加
        ForceKill = 46,//即死
        ImmunePhysicsAttack = 47,//免疫物理伤害
        ImmuneMagicAttack = 48,//免疫魔法伤害
        Tag = 49,//标记
        GeneralSkillHit = 50,//通用技能必定命中(次数)
        GeneralSkillCrit = 51,//通用技能必定暴击(次数)
        AccumulatorTag = 52,//蓄力标记
    }

    public enum HeroTransformType
    {
        None = 0,
        ModelAndSkill = 1,//模型和技能
        AnimationAndSkill = 2,//动作和技能
        Scale = 3,//模型缩放
    }

    public enum BaseResType
    {
        None = 0,           //无
        Resource = 1,       //资源(暂时没用)
        Hero = 2,           //伙伴
        Equipment = 3,      //装备
        Item = 4,           //道具
        Hero_Exp = 5,       //伙伴经验
        //        Player_Exp = 6,     //主角经验 is legacy, 暂时不用
        Account_Exp = 7,    //账号经验
        Gold = 8,           //金币
        Diamond = 9,        //钻石
        Crystal = 10,       //水晶
        PveAction = 11,     //pve行动点数
        TowerAction = 12,   //千层塔行动点数
        PvpAction = 13,     //竞技场行动点数
        Honor = 14,         //荣誉
        ArenaPoint = 15,    //积分赛积分
        ExpeditionPoint = 16,//远征币(类似荣誉)
        WorldBossResource = 17, //世界Boss资源
        FromationTrainPoint = 18, //阵型培养点
        RMB = 100,          //人民币

    }
    public enum EquipmentStrengthenSortType
    {
        All = 0,		//全部
        Weapon = 1,		//武器
        Armor = 2,		//防具
        Accessory = 3,	//饰品
    }
    public enum EquipmentType
    {

        //
        None = 0,         		  //无效值
        PhysicalWeapon = 1,       //物理攻击武器
        MagicalWeapon = 4,        //魔法攻击武器
        Armor = 2,                //防具
        Accessory = 3,            //饰品
    }

    public enum EquipmentWearOffType
    {
        Off = 0,    //脱
        Wear = 1,   //穿
    }

    public enum FormationPosition
    {
        // Invalid position.
        Invalid_Position = 0,
        // Player positions.
        Player_Position_1 = 1,
        Player_Position_2 = 2,
        Player_Position_3 = 3,
        Player_Position_4 = 4,
        Player_Position_5 = 5,
        Player_Position_6 = 6,
        Player_Position_7 = 7,
        Player_Position_8 = 8,
        Player_Position_9 = 9,
        // Enemy positions.
        Enemy_Position_1 = 101,
        Enemy_Position_2 = 102,
        Enemy_Position_3 = 103,
        Enemy_Position_4 = 104,
        Enemy_Position_5 = 105,
        Enemy_Position_6 = 106,
        Enemy_Position_7 = 107,
        Enemy_Position_8 = 108,
        Enemy_Position_9 = 109,
    }

    public enum GenderType
    {
        Male = 1,     //男性
        Female = 2,   //女性
        Child = 3,    //小孩
    }

    public enum RoleType
    {
        Invalid = 0,       //无效值
        Defence = 1,       //防御型
        Offence = 2,       //攻击型
        Mage = 3,          //魔法型
        Support = 4,       //辅助型
        Mighty = 5,        //全能型
    }

    public enum RoleAttackAttributeType  //角色攻击属性类型
    {
        Invalid = 0,          //无效值
        PhysicalAttack = 1,   //物理攻击
        MagicalAttack = 2,    //魔法攻击
    }

    public enum RoleQuality
    {
        White = 1,      //白
        Green = 2,      //绿
        Blue = 3,       //蓝
        Purple = 4,     //紫
        Orange = 5,     //橙
        Red = 6,        //红
    }

    public enum ItemQuality
    {
        White = 1,      //白
        Green = 2,      //绿
        Blue = 3,       //蓝
        Purple = 4,     //紫
        Orange = 5,     //橙
        Red = 6,        //红
    }

    public enum RoleStrengthenStage
    {
        White = 0,       //白
        Green = 1,       //绿
        Blue = 2,        //蓝
        Purple = 3,      //紫
        Orange = 4,      //橙
    }

    public enum BagType
    {
        HeroBag = 1,           //英雄背包     //与服务端对应(1为英雄背包, 2为装备背包)
        EquipmentBag = 2,      //装备背包
    }

    public enum HeroSortType
    {
        Invalid = 0,       //无效值
        QualityAsc = 1,    //按品质升序
        QualityDesc = 2,   //按品质降序
    }

    //    public enum BuyHeroType
    //    {
    //        HonorSingle = 1,      //荣誉一抽
    //        DiamondSingle = 2,    //钻石一抽
    //        DiamondTen = 3,       //钻石十抽
    //    }
    //
    //    public enum BuyHeroCostType
    //    {
    //        Free = 0,       //免费
    //        Honor = 1,      //荣誉点
    //        Diamond = 2,    //钻石
    //    }
    //
    //    public enum BuyEquipmentType
    //    {
    //        HonorSingle = 1,         //荣誉一抽
    //        DiamondSingle = 2,       //钻石一抽
    //    }
    //
    //    public enum BuyEquipmentCostType
    //    {
    //        Free = 0,                 //免费
    //        Honor = 1,                //荣誉
    //        Diamond = 2,              //钻石
    //    }

    //英雄合成
    public enum CombinePosition
    {
        Invalide_Position,
        Combine_Position_1,
        Combine_Position_2,
        Combine_Position_3,
        Combine_Position_4,
        Combine_Max,

    }

    public enum DungeonType
    {
        Invalid = 0,                //无效值
        Easy = 1,                   //简单副本
        Normal = 2,                 //普通副本
        Hard = 3,                   //困难副本
        ActivityNormal = 5,         //活动普通副本
        ActivityDamageOutput = 6,   //活动伤害输出副本
        WorldTree = 7,              //世界树副本
        ActivitySingle = 8,         //活动单体技能副本
        ActivityRange = 9,          //活动范围技能副本
        ActivityMirror = 10,        //活动镜像副本
        ActitityFloat = 11,         //活动浮空伤害副本
        BossSubspecies = 20,         //Boss亚种
    }

    public enum DungeonBossType
    {
        Normal = 0,
        Elite = 1,
        Boss = 2,
    }

    public enum DailyDungeonDifficulty
    {
        Easy = 1,                  // 简单难度
        Normal = 2,                // 普通难度
        Hard = 3,                  // 困难难度
        Hell = 4,                  // 地狱难度
    }

    //功能开放
    public enum FunctionOpenType
    {
        Invalide = 0,
        MainView = 1,				// 主界面
        MainView_Task = 100,        // @[主界面 任务按钮]
        MainView_Mail = 200,		// @[邮件]
        MainView_SevenHilarity = 2600,		// @[邮件]
        MainView_Friend = 300,		// @[好友]
        MainView_SignIn = 400,		// @[签到]
        MainView_Activity = 500,    //@[主界面 活动]
        MainView_Ranking = 600,     //@[主界面 排行榜]

        MainView_Player = 700,		//@[主界面-主角按钮 是否可见]
        // Player_Summon_Beast = 31,	//主角-召唤兽
        Player_Change_Profession = 701,			//@[主角-转职]
        PlayerWearEquip = 709,		//@[主角-穿戴]
        PlayerStrengthen = 703,     //@使魔强化]
        PlayerTraining = 705,     	//@使魔培养]
        PlayerBreakthrough = 707,    //@使魔突破]

        MainView_Hero = 800,		// @[主界面-英雄按钮]
        HeroStrengthen = 801,		// @[英雄强化]
        HeroAdvance = 803,			// @[英雄升星]
        HeroBreakthrough = 805,		// @[英雄突破]

        MainView_Formation = 900,	   		// @[阵型]
        FormationTraining = 901,     		// @[更换阵型]

        HeroSecondTeam = 905,		// @[英雄2队]
        HeroThirdTeam = 906,	    // @[英雄3队]

        MainView_Equipment = 1000,	// @[主界面-装备按钮 是否可见]
        EquipTraining = 1001,		//@[装备培养]
        EquipAdvance = 1012,		//@[装备进阶]

        EquipRecast = 1004,     	//@[装备重铸]
        EquipEnchant = 1006,     	//@[装备附魔]
        EquipUpStar = 1008,     	//@[装备升星]
        EquipGemInsert = 1010,     	//@[装备镶嵌]

        MainView_Shop = 1100,		// @[主界面-商店按钮 是否可见]

        MainView_BlackMarket = 1200,	   // @黑市
        BlackMarket_Arena = 1202,     		// @黑市 竞技场黑市
        BlackMarket_Expedition = 1203,     // @黑市 远征黑市
        BlackMarket_WorldBoss = 1204,     // @黑市 世界boss黑市
        MainView_Consortia = 1400,     	// @主界面 公会按钮
        MainView_illustrate = 1300,        // @[图鉴]
        MainView_Explore = 1500,      	// @主界面 探险按钮

        MainView_WorldBoss = 1900,         // @[世界Boss]
        WorldBoss_inspireBtn = 1902,       // @[世界Boss鼓舞]

        MainView_FightCenter = 1600,    // @主界面 战斗中心按钮
        FightCenter_Expedition = 1606,	   // @远征
        FightCenter_Arena = 1601,		   // @[PVP竞技场]
        FightCenter_WorldTree = 1604,      // @世界树
        FightCenter_PvpRace = 1608,			//@竞技场积分赛
        FightCenter_MineBattle = 1610,			//@矿战

        MainView_DailyDungeon = 1700,	   // @[每日副本开启等级]

        MainView_PVE = 1800,	         //@[主界面-副本按钮]
        PVE_Normal = 1801,               //@[副本-难度2]
        PVE_Hard = 1802,                 //@[副本-难度3]
        SingleSweep = 1803,			 //@[副本-扫荡]
        DoubleSpeed = 1807,	     		//@[副本-2倍数]
        AutoFight = 1808,		     	//@[副本-自动战斗]
        TenSweep = 1805,			 	//@[副本-扫荡10]
        FightPause = 1809,           //战斗暂停


        MainView_Chat = 2000,			// @[聊天]
        MainView_Pack = 2100,			//背包

        Boss_List_View = 2200,          // Boss列表界面
        MainView_dailyTask = 2300,          // --每日任务
        MainView_compose = 2400,			//合成
        /* 客户端自定义 */
        Setting = 80,				//设置
        Shop_Hero = 88811,				//商店-英雄
        Shop_Equipment = 88812,		//商店-装备
        Shop_Diamond = 88813,			//商店-钻石
        Shop_Action = 88814,           //商店-行动力
        Shop_Gold = 88815,             //商店-金币
        Shop_Other = 88816,            //商店-其它

        RoleInfoView = 8881000,		//英雄培养界面
        PveEmbattleView = 8881001,		//pve阵型界面
        Equipment_View = 8881002,		//装备界面
        Dungeon_Detail_View = 8881003,	//副本-详情
        Dungeon_SelectChapter_View = 8881004,//副本-章节
        Dungeon_Daily_Detail = 8881005,//每日副本详细信息界面
        MultpleFight_View = 81006, 	//pvp、竞技场综合界面
        Shop_View = 8881007,			//商店
        PlayerInfoView = 8881008,		//主角信息界面
        FightCenter_PvpRaceMatch = 16081,			//@竞技场积分赛匹配界面
        MainView_illustrate_hero_detial_view = 13004,//英雄图鉴详情界面
        /* 客户端自定义 */
    }

    public enum ShadowType
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
    }

    public enum RoleAttributeValueType
    {
        RealValue,   //真实值
        Percent,     //百分比
    }

    public enum EquipmentAttributeType
    {
        HP = 0,                 //生命力
        NormalAtk = 1,          //物理攻击
        MagicAtk = 2,           //魔法攻击
        Def = 3,                //防御
        Speed = 4,              //行动力
        Hit = 5,                //命中
        Dodge = 6,              //闪避
        Crit = 7,               //暴击
        AntiCrit = 8,           //抗暴击
        Block = 9,              //格挡
        AntiBlock = 10,         //破击(破格挡)
        CounterAtk = 11,        //反击
        CritHurtAdd = 12,       //暴击伤害
        CritHurtDec = 13,       //暴击伤害减免
        Armor = 14,             //破甲
        DamageAdd = 15,        //伤害减免
        DamageDec = 16,         //伤害加成
        HPPercent = 17,         //生命百分比
        NormalAtkPercent = 18,  //物理攻击百分比
        MagicAtkPercent = 19,   //魔法攻击百分比
        DefPercent = 20         //防御百分比(双防)
    }
    public enum RoleAttributeType     //角色属性类型定义(主角与英雄共用)
    {
        HP = 0,                 //生命力
        NormalAtk = 1,          //物理攻击
        MagicAtk = 2,           //魔法攻击
        Normal_Def = 3,         //物理防御
        Speed = 4,              //行动力
        Hit = 5,                //命中
        Dodge = 6,              //闪避
        Crit = 7,               //暴击
        AntiCrit = 8,           //抗暴击
        Block = 9,              //格挡
        AntiBlock = 10,         //破击(破格挡)
        CounterAtk = 11,        //反击
        CritHurtAdd = 12,       //暴击伤害
        CritHurtDec = 13,       //暴击伤害减免
        Armor = 14,             //破甲
        DamageAdd = 15,        //伤害减免
        DamageDec = 16,         //伤害加成
        //		HPPercent = 17,         //生命百分比
        //		NormalAtkPercent = 18,  //物理攻击百分比
        //		MagicAtkPercent = 19,   //魔法攻击百分比
        //		Normal_DefPercent = 20, //物理防御百分比
        //		Magic_DefPercent = 21,	//魔法防御百分比
        MAX,
    }

    public enum FightResultQuitType
    {
        Fight_Again,		//再来一次直接打
        Fight_Again_Map,	//再来一次要布阵
        Fight_Next_Dungeon,	//下一关
        Go_Map,				//地图
        Go_Hero,			//英雄
        Go_Player,			//主角
        Go_Equip,			//装备
        Go_Shop,			//商店
        Go_Pvp,				//pvp
        Go_MainView,		//mainview
        Go_Formation,		//阵型
        Fight_Activity_again, //再次攻打每日副本
        Go_Activity,        //回到每日飞奔
        Go_Expedition,		//远征
        Go_WorldTree,       //回到世界树
        GO_PVP_RACE,        //pvp积分赛
        GO_FRIEND,          //好友
        GO_MINE,            //矿战
        GO_Boss_List,       //Boss列表
    }
    public enum TaskBigType
    {
        Task_Invalide = 0,
        Task_achievement = 1,
        Task_daliy = 2,
        Task_main = 3,
        Task_Max,
    }
    public enum TaskType
    {
        Invalid = 0,                      //无效值
        AccountLevelRequirement = 1,      //账号等级 等级要求 0
        PassDungeonTimes = 2,             //通关副本 副本ID   次数
        //OwnHero = 3,                    //拥有英雄 英雄ID(4项组合) 英雄数量     //暂时不使用次枚举
        ProfessionIDAndLevel = 4,         //职业等级 职业ID 等级
        VIPLevelRequirement = 5,          //VIP等级 等级
        MultipleHeroLevelRequirement = 6, //多个高等级英雄 英雄数量 英雄等级
        MultipleHeroStarRequirement = 7,  //多个高星级英雄 英雄数量 英雄星级
        OwnResouce = 8,                   //拥有资源 资源ID(4项组合)
        FirstTopUpInTime = 9,             //指定时间首充奖励 时间
        SingleTopUp = 10,                 //单次充值总额 充值数值
        accumulateTopUp = 50,			//累计充值总额	充值数值
        thousandTower = 11,				//千层塔达到层数
        heroBreakThrough = 12,				//伙伴突破次数
        MultipleEquipmentStarRequirement = 13,//多个高星级装备  装备数量，装备星级
        accumulateCostDiamind = 14,				//累计消耗钻石  钻石数量
        singleUseHonor = 15,					//一次性消费荣誉  荣誉数量
        accumulateOwnGold = 16,					//累计拥有金币  金币数量
        accumulateTenLottery = 17,				//累计十连抽 次数
        assignDungeonStar = 18,				//指定副本星级
        assignDungeonDifficultyTotalStar = 19,	//指定难度总星星数
        accumulatePvpVictoryCount = 20,			//竞技场累计胜利次数
        accumulatePvpFightCount = 21,			//竞技场累计战斗次数
        promoteAccountLevel = 23,				//提升账号等级
        strengthenEquipCount = 24,				//强化装备次数
        boostNewDungeonCount = 25,				//推进新副本次数
        dailyLogin = 26,						//每日登陆
        costPveAction = 27,						//消耗体力
        dailyDungeonFight = 28,					//参与每日副本
        pvpFightCount = 29,						//竞技场战斗次数
        TenLotteryCount = 30,					//十连抽次数
        thousandTowerFight = 31,				//参与千层塔
        clearDungeon = 32,						//通关某个副本

    }
    public enum RedPointType
    {
        None = 0,
        RedPoint_TaskAchievement = 1,
        RedPoint_TaskDaily = 2,
        RedPoint_TaskMain = 3,
        RedPoint_TaskAll = 4,
        RedPoint_Mail = 5,
        RedPoint_Chat = 6,
        RedPoint_FriendAll = 7,
        RedPoint_MyFriend = 8,
        RedPoint_FriendRequest = 9,
        RedPoint_Formation_specific = 10,	//具体阵型上显示
        RedPoint_Formation = 11,		//阵型二级界面提示
        RedPoint_Hero_New = 12,//获得新英雄提示
        RedPoint_VIP_Benefit = 13,   //VIP福利
        RedPoint_Shop_Has_Free_Item = 14,   //商店中有冷却好的免费抽卡
        RedPoint_Activity = 15,   //可领取活动
        RedPoint_Consortia_apply = 16,   //公会申请提示
        RedPoint_Explore_Has_Unhandle_Task = 17,  //探险未领取或未确认失败
        RedPoint_Have_Not_Challenge_Any_Daily_Dungeon_Today = 18,  //今日未挑战每日副本
        RedPoint_Seven_Hilarity = 2600,  //七日狂欢
    }
    public enum ExpeditionDungeonType
    {
        Expedition_Normal = 1,
        Expedition_SmallTreasureBox = 2,
        Expedition_BigTreasureBox = 3,
    }
    public enum BlackMarketType
    {
        BlackMarket_Min = 0,
        BlackMarket_Hero = 1,
        BlackMarket_Equip = 2,
        BlackMarket_Item = 3,
        BlackMarket_LimitActivity = 4,
        BlackMarket_Activity = 5,
        BlackMarket_Max = 6,
    }
    public enum BlackMarketLimitType
    {
        BlackMarketLimit_None = 0,
        BlackMarketLimit_Person = 1,
        BlackMarketLimit_Server = 2,//全服
    }
    public enum ChatType
    {
        World = 1,		//世界聊天
        Private = 2,	//私人聊天
        System = 3,		//系统公告
    }
    public enum ChatContentType
    {
        Text = 0,		//纯文本
        LinkEquip = 1,	//链接装备
        Pic = 2,		//图片
        animation = 3,	//动画
    }

    public enum WorldTreeDungeonStatus
    {
        Locked = 0,    //未解锁
        Unlocked = 1,  //可挑战
        Passed = 2,    //已通关
    }
    //战斗中心
    public enum MultipleFightCenterType
    {
        FightCenter_Pvp = 1,
        FightCenter_Expetition = 2,
        FightCenter_WorldTree = 3,
        FightCenter_WorldBoss = 4,//世界boss
        FightCenter_PvpRace = 5,//竞技场积分赛
        FightCenter_MineBattle = 6,//矿战
    }
    //物品路径跳转
    public enum GoodsJumpType
    {
        None = 0,
        Jump_Dungeon = 1,		//副本掉落（多个副本id）
        Jump_WorldTree = 2,		//世界树 功能页面
        Jump_Expedition = 3,	//远征 功能页面
        Jump_DailyDungeon = 4,	//每日副本 类型+页面
        Jump_Pvp = 5,			//、PVP获得 功能页面
        Jump_Shop = 6,			//、商店购买 对应页签
        Jump_Sign = 7,			//、签到获得 功能页面
        Jump_BlackMarket = 8,	//、黑市获得 对应页签
        Jump_TakeCard = 9,		//、商店抽卡获得 对应页签
        Jump_Explore = 10,     // 探险
        Jump_ConsortiaShop = 11,     // 公会商店
        Jump_Task = 12,       //任务
    }
    public enum ITEM_TYPE
    {
        Material = 1,          // 材料
        Sweep = 2,             // 扫荡卷
        ExpPotion = 3,         // 经验药水
        HeroExp = 5,           // 英雄经验
        AccountExp = 7,        // 经验值
        Gold = 8,              // 金币
        Diamomd = 9,           // 钻石
        Crystal = 10,          // 水晶
        PveAction = 11,        // 行动力
        Honor = 14,            // 荣誉
        ExpeditionPoint = 16,  // 远征币
        PetMaterial = 20,      // 使魔材料
        Gem = 21,              // 宝石
        Enchant = 22,          // 附魔卷轴
        StarLevelUpGem = 23,   // 升星宝石
        HeroPiece = 24,        // 英雄碎片
        RandomGift = 25,       // 随机礼包
        EquipPiece = 26,  //装备碎片
    }
    public enum ITEM_ID
    {
        bigExpMedicine = 10602,		//大经验药水
        midExpMedicine = 10601,		//中经验药水
        smallExpMedicine = 10600,	//小经验药水
    }

    public enum DialogNPCSide
    {
        Left = 1,
        Right = 2,
    }

    public enum FormationState
    {
        Locked = 1,
        NotInUse = 2,
        InUse = 3,
    }
    public enum FormationEffectType
    {
        PhysicAtk = 1,		// 物理攻击%
        MagicAtk = 2,		// 魔法攻击%
        PhysicDef = 3,		// 物理防御%
        MagicDef = 4,		// 魔法防御%
        Hit = 5,			// 命中概率%
        Dodge = 6,			// 闪避概率%
        Crit = 7,			// 暴击概率%
        AntiCrit = 8,       // 防爆概率%
        Block = 9,			// 格挡概率%
        AntiBlock = 10,     // 破击概率%
        CritHurtAdd = 11,   // 暴击伤害加成%
        CritHurtDec = 12,   // 暴击伤害减免%
        DamageAdd = 13,     // 伤害加成%
        DamageDec = 14,     // 伤害减免%
        Weakness = 15,      // 弱点攻击%
        HPLimit = 16,         // 血量上限增加%
        TreatPercent = 17,	//  每一定时间回血比例%
        TreatAdd = 18,      // 治疗效果增加%
    }
    public enum FormationTeamType
    {
        pveFirstTeam = 101,
        pveSecondTeam = 102,
        pveThirdTeam = 103,
        pvpTeam = 201,
        expeditionTeam = 301,
    }

    public enum FormationAttrType
    {
        None = 0,
        Base = 1,
        Addition = 2,
    }

    //查看详情
    public enum ShowDescriptionType
    {
        click = 1,	//点击显示
        longPress = 2,//长按显示
    }
    public enum ServerState
    {
        None = 0,
        Full = 1,		//爆满
        New = 2,		//新区
        Recommend = 3,  //推荐
        Normal = 4,     //流畅
        Maintain = 5,	//维护
    }
    //消费提示
    public enum ConsumeTipType
    {
        None = 0,
        DiamondDrawSingleHero = 1001,	//钻石单抽英雄卡
        DiamondDrawTenHeroes = 1002,		//钻石十抽英雄卡
        HonorDrawSingleHero = 1003,		//荣誉单抽英雄卡

        DiamondBuyPveAction = 1014,			//钻石购买体力值

        DiamondBuyWorldTreeCount = 1004,	//钻石购买世界树果实
        DiamondBuyPvpCount = 1005,			//钻石购买竞技场次数
        DiamondBuyCoin = 1006,				//钻石购买金币
        DiamondBuyBlackmarket = 1007,		//钻石购买黑市道具
        DiamondBuyFormationTrainPoint = 1008,//钻石购买阵型培养点数(完)
        DiamondBuyWorldBossInspire = 1009,	//钻石世界boss鼓舞(完)
        DiamondDrawMutipleReward = 1010,    //每日副本领取多倍奖励提示
        DiamondResetExpeditionCount = 1011,	//钻石重置远征次数(完)
        DiamondBuyEquipGrid = 1012,			//钻石购买装备格子(完)
        DiamondBuyHeroGrid = 1013,			//钻石购买英雄格子(完)

        GoldDrawSingleArticle = 1015,			//金币单抽物品
        HonorDrawSingleArticle = 1016,			//荣誉单抽物品
        GoldDrawTenArticles = 1017,			    //金币十抽物品
        WorldBossRevive = 1018,             //世界Boss复活
    }
    public enum PlayerSkillTalentType
    {
        PassiveNormal = 0,
        PassiveThreeChoiceOne = 1,
        SummonThreeChoiceOne = 2,
    }

    public enum SkillHeadViewShowType
    {
        Left = 0,
        Right = 1,
    }
    public enum ButtonSoundType
    {
        NormalClick = 0,
        Toggle = 1,
        SkillClick = 2,
        Select = 3,
        ChatSend = 4,
    }
    public enum EquipTrainingType
    {
        LevelUp = 1,
        Advance = 2,
        Recast = 3,
    }

    public enum RoleEquipPos
    {
        All = 0,
        Weapon = 1,
        Armor = 2,
        Accessory = 3,
    }
    public enum PlatformType
    {
        all = -1,
        none = 0,
        shunwang = 1,
        uc = 10,
        baidu = 12,
        qihoo360 = 16,
        xiaomi = 17,
        m4399 = 18,
        oppo = 19,
        vivo = 20,
        lenovo = 23,
        huawei = 24,
        meizu = 31,
        coolpai = 32,
        jinli = 34,
        yingyongbao = 51,
        dowan = 85,
        iOS = 99,
    }

    public enum ExtraDataType
    {
        None = 0,
        SelectServer = 1,//选服
        CreateRole = 2,//创角
        EntryGame = 3,//进入游戏
        Levelup = 4,//升级
        QuitGame = 5,//退出游戏
    }

    //服务器地址类型
    public enum ServerAddressType
    {
        LoginVerify = 100,//登陆验证服务器地址
        ServerList = 200,//获取我方服务器列表
    }

    //关卡扫荡类型
    public enum SweepType
    {
        Single = 1,   //扫荡一次
        Ten = 2,      //连续扫荡十次
    }
    public enum PlayAudioType
    {
        none = 0,//无限制，允许有重复的声音
        SingleInGroup = 1,//小组中只播放一个（注:正在播放不重置）
    }
    //播声音界面类型
    public enum AudioViewType
    {
        mainView = 1,//--主界面英雄
        taskView = 2,// --任务界面
        shopView = 3,//-- 商店
        tutorialView = 4,// -- 新手引导
    }


}