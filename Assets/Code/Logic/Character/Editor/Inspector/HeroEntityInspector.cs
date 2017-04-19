using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Logic.Character.Inspector.Editor
{
    [CustomEditor(typeof(Logic.Character.HeroEntity))]
    public class HeroEntityInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying)
            {
                Logic.Character.CharacterEntity characterEntity = target as Logic.Character.CharacterEntity;
                Logic.Character.Model.CharacterBaseInfo characterBaseInfo = characterEntity.characterInfo;
                if (characterBaseInfo == null) return;
                EditorGUILayout.LabelField("userId:", characterEntity.characterInfo.instanceID.ToString());
                EditorGUILayout.LabelField("pos:", characterEntity.pos.ToString());
                EditorGUILayout.LabelField("eulerAngles:", characterEntity.eulerAngles.ToString());
                EditorGUILayout.LabelField("scale:", characterEntity.scale.ToString());
                EditorGUILayout.LabelField("height:", characterEntity.height.ToString());
                EditorGUILayout.LabelField("state:", characterEntity.status.ToString());
                EditorGUILayout.LabelField("tickCD:", characterEntity.tickCD.ToString());
                EditorGUILayout.LabelField("canAttack:", characterEntity.canAttack.ToString());
                EditorGUILayout.LabelField("animator speed:", characterEntity.anim.speed.ToString());
                EditorGUILayout.LabelField("is dead:", characterEntity.isDead.ToString());
                EditorGUILayout.LabelField("HP:", characterEntity.HP.ToString());
                EditorGUILayout.LabelField("MAXHP:", characterEntity.maxHP.ToString());
                EditorGUILayout.LabelField("physicsAttack:", characterEntity.physicsAttack.ToString());
                EditorGUILayout.LabelField("magicAttack:", characterEntity.magicAttack.ToString());
                EditorGUILayout.LabelField("physicsDefense:", characterEntity.physicsDefense.ToString());
                EditorGUILayout.LabelField("magicDefense:", characterEntity.magicDefense.ToString());
                EditorGUILayout.LabelField("speed:", characterEntity.speed.ToString());
                EditorGUILayout.LabelField("hit:", characterEntity.hit.ToString());
                EditorGUILayout.LabelField("dodge:", characterEntity.dodge.ToString());
                EditorGUILayout.LabelField("crit:", characterEntity.crit.ToString());
                EditorGUILayout.LabelField("antiCrit:", characterEntity.antiCrit.ToString());
                EditorGUILayout.LabelField("block:", characterEntity.block.ToString());
                EditorGUILayout.LabelField("antiBlock:", characterEntity.antiBlock.ToString());
                EditorGUILayout.LabelField("counterAtk:", characterEntity.counterAtk.ToString());
                EditorGUILayout.LabelField("critHurtAdd:", characterEntity.critHurtAdd.ToString());
                EditorGUILayout.LabelField("critHurtDec:", characterEntity.critHurtDec.ToString());
                EditorGUILayout.LabelField("armor:", characterEntity.armor.ToString());
                EditorGUILayout.LabelField("damageDec:", characterEntity.damageDec.ToString());
                EditorGUILayout.LabelField("damageAdd:", characterEntity.damageAdd.ToString());
                Logic.Hero.Model.HeroInfo heroInfo = Hero.Model.HeroProxy.instance.GetHeroInfo(characterEntity.characterInfo.instanceID);
                if (heroInfo != null)
                {
                    EditorGUILayout.LabelField("level:", heroInfo.level.ToString());
                    EditorGUILayout.LabelField("advanceLevel:", heroInfo.advanceLevel.ToString());
                    EditorGUILayout.LabelField("breakthroughLevel:", heroInfo.breakthroughLevel.ToString());
                    EditorGUILayout.LabelField("strengthenLevel:", heroInfo.strengthenLevel.ToString());
                    EditorGUILayout.LabelField("exp:", heroInfo.exp.ToString());
                    EditorGUILayout.LabelField("totalLevelUpExp:", heroInfo.TotalLevelUpExp.ToString());
                }
                //if (characterBaseInfo.hitSkillInfo != null)
                //    EditorGUILayout.LabelField("hitCD:", characterEntity.hitCD.ToString() + "/" + (Const.Model.ConstData.GetConstData().speedMax / characterEntity.speed).ToString());
                if (characterBaseInfo.skillInfo1 != null)
                    EditorGUILayout.LabelField("skill1CD:", characterEntity.skill1CD.ToString() + "/" + characterBaseInfo.skillInfo1.skillData.CD);
                if (characterBaseInfo.skillInfo2 != null)
                    EditorGUILayout.LabelField("skill2CD:", characterEntity.skill2CD.ToString() + "/" + characterBaseInfo.skillInfo2.skillData.CD);
                //EditorGUILayout.LabelField("canOrderHit:", characterEntity.canOrderHit.ToString());
                EditorGUILayout.LabelField("canPlayHit:", characterEntity.canPlayHit.ToString());
                EditorGUILayout.LabelField("canOrderSkill1:", characterEntity.canOrderSkill1.ToString());
                EditorGUILayout.LabelField("canPlaySkill1:", characterEntity.canPlaySkill1.ToString());
                EditorGUILayout.LabelField("canOrderSkill2:", characterEntity.canOrderSkill2.ToString());
                EditorGUILayout.LabelField("canPlaySkill2:", characterEntity.canPlaySkill2.ToString());
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("当前buff列表:");
                EditorGUILayout.LabelField("是否眩晕:", characterEntity.Swimmy.ToString());
                EditorGUILayout.LabelField("是否冰冻:", characterEntity.Frozen.ToString());
                EditorGUILayout.LabelField("是否睡眠:", characterEntity.Sleep.ToString());
                EditorGUILayout.LabelField("是否石化:", characterEntity.Landification.ToString());
                EditorGUILayout.LabelField("是否禁锢:", characterEntity.Tieup.ToString());
                EditorGUILayout.LabelField("是否无敌:", characterEntity.Invincible.ToString());
                EditorGUILayout.LabelField("是否沉默:", characterEntity.Silence.ToString());
                EditorGUILayout.LabelField("是否致盲:", characterEntity.Blind.ToString());
                EditorGUILayout.LabelField("是否浮空:", characterEntity.Float.ToString());
                EditorGUILayout.LabelField("是否倒地:", characterEntity.Tumble.ToString());
                EditorGUILayout.LabelField("当前时间:", Time.time.ToString());
                EditorGUILayout.LabelField("当前战斗时间:", Common.GameTime.Controller.TimeController.instance.fightSkillTime.ToString());
                foreach (var kvp in characterEntity.buffDic)
                {
                    EditorGUILayout.LabelField(kvp.Key.ToString() + ":");
                    foreach (var b in kvp.Value)
                    {
                        if (b.mechanics != null)
                            EditorGUILayout.LabelField("技能效果ID:" + b.mechanics.mechanicsId.ToString());
                        switch (b.skillLevelBuffAddType)
                        {
                            case Logic.Enums.SkillLevelBuffAddType.Value:
                                EditorGUILayout.LabelField("按技能等级增加效果值");
                                break;
                            case Logic.Enums.SkillLevelBuffAddType.Time:
                                EditorGUILayout.LabelField("按技能等级增加效果持续时间");
                                break;
                        }
                        if (b.forever)
                            EditorGUILayout.LabelField(string.Format("{0}的光环buff", b.character.characterInfo.baseId));
                        EditorGUILayout.LabelField("buff持续时间到：" + b.time.ToString());
                        EditorGUILayout.LabelField("buff次数：" + b.count.ToString());
                        switch (b.buffAddType)
                        {
                            case Enums.BuffAddType.Fixed:
                                EditorGUILayout.LabelField("增加固定技能效果值：" + b.value.ToString());
                                break;
                            case Enums.BuffAddType.Percent:
                                EditorGUILayout.LabelField("增加百分比技能效果值：" + b.value.ToString());
                                break;
                        }
                    }
                }
                EditorGUILayout.LabelField("当前buff icon列表:");
                foreach (var kvp in characterEntity.buffIconDic)
                {
                    if (kvp.Value >= Common.GameTime.Controller.TimeController.instance.fightSkillTime)
                        EditorGUILayout.LabelField("buff icon:" + kvp.Key);
                }
                EditorGUILayout.LabelField("当前buff特效列表:");
                foreach (var kvp in characterEntity.buffEffectDic)
                {
                    EditorGUILayout.LabelField("Buff 类型:" + kvp.Key.ToString());
                    EditorGUILayout.LabelField("特效名称：");
                    foreach (var k in kvp.Value)
                    {
                        EditorGUILayout.LabelField(k);
                    }
                }
            }
        }
    }
}