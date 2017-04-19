using UnityEngine;
using UnityEditor;
using Logic.Character;
using Logic.Skill.Model;
using Logic.Enums;
using Logic.Net.Controller;
using Logic.Fight.Controller;
using System.Collections.Generic;
using Logic.Effect.Model;
using Common.Util;
using System.Text;
namespace Logic.Fight.Editor
{
    [ExecuteInEditMode]
    public class FightEditorWindow : UnityEditor.EditorWindow
    {
        private static CharacterEntity character = null;
        private static PosIndex posIndex = PosIndex.None;
        #region hit attribute
        private static float hitCD = 0, hitLength = 0, hitMoveTime = 0, hitOffset = 0,
            hitBackTime = 0, endShowTime = 0, hitPauseTime = 0, hitTime = 0;
        private static string effectsID, hitFlyEffectsID, timeline, machanicsIds, hitAudioName, getHitEffectId, getHitAudioName, getHitSkills;
        private static AnimType animType;
        #endregion
        private SkillType hitSkillType = SkillType.None;
        private static string hitGetHitColor;
        private bool hitFoldout = true, skill1Foldout = true, skill2Foldout = true, hitGetHitFoldout = false, skill1GetHitFoldout = false, skill2GetHitFoldout = false;
        private bool hitEffectsFoldout = true, skill1EffectsFoldout = true, skill2EffectsFoldout = true, hitFlyEffectFoldout = true, skill1FlyEffectFoldout = true, skill2FlyEffectFoldout = true;

        //skill
        private AttackableType attackableType;
        private string aoeEffectsID, aoeFlyEffectsID, audioIds;
        private bool hitAoeEffectFoldout = true, skill1AoeEffectFoldout = true, skill2AoeEffectFoldout = true, closeup = false;
        private float mechanicsA, mechanicsB, mechanicsC, bootTime, closeupOverTime, fullScreenTime, fullScreenLength, blackTime, blackLength, cameraTime, cameraLength;
        private float skillAudioDelay;
        private int fullScreen, blackScreen, bgEffect;


        //effect
        private EffectType effectType = EffectType.None;
        private string effectOffset, effectEndoffset, effectTargetPos, partName, curvePoint1, curvePoint2, curvePoint3;
        private float effectDelay = 0, effectLength = 0, effectMoveTime = 0;
        private string randomAngle, colorStr, scaleStr, rotateStr;
        private float curvePoint1EulerAngles, curvePoint2EulerAngles, curvePoint3EulerAngles;
        private bool effectIsRotate, ignoreTimeScale;

        //mechanics
        private TargetType targetType;
        private RangeType rangeType;
        private MechanicsType mechanicsType;
        private bool needTarget;
        private uint maxLayer;
        private float mechanicsAudioDelay;
        private string mechanicsEffectIds;
        private int mechanicsAudioType;

        //enum string
        private string[] targets = new string[] { string.Empty, "我方", "敌方" };
        private string[] skillTypes = new string[] { string.Empty, "普通攻击", "技能", "召唤兽" };
        private string[] animTypes = new string[] { string.Empty, "原地", "移动", "跑步" };
        private string[] attackableTypes = new string[] { string.Empty, "正常攻击", "只能攻击浮空", "只能攻击倒地", "浮空和正常攻击", "倒地和正常攻击", "全部" };
        private string[] rangeTypes = new string[]{string.Empty,"当前目标单体","当前目标所在横行","当前目标所在的列","全体","当前所在目标以及后面1格（共2格）",
            "当前目标以及相邻的十字型","当前目标后面第1格（共1格）","//当前目标后面第2格（共1格）","当前目标横向间隔1格位置（上下2个点，最多打1人)",
            "以当前目标为起点的随机闪电链，3人","全体目标中随机N个","绝对位置，正中十字","绝对位置，第一列","绝对位置，第二列","绝对位置，第三列","绝对位置，第一行",
            "绝对位置，第二行","绝对位置，第三行","绝对位置，除中间1格的其它8格","绝对位置，正中间1格","绝对位置，上对角线(主对角线)","绝对位置，下对角线(次对角线)",
            "绝对位置，全体","弱点攻击","最低血量","随机单体","当前目标及后两列","固定后两列"};
        private string[] effectTypes = new string[] { string.Empty, "原地", "移动", "指定区域", "锁定角色部位", "锁定目标", "变色", "振屏", "黑屏", "曲线移动", "移动目标点为中心", "全屏特效" };
        private string[] mechanicsTypes = new string[] {"伤害","治疗","中毒（持续伤害，数值随等级提升）","持续治疗（数值随等级提升）","无视防御伤害百分比","眩晕",
            "浮空","倒地","无敌","沉默","致盲","冰冻","恢复生命值(百分比)","点燃（持续伤害，数值随等级提升）","流血（持续伤害，数值随等级提升）","睡眠","石化","禁锢","复活","驱散",
			"免疫特殊状态","反伤(时间随等级提升)","反伤(数值随等级提升)","随机效果","伤害并吸血效果","免疫物理攻击","免疫魔法攻击","标记","必定命中","必定暴击","蓄力标记","直接伤害百分比效果",
            "对眩晕状态目标造成额外伤害","对石化状态目标造成额外伤害","对流血状态目标造成额外伤害","对冰冻状态目标造成额外伤害","对中毒状态目标造成额外伤害","对标记状态目标造成额外伤害",
            "对点燃状态目标造成额外伤害",
            "护盾(时间随等级提升)","护盾(数值随等级提升)","吸血(时间随等级提升)","吸血(数值随等级提升)","即死","物理防御百分比(时间随等级提升)",
            "物理防御百分比(数值随等级提升)","魔法防御百分比(时间随等级提升)","魔法防御百分比(数值随等级提升)","物理攻击百分比(时间随等级提升)","物理攻击百分比(数值随等级提升)",
            "魔法攻击百分比(时间随等级提升)","魔法攻击百分比(数值随等级提升)","生命上限百分比(时间随等级提升)","生命上限百分比(数值随等级提升)","行动力百分比(时间随等级提升)",
            "行动力百分比(数值随等级提升)","物理防御固定值(时间随等级提升)","物理防御固定值(数值随等级提升)","魔法防御固定值(时间随等级提升)","魔法防御固定值(数值随等级提升)",
            "物理攻击固定值(时间随等级提升)","物理攻击固定值(数值随等级提升)","魔法攻击固定值(时间随等级提升)","魔法攻击固定值(数值随等级提升)","生命上限固定值(时间随等级提升)",
            "生命上限固定值(数值随等级提升)","行动力固定值(时间随等级提升)","行动力固定值(数值随等级提升)","命中固定值(时间随等级提升)","命中固定值(数值随等级提升)",
            "闪避固定值(时间随等级提升)","闪避固定值(数值随等级提升)","暴击固定值(时间随等级提升)","暴击固定值(数值随等级提升)","防爆固定值(时间随等级提升)",
            "防爆固定值(数值随等级提升)","格挡固定值(时间随等级提升)","格挡固定值(数值随等级提升)","破击固定值(时间随等级提升)","破击固定值(数值随等级提升)",
            "反击固定值(时间随等级提升)","反击固定值(数值随等级提升)","暴击伤害加成固定值(时间随等级提升)","暴击伤害加成固定值(数值随等级提升)","暴击伤害减免固定值(时间随等级提升)",
            "暴击伤害减免固定值(数值随等级提升)","破甲固定值(时间随等级提升)","破甲固定值(数值随等级提升)","伤害减免固定值(时间随等级提升)","伤害减免固定值(数值随等级提升)",
            "伤害加成固定值(时间随等级提升)","伤害加成固定值(数值随等级提升)" ,"变换"};

        private static List<MechanicsType> mechanicsTypeIndexs;
        //animation
        private float runSpeed;
        Vector2 hitScrollPos = Vector2.zero, skill1ScrollPos = Vector2.zero, skill2ScrollPos = Vector2.zero;
        private bool animationIsRotate;

        private float timeScale = 1f;
        private bool timeScaleEnable = false;

        //buff
        BuffType buffType = BuffType.None;
        BuffAddType buffAddType = BuffAddType.Fixed;
        SkillLevelBuffAddType skillLevelBuffAddType = SkillLevelBuffAddType.Time;
        private string[] buffTypes = new string[] { string.Empty, "眩晕", "无敌", "沉默", "致盲", "浮空", 
            "倒地", "中毒", "持续治疗", "行动力", "护盾", "吸血", "物理防御", "魔法防御", "物理攻击", "魔法攻击", "生命上限", "命中",
            "闪避", "暴击", "防爆", "格挡", "破击", "反击", "暴击伤害加成" , "暴击伤害减免", "破甲", "伤害减免" , "伤害增加", "冰冻", 
            "恢复百分比" , "点燃", "流血", "睡眠", "石化", "禁锢" ,"复活","驱散",
            "免疫特殊状态","反伤(时间随等级提升)","反伤(数值随等级提升)"};
        float buffTime = 1, buffValue = 1;

        [MenuItem("CEngine/技能编辑器 %w", false, 100)]
        public static void OpenFightEditorWindow()
        {
            EditorWindow.GetWindow<FightEditorWindow>(true, "战斗编辑器");
            character = GetSelectedCharacterEntity();
            mechanicsTypeIndexs = new List<MechanicsType>();
            foreach (MechanicsType m in System.Enum.GetValues(typeof(MechanicsType)))
            {
                if (m == MechanicsType.None) continue;
                mechanicsTypeIndexs.Add(m);
            }
        }

        void OnDestroy()
        {
            if (VirtualServerController.instance)
                VirtualServerController.instance.fightEidtor = false;
            if (mechanicsTypeIndexs != null)
            {
                mechanicsTypeIndexs.Clear();
                mechanicsTypeIndexs = null;
            }
        }

        void OnLostFocus()
        {
            if (!Application.isPlaying)
                this.Close();
            Time.timeScale = timeScale;
        }

        void OnHierarchyChange()
        {
            if (!Application.isPlaying)
                this.Close();
        }

        private static CharacterEntity GetSelectedCharacterEntity()
        {
            GameObject[] gos = Selection.gameObjects;
            CharacterEntity result = null;
            if (gos.Length == 0 || gos.Length > 1)
            {
                Debugger.Log("only u can select one gameObject !");
                return null;
            }
            result = gos[0].GetComponent<CharacterEntity>();
            if (!result)
                result = gos[0].GetComponentInParent<CharacterEntity>();
            return result;
        }

        void OnGUI()
        {
            if (!Application.isPlaying)
                this.Close();
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("战斗编辑器");
            if (GUILayout.Button("停止战斗", GUILayout.Width(200)))
            {
                if (VirtualServerController.instance)
                    VirtualServerController.instance.fightEidtor = true;
                Fight.Controller.FightController.instance.autoFight = false;
            }
            if (GUILayout.Button("开始战斗", GUILayout.Width(200)))
            {
                if (VirtualServerController.instance)
                    VirtualServerController.instance.fightEidtor = false;
            }
            if (Game.GameSetting.instance.speedMode == Game.GameSpeedMode.Normal)
            {
                if (GUILayout.Button("两倍速", GUILayout.Width(200)))
                {
                    Game.GameSetting.instance.speedMode = Game.GameSpeedMode.Double;
                }
            }
            if (Game.GameSetting.instance.speedMode == Game.GameSpeedMode.Double)
            {
                if (GUILayout.Button("一倍速", GUILayout.Width(200)))
                {
                    Game.GameSetting.instance.speedMode = Game.GameSpeedMode.Normal;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (character != GetSelectedCharacterEntity())//角色改变
            {
                character = GetSelectedCharacterEntity();
            }
            if (character == null || character is PetEntity)
                return;
            bool isHero = character is HeroEntity || character is PlayerEntity;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("去死", GUILayout.Width(200)))
            {
                character.SetDamageValue(character, null, character.characterInfo.maxHP);
                //character.isDead = true;
            }
            if (character is PlayerEntity)
            {
                if (GUILayout.Button("召唤兽技能", GUILayout.Width(200)))
                {
                    Logic.UI.SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<Logic.UI.SkillBar.View.SkillBarView>(Logic.UI.SkillBar.View.SkillBarView.PREFAB_PATH);
                    Logic.UI.SkillBar.View.SkillItemBoxView skillItemBoxView = skillBarView.FindSkillItemViewOfPlayer();
                    if (skillItemBoxView)
                        skillItemBoxView.AeonBtnClickHandler();
                }
            }
            if (GUILayout.Button("重新导入相关CSV", GUILayout.Width(200)))
            {
                SkillData.ReImportCSV();
                EffectData.ReImportCSV();
                MechanicsData.ReImportCSV();
                AnimationData.ReImportCSV();
                PassiveData.ReImportCSV();
                Debugger.Log("重新导入相关CSV成功！");
            }
            if (GUILayout.Button("保存数据", GUILayout.Width(200)))
            {
                if (SkillData.SaveCSV() && EffectData.SaveCSV() && MechanicsData.SaveCSV() && AnimationData.SaveCSV() && PassiveData.SaveCSV())
                    Debugger.Log("保存成功！");
                else
                    Debugger.Log("保存失败！");
                AssetDatabase.Refresh();
            }
            timeScaleEnable = EditorGUILayout.Toggle("时间缩放:", timeScaleEnable, GUILayout.Width(300));
            timeScale = EditorGUILayout.Slider("慢放", timeScale, 0, 1, GUILayout.Width(500));
            if (timeScaleEnable)
                Time.timeScale = timeScale;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("暂停"))
                Debug.Break();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            buffType = (BuffType)EditorGUILayout.Popup("Buff 类型：", (int)buffType, buffTypes, GUILayout.Width(200));
            buffAddType = (BuffAddType)EditorGUILayout.EnumPopup("固定值或百分比：", buffAddType, GUILayout.Width(200));
            skillLevelBuffAddType = (SkillLevelBuffAddType)EditorGUILayout.EnumPopup("时间或数值：", skillLevelBuffAddType, GUILayout.Width(200));
            buffTime = EditorGUILayout.FloatField("buff时长：", buffTime, GUILayout.Width(200));
            buffValue = EditorGUILayout.FloatField("buff值或概率：", buffValue, GUILayout.Width(200));
            if (GUILayout.Button("增加", GUILayout.Width(200)))
            {
                MechanicsData mechanics = Logic.Skill.Model.MechanicsData.GetMechanicsDataById(1021);
                character.AddBuff(character, character, character.characterInfo.hitSkillInfo, mechanics, buffType, skillLevelBuffAddType, buffAddType, buffTime, buffValue, 1, 1);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("角色名称：" + character.characterName);
            posIndex = (PosIndex)(isHero ? character.positionId : character.positionId - 100);
            posIndex = (PosIndex)EditorGUILayout.EnumPopup("选择角色位置：", posIndex);
            if (posIndex != 0)
                character.positionId = (uint)posIndex + (uint)(isHero ? 0 : 100);
            EditorGUILayout.LabelField("角色位置：" + character.pos.ToString());
            EditorGUILayout.LabelField("角色转向：" + character.eulerAngles.ToString());
            EditorGUILayout.LabelField("角色缩放：" + character.scale.ToString());
            EditorGUILayout.LabelField("角色高度：" + character.height.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            #region 普通攻击1
            SkillInfo hitSkillInfo = character.characterInfo.hitSkillInfo;
            ShowSkillInfo(hitSkillInfo, ref hitFoldout, ref hitGetHitFoldout, ref hitEffectsFoldout, ref hitAoeEffectFoldout, ref hitFlyEffectFoldout, ref hitScrollPos, 0);
            #endregion
            //#region 普通攻击2
            //SkillInfo hitSkillInfo2 = character.characterInfo.hitSkillInfo;
            //ShowSkillInfo(hitSkillInfo2, hitFoldout, hitGetHitFoldout);
            //#endregion
            #region 技能1
            SkillInfo skillInfo1 = character.characterInfo.skillInfo1;
            ShowSkillInfo(skillInfo1, ref skill1Foldout, ref skill1GetHitFoldout, ref skill1EffectsFoldout, ref skill1AoeEffectFoldout, ref skill1FlyEffectFoldout, ref skill1ScrollPos, 1);
            #endregion
            #region 技能2
            SkillInfo skillInfo2 = character.characterInfo.skillInfo2;
            if (skillInfo2 != null)
            {
                ShowSkillInfo(skillInfo2, ref skill2Foldout, ref skill2GetHitFoldout, ref skill2EffectsFoldout, ref skill2AoeEffectFoldout, ref skill2FlyEffectFoldout, ref skill2ScrollPos, 2);
            }
            #endregion
            EditorGUILayout.EndHorizontal();
        }

        private void ShowSkillInfo(SkillInfo skillInfo, ref bool foldout1, ref bool foldout2, ref bool foldout3, ref bool foldout4, ref bool foldout5, ref Vector2 scrollPos, uint skillPosition)
        {
            if (skillInfo == null) return;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            foldout1 = EditorGUILayout.Foldout(foldout1, "展开技能");
            if (GUILayout.Button("释放"))
            {
                if (character is HeroEntity || character is PlayerEntity)
                {
                    //Logic.UI.SkillBar.View.SkillBarView skillBarView = UI.UIMgr.instance.Get<Logic.UI.SkillBar.View.SkillBarView>(Logic.UI.SkillBar.View.SkillBarView.PREFAB_PATH);
                    //Logic.UI.SkillBar.View.SkillItemView skillItemView = skillBarView.FindSkillItem(StringUtil.ConcatNumber(character.characterInfo.userID, skillInfo.skillData.skillId));
                    //if (skillItemView)
                    //    skillItemView.SkillClickHandler();
                    //else
                    FightController.instance.OrderPlayerSkill(character.characterInfo.instanceID, skillInfo.skillData.skillId, true);
                }
                else
                    FightController.instance.OrderEnemySkill(character.characterInfo.instanceID, skillInfo.skillData.skillId, true);
            }
            EditorGUILayout.EndHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            if (foldout1)
            {
                EditorGUILayout.LabelField("技能ID：" + skillInfo.skillData.skillId);
                EditorGUILayout.LabelField("技能名称：" + skillInfo.skillData.skillName);
                EditorGUILayout.LabelField("动作名称：" + skillInfo.animationData.animName);

                //EditorGUILayout.LabelField("技能类型说明： Hit：普通攻击 Skill：技能 Aeon：召唤兽");
                hitSkillType = skillInfo.skillData.skillType;
                hitSkillType = (SkillType)EditorGUILayout.Popup("技能类型：", (int)hitSkillType, skillTypes, GUILayout.Width(500));
                if (hitSkillType != SkillType.None)
                    skillInfo.skillData.skillType = hitSkillType;

                if (skillInfo.skillData.skillType != SkillType.Hit)
                    EditorGUILayout.LabelField("技能层数：" + skillInfo.currentLevel.ToString());

                AnimationData animationData = AnimationData.GetAnimationDataById(skillInfo.skillData.skillId);
                if (animationData != null)
                {
                    //EditorGUILayout.LabelField("动作类型： Root：原地 Trace：移动  Run：跑步攻击");
                    animType = animationData.animType;
                    animType = (AnimType)EditorGUILayout.Popup("动作类型：", (int)animType, animTypes, GUILayout.Width(500));
                    if (animType != AnimType.None)
                        animationData.animType = animType;

                    hitLength = animationData.length;
                    hitLength = EditorGUILayout.FloatField("动画长度(技能长度)：", hitLength, GUILayout.Width(500));
                    animationData.length = hitLength;

                    bootTime = skillInfo.skillData.bootTime;
                    bootTime = EditorGUILayout.FloatField("引导时长：", bootTime, GUILayout.Width(500));
                    skillInfo.skillData.bootTime = bootTime;

                    hitPauseTime = skillInfo.skillData.pauseTime;
                    hitPauseTime = EditorGUILayout.FloatField("暂停时长：", hitPauseTime, GUILayout.Width(500));
                    skillInfo.skillData.pauseTime = hitPauseTime;

                    closeup = skillInfo.animationData.closeup;
                    closeup = EditorGUILayout.Toggle("是否有特写镜头", closeup, GUILayout.Width(200));
                    skillInfo.animationData.closeup = closeup;

                    if (closeup)
                    {
                        closeupOverTime = skillInfo.animationData.closeupOverTime;
                        closeupOverTime = EditorGUILayout.FloatField("镜头特写结束时间：", closeupOverTime, GUILayout.Width(500));
                        skillInfo.animationData.closeupOverTime = closeupOverTime;
                    }

                    if (AnimType.Root != animType)
                    {
                        hitOffset = animationData.offset;
                        hitOffset = EditorGUILayout.FloatField("移动攻击站位偏移距离：", hitOffset, GUILayout.Width(500));
                        animationData.offset = hitOffset;

                        hitMoveTime = animationData.moveTime;
                        hitMoveTime = EditorGUILayout.FloatField("移动时间：", hitMoveTime, GUILayout.Width(500));
                        animationData.moveTime = hitMoveTime;

                        hitTime = animationData.hitTime;
                        hitTime = EditorGUILayout.FloatField("攻击时间：", hitTime, GUILayout.Width(500));
                        animationData.hitTime = hitTime;

                        if (animType == AnimType.Trace)
                        {
                            hitBackTime = animationData.backTime;
                            hitBackTime = EditorGUILayout.FloatField("移动攻击回撤时间：", hitBackTime, GUILayout.Width(500));
                            animationData.backTime = hitBackTime;
                        }
                        else if (animType == AnimType.Run)
                        {
                            runSpeed = animationData.runSpeed;
                            runSpeed = EditorGUILayout.FloatField("跑步速度：", runSpeed, GUILayout.Width(500));
                            animationData.runSpeed = runSpeed;
                        }
                        animationIsRotate = animationData.isRotate;
                        animationIsRotate = EditorGUILayout.Toggle("是否旋转角色:", animationIsRotate, GUILayout.Width(200));
                        animationData.isRotate = animationIsRotate;
                    }

                    if (AnimType.Run != animType)
                    {
                        endShowTime = animationData.endShowTime;
                        endShowTime = EditorGUILayout.FloatField("收招动画时间(原地技能也要填)：", endShowTime, GUILayout.Width(500));
                        animationData.endShowTime = endShowTime;
                    }
                }

                EditorGUILayout.Space();
                cameraTime = skillInfo.skillData.cameraTime;
                cameraTime = EditorGUILayout.FloatField("镜头拉近时间点：", cameraTime, GUILayout.Width(500));
                skillInfo.skillData.cameraTime = cameraTime;

                cameraLength = skillInfo.skillData.cameraLength;
                cameraLength = EditorGUILayout.FloatField("镜头持续时长：", cameraLength, GUILayout.Width(500));
                skillInfo.skillData.cameraLength = cameraLength;

                blackScreen = (int)skillInfo.animationData.blackScreen;
                blackScreen = EditorGUILayout.IntField("黑屏特效", blackScreen, GUILayout.Width(200));
                skillInfo.animationData.blackScreen = (uint)blackScreen;
                if (skillInfo.animationData.blackScreen > 0)
                {
                    EffectData effectData = EffectData.GetEffectDataById(skillInfo.animationData.blackScreen);
                    List<EffectType> canSelectTypeList = new List<EffectType>();
                    canSelectTypeList.Add(EffectType.BlackScreen);
                    EffectGUILayout(effectData, canSelectTypeList);
                }

                blackTime = skillInfo.skillData.blackTime;
                blackTime = EditorGUILayout.FloatField("黑屏时间点：", blackTime, GUILayout.Width(500));
                skillInfo.skillData.blackTime = blackTime;

                blackLength = skillInfo.skillData.blackLength;
                blackLength = EditorGUILayout.FloatField("黑屏持续时长：", blackLength, GUILayout.Width(500));
                skillInfo.skillData.blackLength = blackLength;

                fullScreen = (int)animationData.fullScreen;
                fullScreen = EditorGUILayout.IntField("全屏特效：", fullScreen, GUILayout.Width(200));
                animationData.fullScreen = (uint)fullScreen;
                if (skillInfo.animationData.fullScreen > 0)
                {
                    EffectData effectData = EffectData.GetEffectDataById(skillInfo.animationData.fullScreen);
                    List<EffectType> canSelectTypeList = new List<EffectType>();
                    canSelectTypeList.Add(EffectType.FullScreen);
                    EffectGUILayout(effectData, canSelectTypeList);
                }

                fullScreenTime = skillInfo.skillData.fullScreenTime;
                fullScreenTime = EditorGUILayout.FloatField("全屏特效时间点：", fullScreenTime, GUILayout.Width(500));
                skillInfo.skillData.fullScreenTime = fullScreenTime;

                fullScreenLength = skillInfo.skillData.fullScreenLength;
                fullScreenLength = EditorGUILayout.FloatField("全屏特效持续时长：", fullScreenLength, GUILayout.Width(500));
                skillInfo.skillData.fullScreenLength = fullScreenLength;

                bgEffect = (int)animationData.bgEffect;
                bgEffect = EditorGUILayout.IntField("背景特效：", bgEffect, GUILayout.Width(200));
                animationData.bgEffect = (uint)bgEffect;
                if (skillInfo.animationData.bgEffect > 0)
                {
                    EffectData effectData = EffectData.GetEffectDataById(skillInfo.animationData.bgEffect);
                    List<EffectType> canSelectTypeList = new List<EffectType>();
                    canSelectTypeList.Add(EffectType.BGEffect);
                    EffectGUILayout(effectData, canSelectTypeList);
                }

                EditorGUILayout.Space();
                audioIds = skillInfo.skillData.audioIds.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                audioIds = EditorGUILayout.TextField("音效Id：", audioIds, GUILayout.Width(500));
                skillInfo.skillData.audioIds = audioIds.ToList<int>(CSVUtil.SYMBOL_SEMICOLON);

                skillAudioDelay = skillInfo.skillData.audioDelay;
                skillAudioDelay = EditorGUILayout.FloatField("效果音效延时：", skillAudioDelay, GUILayout.Width(500));
                skillInfo.skillData.audioDelay = skillAudioDelay;

                if (skillInfo.skillData.skillType == SkillType.Skill)
                {
                    if (skillPosition == 1)
                        EditorGUILayout.LabelField("当前CD：", character.skill1CD.ToString(), GUILayout.Width(500));
                    else
                        EditorGUILayout.LabelField("当前CD：", character.skill2CD.ToString(), GUILayout.Width(500));
                    hitCD = skillInfo.skillData.CD;
                    hitCD = EditorGUILayout.FloatField("CD：", hitCD, GUILayout.Width(500));
                    skillInfo.skillData.CD = hitCD;
                }
                else if (skillInfo.skillData.skillType == SkillType.Hit)
                {
                    //EditorGUILayout.LabelField("当前CD：", character.hitCD.ToString(), GUILayout.Width(500));
                    //EditorGUILayout.LabelField("CD：", (Const.Model.ConstData.GetConstData().speedMax / character.speed).ToString(), GUILayout.Width(500));
                }
                //EditorGUILayout.LabelField("可攻击类型：Normal：正常攻击 Float：只能攻击浮空 Tumble：只能攻击倒地 FloatAndNormal：浮空和正常攻击 TumbleAndNormal：倒地和正常攻击");
                attackableType = skillInfo.skillData.attackableType;
                attackableType = (AttackableType)EditorGUILayout.Popup("可攻击类型：", (int)attackableType, attackableTypes, GUILayout.Width(500));
                if (attackableType != AttackableType.None)
                    skillInfo.skillData.attackableType = attackableType;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("施法特效只做表现");
                effectsID = skillInfo.skillData.effectIds.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                effectsID = EditorGUILayout.TextField("施法特效列表：", effectsID, GUILayout.Width(500));
                skillInfo.skillData.effectIds = effectsID.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);

                foldout3 = EditorGUILayout.Foldout(foldout3, "展开施法特效列表");
                if (foldout3)
                {
                    foreach (uint effectId in skillInfo.skillData.effectIds)
                    {
                        if (EffectData.ExsitGetEffectData(effectId))
                        {
                            EffectData effectData = EffectData.GetEffectDataById(effectId);
                            List<EffectType> canSelectTypeList = new List<EffectType>();
                            canSelectTypeList.Add(EffectType.LockPart);
                            canSelectTypeList.Add(EffectType.Root);
                            canSelectTypeList.Add(EffectType.ShakeScreen);
                            canSelectTypeList.Add(EffectType.BlackScreen);
                            canSelectTypeList.Add(EffectType.LockTarget);
                            canSelectTypeList.Add(EffectType.TargetArea);
                            canSelectTypeList.Add(EffectType.MoveTargetPos);
                            canSelectTypeList.Add(EffectType.FullScreen);
                            EffectGUILayout(effectData, canSelectTypeList);
                        }
                    }
                }
                EditorGUILayout.Space();
                if (animType == AnimType.Root)
                {
                    aoeEffectsID = skillInfo.skillData.aoeEffects.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                    aoeEffectsID = EditorGUILayout.TextField("AOE特效列表：", aoeEffectsID, GUILayout.Width(500));
                    skillInfo.skillData.aoeEffects = aoeEffectsID.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);

                    foldout4 = EditorGUILayout.Foldout(foldout4, "展开AOE特效列表");
                    if (foldout4)
                    {
                        EditorGUILayout.LabelField("AOE飞行特效不产生任何效果只做表现，配合AOE特效使用");
                        EditorGUILayout.LabelField("可不填，一旦使用必须与AOE特效11对应");
                        aoeFlyEffectsID = skillInfo.skillData.aoeFlyEffects.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        aoeFlyEffectsID = EditorGUILayout.TextField("AOE飞行特效列表：", aoeFlyEffectsID, GUILayout.Width(500));
                        skillInfo.skillData.aoeFlyEffects = aoeFlyEffectsID.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);
                        foreach (uint effectId in skillInfo.skillData.aoeFlyEffects)
                        {
                            EffectData effectData = EffectData.GetEffectDataById(effectId);
                            List<EffectType> canSelectTypeList = new List<EffectType>();
                            canSelectTypeList.Add(EffectType.CurveTrace);
                            canSelectTypeList.Add(EffectType.Trace);
                            EffectGUILayout(effectData, canSelectTypeList);
                        }

                        EditorGUILayout.LabelField("AOE特效");
                        foreach (uint effectId in skillInfo.skillData.aoeEffects)
                        {
                            EffectData effectData = EffectData.GetEffectDataById(effectId);
                            List<EffectType> canSelectTypeList = new List<EffectType>();
                            canSelectTypeList.Add(EffectType.TargetArea);
                            canSelectTypeList.Add(EffectType.FullScreen);
                            EffectGUILayout(effectData, canSelectTypeList);
                        }
                    }
                    EditorGUILayout.Space();
                    hitFlyEffectsID = skillInfo.skillData.flyEffectIds.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                    hitFlyEffectsID = EditorGUILayout.TextField("飞行特效列表：", hitFlyEffectsID, GUILayout.Width(500));
                    skillInfo.skillData.flyEffectIds = hitFlyEffectsID.ToList<uint>(CSVUtil.SYMBOL_SEMICOLON);

                    foldout5 = EditorGUILayout.Foldout(foldout5, "展开飞行特效列表");
                    if (foldout5)
                    {
                        foreach (uint effectId in skillInfo.skillData.flyEffectIds)
                        {
                            EffectData effectData = EffectData.GetEffectDataById(effectId);
                            List<EffectType> canSelectTypeList = new List<EffectType>();
                            canSelectTypeList.Add(EffectType.CurveTrace);
                            canSelectTypeList.Add(EffectType.Trace);
                            canSelectTypeList.Add(EffectType.LockTarget);
                            canSelectTypeList.Add(EffectType.BlackScreen);
                            canSelectTypeList.Add(EffectType.ShakeScreen);
                            EffectGUILayout(effectData, canSelectTypeList);
                        }
                    }
                }
            }
            EditorGUILayout.Space();
            timeline = skillInfo.skillData.timeline.GetKeys().ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
            timeline = EditorGUILayout.TextField("时间线：", timeline, GUILayout.Width(500));
            List<float> time = timeline.ToList<float>(CSVUtil.SYMBOL_SEMICOLON);

            foldout2 = EditorGUILayout.Foldout(foldout2, "展开时间线");
            if (foldout2)
            {
                Dictionary<float, List<uint>> timelineDic = new Dictionary<float, List<uint>>();
                int i = 0;
                foreach (var t in time)
                {
                    EditorGUILayout.LabelField("时间点：" + t.ToString(), GUILayout.Width(500));
                    if (skillInfo.skillData.timeline.Count > i)
                        machanicsIds = skillInfo.skillData.timeline.GetValues()[i].ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                    else
                        machanicsIds = "0";
                    machanicsIds = EditorGUILayout.TextField("效果列表：", machanicsIds, GUILayout.Width(500));
                    timelineDic[t] = machanicsIds.ToList<uint>(CSVUtil.SYMBOL_SEMICOLON);
                    int j = 0;
                    foreach (uint mechanicsId in timelineDic[t])
                    {
                        MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsId);
                        //EditorGUILayout.LabelField("目标类型：Ally：友军 Enemy：敌军");
                        targetType = mechanicsData.targetType;
                        targetType = (TargetType)EditorGUILayout.Popup("目标类型：", (int)targetType, targets, GUILayout.Width(500));
                        if (targetType != TargetType.None)
                            mechanicsData.targetType = targetType;

                        //EditorGUILayout.LabelField("范围类型：CurrentSingle：当前目标单体 CurrentRow：当前目标所在横行 CurrentColumn：当前目标所在的列 All：全体");
                        rangeType = mechanicsData.rangeType;
                        rangeType = (RangeType)EditorGUILayout.Popup("范围类型：", (int)rangeType, rangeTypes, GUILayout.Width(500));
                        if (rangeType != RangeType.None)
                            mechanicsData.rangeType = rangeType;

                        needTarget = mechanicsData.needTarget;
                        needTarget = EditorGUILayout.Toggle("是否需要目标", needTarget);
                        mechanicsData.needTarget = needTarget;

                        EditorGUILayout.LabelField("效果类型：不支持中文，并且太多了，查表！");
                        mechanicsType = mechanicsData.mechanicsType;
                        int mindex = mechanicsTypeIndexs.IndexOf(mechanicsType);
                        mindex = EditorGUILayout.Popup("效果类型：", mindex, mechanicsTypes, GUILayout.Width(500));
                        mechanicsType = mechanicsTypeIndexs[mindex];
                        EditorGUILayout.LabelField("效果类型：" + mechanicsType.ToString() + "(" + ((int)mechanicsType).ToString() + ")");
                        if (mechanicsType != MechanicsType.None)
                            mechanicsData.mechanicsType = mechanicsType;

                        #region 效果数值
                        switch (mechanicsData.mechanicsType)
                        {
                            case MechanicsType.Damage:
                                EditorGUILayout.LabelField("伤害效果");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ImmediatePercentDamage:
                                EditorGUILayout.LabelField("直接伤害百分比效果");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("直接伤害百分比：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.DrainDamage:
                                EditorGUILayout.LabelField("伤害并吸血效果");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("吸血比例：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.SwimmyExtraDamage:
                                EditorGUILayout.LabelField("对眩晕状态目标造成额外伤害");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("额外伤害：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.LandificationExtraDamage:
                                EditorGUILayout.LabelField("对石化状态目标造成额外伤害");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("额外伤害：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.BleedExtraDamage:
                                EditorGUILayout.LabelField("对流血状态目标造成额外伤害");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("额外伤害：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.FrozenExtraDamage:
                                EditorGUILayout.LabelField("对冰冻状态目标造成额外伤害");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("额外伤害：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.PoisoningExtraDamage:
                                EditorGUILayout.LabelField("对中毒状态目标造成额外伤害");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("额外伤害：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.TagExtraDamage:
                                EditorGUILayout.LabelField("对标记状态目标造成额外伤害");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("额外伤害：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.IgniteExtraDamage:
                                EditorGUILayout.LabelField("对点燃状态目标造成额外伤害");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("额外伤害：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Treat:
                                EditorGUILayout.LabelField("加血效果");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("加血量：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Poisoning:
                                EditorGUILayout.LabelField("中毒，持续伤害效果，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("每秒伤害百分比：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Ignite:
                                EditorGUILayout.LabelField("点燃，持续伤害效果，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("每秒伤害百分比：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Bleed:
                                EditorGUILayout.LabelField("流血，持续伤害效果，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("每秒伤害百分比：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.LastTreat:
                                EditorGUILayout.LabelField("持续回血效果，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("每秒回血百分比：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.TreatPercent:
                                EditorGUILayout.LabelField("持续回血百分比(最大生命值)，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("最大生命值回血百分比：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.IgnoreDefenseDamage:
                                EditorGUILayout.LabelField("无视防御");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("伤害：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Swimmy:
                                EditorGUILayout.LabelField("眩晕");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Frozen:
                                EditorGUILayout.LabelField("冰冻");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Sleep:
                                EditorGUILayout.LabelField("睡眠");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Landification:
                                EditorGUILayout.LabelField("石化");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Tieup:
                                EditorGUILayout.LabelField("禁锢");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Reborn:
                                EditorGUILayout.LabelField("复活");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("血量百分比：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Float:
                                EditorGUILayout.LabelField("浮空");
                                mechanicsA = 0f;
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Tumble:
                                EditorGUILayout.LabelField("倒地");
                                mechanicsA = 0f;
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Invincible:
                                EditorGUILayout.LabelField("无敌");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Silence:
                                EditorGUILayout.LabelField("沉默");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Blind:
                                EditorGUILayout.LabelField("致盲");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Disperse:
                                EditorGUILayout.LabelField("驱散");
                                mechanicsA = 0f;
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Immune:
                                EditorGUILayout.LabelField("免疫特殊状态");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ReboundTime:
                                EditorGUILayout.LabelField("反伤(时间随等级提升)");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("反伤比率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ReboundValue:
                                EditorGUILayout.LabelField("反伤(数值随等级提升)");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("反伤比率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.RandomMechanics:
                                EditorGUILayout.LabelField("随机效果");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("随机效果库ID：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = 0;
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ShieldTime:
                                EditorGUILayout.LabelField("一定时间吸收固定伤害，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("吸收伤害值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ShieldValue:
                                EditorGUILayout.LabelField("一定时间吸收固定伤害，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("吸收伤害值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.DrainTime:
                                EditorGUILayout.LabelField("吸血，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("吸收伤害值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.DrainValue:
                                EditorGUILayout.LabelField("吸血，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("吸收伤害值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ImmunePhysicsAttack:
                                EditorGUILayout.LabelField("免疫物理攻击，时间技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ImmuneMagicAttack:
                                EditorGUILayout.LabelField("免疫魔法攻击，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Tag:
                                EditorGUILayout.LabelField("标记，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.GeneralSkillHit:
                                EditorGUILayout.LabelField("必定命中");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("次数：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.GeneralSkillCrit:
                                EditorGUILayout.LabelField("必定暴击");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("次数：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.AccumulatorTag:
                                EditorGUILayout.LabelField("蓄力标记，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ForceKill:
                                EditorGUILayout.LabelField("即死");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("概率：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.PhysicsDefensePercentTime:
                                EditorGUILayout.LabelField("物理防御百分比，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.PhysicsDefensePercentValue:
                                EditorGUILayout.LabelField("物理防御百分比，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.MagicDefensePercentTime:
                                EditorGUILayout.LabelField("魔法防御百分比，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.MagicDefensePercentValue:
                                EditorGUILayout.LabelField("魔法防御百分比，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.PhysicsAttackPercentTime:
                                EditorGUILayout.LabelField("物理攻击百分比，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.PhysicsAttackPercentValue:
                                EditorGUILayout.LabelField("物理攻击百分比，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.MagicAttackPercentTime:
                                EditorGUILayout.LabelField("魔法攻击百分比，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.MagicAttackPercentValue:
                                EditorGUILayout.LabelField("魔法攻击百分比，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.HPLimitPercentTime:
                                EditorGUILayout.LabelField("生命值上限百分比，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.HPLimitPercentValue:
                                EditorGUILayout.LabelField("生命值上限百分比，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.SpeedPercentTime:
                                EditorGUILayout.LabelField("行动力百分比，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.SpeedPercentValue:
                                EditorGUILayout.LabelField("行动力百分比，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.PhysicsDefenseFixedTime:
                                EditorGUILayout.LabelField("物理防御固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.PhysicsDefenseFixedValue:
                                EditorGUILayout.LabelField("物理防御固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.MagicDefenseFixedTime:
                                EditorGUILayout.LabelField("魔法防御固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.MagicDefenseFixedValue:
                                EditorGUILayout.LabelField("魔法防御固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.PhysicsAttackFixedTime:
                                EditorGUILayout.LabelField("物理攻击固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.PhysicsAttackFixedValue:
                                EditorGUILayout.LabelField("物理攻击固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.MagicAttackFixedTime:
                                EditorGUILayout.LabelField("魔法攻击固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.MagicAttackFixedValue:
                                EditorGUILayout.LabelField("魔法攻击固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.HPLimitFixedTime:
                                EditorGUILayout.LabelField("生命值上限固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.HPLimitFixedValue:
                                EditorGUILayout.LabelField("生命值上限固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.SpeedFixedTime:
                                EditorGUILayout.LabelField("行动力固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.SpeedFixedValue:
                                EditorGUILayout.LabelField("行动力固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.HitTime:
                                EditorGUILayout.LabelField("命中固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.HitValue:
                                EditorGUILayout.LabelField("命中固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.DodgeTime:
                                EditorGUILayout.LabelField("闪避固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.DodgeValue:
                                EditorGUILayout.LabelField("闪避固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.CritTime:
                                EditorGUILayout.LabelField("暴击固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.CritValue:
                                EditorGUILayout.LabelField("暴击固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.AntiCritTime:
                                EditorGUILayout.LabelField("防爆固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.AntiCritValue:
                                EditorGUILayout.LabelField("防爆固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.BlockTime:
                                EditorGUILayout.LabelField("格挡固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.BlockValue:
                                EditorGUILayout.LabelField("格挡固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.AntiBlockTime:
                                EditorGUILayout.LabelField("破击固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.AntiBlockValue:
                                EditorGUILayout.LabelField("破击固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.CounterAtkTime:
                                EditorGUILayout.LabelField("反击固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.CounterAtkValue:
                                EditorGUILayout.LabelField("反击固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.CritHurtAddTime:
                                EditorGUILayout.LabelField("暴击伤害加成固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.CritHurtAddValue:
                                EditorGUILayout.LabelField("暴击伤害加成固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.CritHurtDecTime:
                                EditorGUILayout.LabelField("暴击伤害减免固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.CritHurtDecValue:
                                EditorGUILayout.LabelField("暴击伤害减免固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ArmorTime:
                                EditorGUILayout.LabelField("破甲固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.ArmorValue:
                                EditorGUILayout.LabelField("破甲固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.DamageDecTime:
                                EditorGUILayout.LabelField("伤害减免固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.DamageDecValue:
                                EditorGUILayout.LabelField("伤害减免固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.DamageAddTime:
                                EditorGUILayout.LabelField("伤害加成固定值，时间随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.DamageAddValue:
                                EditorGUILayout.LabelField("伤害加成固定值，值随技能等级提升");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("持续时间：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = skillInfo.skillData.mechanicsValues[i][j].b;
                                mechanicsB = EditorGUILayout.FloatField("数值：", mechanicsB, GUILayout.Width(500));
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            case MechanicsType.Transform:
                                EditorGUILayout.LabelField("变换");
                                mechanicsA = skillInfo.skillData.mechanicsValues[i][j].a;
                                mechanicsA = EditorGUILayout.FloatField("英雄变换Id：", mechanicsA, GUILayout.Width(500));
                                mechanicsB = 0f;
                                mechanicsC = skillInfo.skillData.mechanicsValues[i][j].c;
                                mechanicsC = EditorGUILayout.FloatField("概率：", mechanicsC, GUILayout.Width(500));
                                break;
                            default:
                                break;
                        }
                        skillInfo.skillData.mechanicsValues[i][j] = new Triple<float, float, float>(mechanicsA, mechanicsB, mechanicsC);
                        #endregion

                        mechanicsEffectIds = mechanicsData.effectIds.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        mechanicsEffectIds = EditorGUILayout.TextField("效果特效列表：", mechanicsEffectIds, GUILayout.Width(500));
                        mechanicsData.effectIds = mechanicsEffectIds.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);
                        foreach (uint effectId in mechanicsData.effectIds)
                        {
                            EffectData effectData = EffectData.GetEffectDataById(effectId);
                            List<EffectType> canSelectTypeList = new List<EffectType>();
                            canSelectTypeList.Add(EffectType.LockPart);
                            canSelectTypeList.Add(EffectType.LockTarget);
                            canSelectTypeList.Add(EffectType.Root);
                            canSelectTypeList.Add(EffectType.ShakeScreen);
                            canSelectTypeList.Add(EffectType.BlackScreen);
                            canSelectTypeList.Add(EffectType.ChangeColor);
                            EffectGUILayout(effectData, canSelectTypeList);
                        }

                        mechanicsAudioType = mechanicsData.audioType;
                        mechanicsAudioType = EditorGUILayout.IntField("音效类型Id：", mechanicsAudioType, GUILayout.Width(500));
                        mechanicsData.audioType = mechanicsAudioType;

                        mechanicsAudioDelay = mechanicsData.audioDelay;
                        mechanicsAudioDelay = EditorGUILayout.FloatField("效果音效延时：", mechanicsAudioDelay, GUILayout.Width(500));
                        mechanicsData.audioDelay = mechanicsAudioDelay;
                        j++;

                        maxLayer = mechanicsData.maxLayer;
                        maxLayer = (uint)EditorGUILayout.IntField("最大效果层数", (int)maxLayer);
                        mechanicsData.maxLayer = maxLayer;
                    }
                    i++;
                }
                //if (GUILayout.Button("修改"))
                skillInfo.skillData.timeline = timelineDic;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void EffectGUILayout(EffectData effectData, List<EffectType> canSelectTypeList)
        {
            if (effectData != null)
            {
                EditorGUILayout.TextField("特效名称：", effectData.effectName, GUILayout.Width(500));
                effectType = effectData.effectType;
                effectType = (EffectType)EditorGUILayout.Popup("特效类型：", (int)effectType, effectTypes, GUILayout.Width(500));
                if (canSelectTypeList.Contains(effectType))
                    effectData.effectType = effectType;

                if (effectType != EffectType.ChangeColor)
                {
                    effectDelay = effectData.delay;
                    effectDelay = EditorGUILayout.FloatField("特效延迟出现时长：", effectDelay, GUILayout.Width(500));
                    effectData.delay = effectDelay;
                }

                ignoreTimeScale = effectData.ignoreTimeScale;
                ignoreTimeScale = EditorGUILayout.Toggle("特效忽略暂停：", ignoreTimeScale, GUILayout.Width(500));
                effectData.ignoreTimeScale = ignoreTimeScale;
                switch (effectType)
                {
                    case EffectType.BlackScreen:
                        //effectLength = effectData.length;
                        //effectLength = EditorGUILayout.FloatField("黑屏时长：", effectLength, GUILayout.Width(500));
                        //effectData.length = effectLength;
                        break;
                    case EffectType.Root:
                        {
                            effectLength = effectData.length;
                            effectLength = EditorGUILayout.FloatField("特效长度：", effectLength, GUILayout.Width(500));
                            effectData.length = effectLength;

                            effectOffset = effectData.offset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                            effectOffset = EditorGUILayout.TextField("特效释放偏移距离：", effectOffset, GUILayout.Width(500));
                            effectData.offset = effectOffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);

                            effectEndoffset = effectData.endOffset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                            effectEndoffset = EditorGUILayout.TextField("特效消失偏移距离：", effectEndoffset, GUILayout.Width(500));
                            effectData.endOffset = effectEndoffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);

                            StringBuilder sb = new StringBuilder();
                            for (int i = 0, count = effectData.randomAngles.Count; i < count; i++)
                            {
                                sb.Append(effectData.randomAngles[i].ToCustomString());
                                if (i != count - 1)
                                    sb.Append(CSVUtil.SYMBOL_SEMICOLON);
                            }
                            randomAngle = sb.ToString();
                            EditorGUILayout.LabelField("多个角度有|分开，格式30;30;30|60;60;60");
                            randomAngle = EditorGUILayout.TextField("受击随机角度：", randomAngle, GUILayout.Width(500));
                            string[] vector3s = randomAngle.ToArray(CSVUtil.SYMBOL_PIPE);
                            List<Vector3> randoms = new List<Vector3>();
                            for (int i = 0, count = vector3s.Length; i < count; i++)
                            {
                                randoms.Add(vector3s[i].ToVector3(CSVUtil.SYMBOL_SEMICOLON));
                            }
                            effectData.randomAngles = randoms;
                        }
                        break;
                    case EffectType.Trace:
                        effectOffset = effectData.offset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        effectOffset = EditorGUILayout.TextField("特效释放偏移距离：", effectOffset, GUILayout.Width(500));
                        effectData.offset = effectOffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);

                        effectEndoffset = effectData.endOffset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        effectEndoffset = EditorGUILayout.TextField("特效消失偏移距离：", effectEndoffset, GUILayout.Width(500));
                        effectData.endOffset = effectEndoffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);

                        effectMoveTime = effectData.moveTime;
                        effectMoveTime = EditorGUILayout.FloatField("特效移动时长：", effectMoveTime, GUILayout.Width(500));
                        effectData.moveTime = effectMoveTime;
                        break;
                    case EffectType.TargetArea:
                        effectLength = effectData.length;
                        effectLength = EditorGUILayout.FloatField("特效长度：", effectLength, GUILayout.Width(500));
                        effectData.length = effectLength;
                        break;
                    case EffectType.LockPart:
                        {
                            effectLength = effectData.length;
                            effectLength = EditorGUILayout.FloatField("特效长度：", effectLength, GUILayout.Width(500));
                            effectData.length = effectLength;

                            partName = effectData.partName;
                            partName = EditorGUILayout.TextField("骨骼名称", partName, GUILayout.Width(500));
                            effectData.partName = partName;

                            effectIsRotate = effectData.isRotate;
                            effectIsRotate = EditorGUILayout.Toggle("是否跟随骨骼旋转:", effectIsRotate, GUILayout.Width(200));
                            effectData.isRotate = effectIsRotate;

                            effectOffset = effectData.offset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                            effectOffset = EditorGUILayout.TextField("特效释放偏移距离：", effectOffset, GUILayout.Width(500));
                            effectData.offset = effectOffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);

                            StringBuilder sb = new StringBuilder();
                            for (int i = 0, count = effectData.randomAngles.Count; i < count; i++)
                            {
                                sb.Append(effectData.randomAngles[i].ToCustomString());
                                if (i != count - 1)
                                    sb.Append(CSVUtil.SYMBOL_SEMICOLON);
                            }
                            randomAngle = sb.ToString();
                            randomAngle = EditorGUILayout.TextField("受击随机角度：", randomAngle, GUILayout.Width(500));
                            string[] vector3s = randomAngle.ToArray(CSVUtil.SYMBOL_SEMICOLON);
                            List<Vector3> randoms = new List<Vector3>();
                            for (int i = 0, count = vector3s.Length; i < count; i++)
                            {
                                randoms.Add(vector3s[i].ToVector3());
                            }
                            effectData.randomAngles = randoms;
                        }
                        break;
                    case EffectType.LockTarget:
                        effectLength = effectData.length;
                        effectLength = EditorGUILayout.FloatField("特效长度：", effectLength, GUILayout.Width(500));
                        effectData.length = effectLength;

                        effectOffset = effectData.offset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        effectOffset = EditorGUILayout.TextField("特效释放偏移距离：", effectOffset, GUILayout.Width(500));
                        effectData.offset = effectOffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);

                        effectIsRotate = effectData.isRotate;
                        effectIsRotate = EditorGUILayout.Toggle("是否跟随角色旋转(仅用于施法特效):", effectIsRotate, GUILayout.Width(200));
                        effectData.isRotate = effectIsRotate;
                        break;
                    case EffectType.CurveTrace:
                        effectOffset = effectData.offset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        effectOffset = EditorGUILayout.TextField("特效释放偏移距离：", effectOffset, GUILayout.Width(500));
                        effectData.offset = effectOffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);

                        effectEndoffset = effectData.endOffset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        effectEndoffset = EditorGUILayout.TextField("特效消失偏移距离：", effectEndoffset, GUILayout.Width(500));
                        effectData.endOffset = effectEndoffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);

                        effectMoveTime = effectData.moveTime;
                        effectMoveTime = EditorGUILayout.FloatField("特效移动时长：", effectMoveTime, GUILayout.Width(500));
                        effectData.moveTime = effectMoveTime;

                        curvePoint1 = effectData.curvePoint1.Key.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        curvePoint1 = EditorGUILayout.TextField("曲线点1：", curvePoint1, GUILayout.Width(500));
                        curvePoint1EulerAngles = effectData.curvePoint1.Value;
                        curvePoint1EulerAngles = EditorGUILayout.FloatField("曲线点1旋转角度：", curvePoint1EulerAngles, GUILayout.Width(500));
                        effectData.curvePoint1 = new KeyValuePair<Vector3, float>(curvePoint1.ToVector3(CSVUtil.SYMBOL_SEMICOLON), curvePoint1EulerAngles);

                        curvePoint2 = effectData.curvePoint2.Key.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        curvePoint2 = EditorGUILayout.TextField("曲线点2：", curvePoint2, GUILayout.Width(500));
                        curvePoint2EulerAngles = effectData.curvePoint2.Value;
                        curvePoint2EulerAngles = EditorGUILayout.FloatField("曲线点2旋转角度：", curvePoint2EulerAngles, GUILayout.Width(500));
                        effectData.curvePoint2 = new KeyValuePair<Vector3, float>(curvePoint2.ToVector3(CSVUtil.SYMBOL_SEMICOLON), curvePoint2EulerAngles);

                        curvePoint3 = effectData.curvePoint3.Key.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        curvePoint3 = EditorGUILayout.TextField("曲线点3：", curvePoint3, GUILayout.Width(500));
                        curvePoint3EulerAngles = effectData.curvePoint3.Value;
                        curvePoint3EulerAngles = EditorGUILayout.FloatField("曲线点1旋转角度：", curvePoint3EulerAngles, GUILayout.Width(500));
                        effectData.curvePoint3 = new KeyValuePair<Vector3, float>(curvePoint3.ToVector3(CSVUtil.SYMBOL_SEMICOLON), curvePoint3EulerAngles);
                        break;
                    case EffectType.MoveTargetPos:
                        {
                            effectLength = effectData.length;
                            effectLength = EditorGUILayout.FloatField("特效长度：", effectLength, GUILayout.Width(500));
                            effectData.length = effectLength;

                            partName = effectData.partName;
                            partName = EditorGUILayout.TextField("节点名称", partName, GUILayout.Width(500));
                            effectData.partName = partName;

                            effectOffset = effectData.offset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                            effectOffset = EditorGUILayout.TextField("特效释放偏移距离：", effectOffset, GUILayout.Width(500));
                            effectData.offset = effectOffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
                        }
                        break;
                    case EffectType.FullScreen:
                        effectLength = effectData.length;
                        effectLength = EditorGUILayout.FloatField("特效长度：", effectLength, GUILayout.Width(500));
                        effectData.length = effectLength;

                        effectOffset = effectData.offset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        effectOffset = EditorGUILayout.TextField("特效释放偏移距离：", effectOffset, GUILayout.Width(500));
                        effectData.offset = effectOffset.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
                        break;
                    case EffectType.ChangeColor:
                        effectLength = effectData.length;
                        effectLength = EditorGUILayout.FloatField("特效长度：", effectLength, GUILayout.Width(500));
                        effectData.length = effectLength;

                        colorStr = effectData.color.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                        colorStr = EditorGUILayout.TextField("颜色：", colorStr, GUILayout.Width(500));
                        effectData.color = colorStr.ToColor(CSVUtil.SYMBOL_SEMICOLON);
                        break;
                }

                scaleStr = effectData.scale.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                scaleStr = EditorGUILayout.TextField("敌方特效缩放：", scaleStr, GUILayout.Width(500));
                Vector3 scale = scaleStr.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
                if (scale == Vector3.zero)
                    scale = Vector3.one;
                effectData.scale = scale;

                rotateStr = effectData.rotate.ToCustomString(CSVUtil.SYMBOL_SEMICOLON);
                rotateStr = EditorGUILayout.TextField("敌方特效旋转角度：", rotateStr, GUILayout.Width(500));
                effectData.rotate = rotateStr.ToVector3(CSVUtil.SYMBOL_SEMICOLON);

                EditorGUILayout.Space();
            }
        }

        public enum PosIndex
        {
            None = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
        }
    }
}
