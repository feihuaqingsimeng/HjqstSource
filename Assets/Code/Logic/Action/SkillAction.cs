using UnityEngine;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Audio.Controller;
using Logic.Character.Controller;
using Logic.Character;
using Logic.Skill.Model;
using Logic.Effect.Model;
using Logic.Effect.Controller;
using Common.TimeScale;
using Common.Util;
using Logic.Position.Model;
using Logic.Fight.Controller;
using Common.Animators;
using Logic.Game;
using Logic.Judge;
using Logic.Audio.Model;
using Logic.Cameras.Controller;
namespace Logic.Action
{
    public class SkillAction : AIAction
    {
        public bool isPlayer;
        public SkillInfo skillInfo;
        public List<Dictionary<uint, List<KeyValuePair<uint, uint>>>> timelineList;
        public float positionRow;
        public Vector3 endPos;
        public override void Execute()
        {
            if (skillInfo == null || timelineList == null) return;
            if (!character) return;
            if (character.status == Status.Dead) return;
            foreach (var kvp in character.characterInfo.passiveIdDic)
            {
                LuaInterface.LuaFunction func = PassiveSkillController.instance.GetPassiveSkillLuaFunction(PassiveSkillController.PLAY_SKILL, kvp.Key);
                if (func != null)
                    func.Call(character, kvp.Value);
            }
            int layer = (int)LayerType.Fight;
            if (FightController.instance.fightStatus == FightStatus.Normal)
            {
                if (skillInfo.skillData.pauseTime > 0)
                {
                    TimeScaler.instance.PauseTrigger(character, skillInfo.skillData.pauseTime / Game.GameSetting.instance.speed, GameScaleMode.Animator, Game.GameSetting.instance.speed, character.anim);
                }
            }
            if (skillInfo.skillData.showTime)
            {
                Logic.UI.SkillBanner.View.SkillBannerView skillBannerView = Logic.UI.SkillBanner.View.SkillBannerView.Open();
                skillBannerView.Show(skillInfo);
                if (isPlayer && FightController.instance.fightType != FightType.SkillDisplay)
                    Logic.UI.SkillHead.View.SkillHeadView.SetRoleInfo(character.characterInfo.roleInfo, SkillHeadViewShowType.Left);
            }

            MechanicsData mechanicsData = null;
            #region camera close up
            if (GameSetting.instance.closeupCameraable)
            {
                if (FightController.instance.fightStatus == FightStatus.Normal)
                {
                    if ((isPlayer || (!isPlayer && EnemyController.instance.isBoss(character.characterInfo.instanceID))) && skillInfo.animationData.closeup)
                    //if (isPlayer && skillInfo.animationData.closeup)
                    {
                        FightController.instance.isCloseup = true;
                        layer = (int)LayerType.Closeup;
                        EnemyController.instance.ShowHPBarViews(false);
                        PlayerController.instance.ShowHPBarViews(false);
                        if (Skill.SkillUtil.GetSkillMechanicsType(skillInfo) == MechanicsType.None)
                            Logic.UI.SkillBar.Controller.SkillBarController.instance.Show(false);
                        Judge.Controller.JudgeController.instance.ShowDamageBarViewPool(false);
                        Cameras.Controller.CameraController.instance.ShowMainCamera(false);
                        Transform parent = TransformUtil.Find(Logic.Cameras.Controller.CameraController.CAMERA_NODE, character.transform, true);
                        Cameras.Controller.CameraController.instance.ShowCloseupCamera(character, parent);
                        TransformUtil.SwitchLayer(character.transform, layer);
                        List<CharacterEntity> targets = CharacterUtil.SwitchCharacterLayer(skillInfo, timelineList, isPlayer, layer);
                        FightController.instance.SetCloseupCharacter(character, targets);
                    }
                }
            }
            #endregion
            #region camera
            /*
#if UNITY_EDITOR
            if (FightController.imitate || character.characterInfo.roleInfo.heroData.starMax >= 5)
            {
#else
            if (character.characterInfo.roleInfo.heroData.starMax >= 5)
            {
#endif
                if (skillInfo.skillData.skillType == SkillType.Skill)
                {
                    switch (skillInfo.animationData.animType)
                    {
                        case AnimType.Root:
                            {
                                float backTime = skillInfo.skillData.timeline.First().Key - 0.3f;
                                if (backTime > 0.5f)
                                {
                                    float delay = 0.3f;
                                    CameraController.instance.CloseTarget(character, delay);
                                    //int layerNone = (int)LayerType.None;
                                    //SwitchCharactersLayer(layerNone, false);
                                    //CameraController.instance.ResetMainCamera(backTime, () =>
                                    //{
                                    //    SwitchCharactersLayer((int)LayerType.Fight, true);
                                    //});
                                    CameraController.instance.ResetMainCamera(backTime, null);
                                }
                            }
                            break;
                        case AnimType.Trace:
                            {
                                float backTime = skillInfo.animationData.moveTime;
                                if (backTime > 0.5f)
                                {
                                    float delay = 0.3f;
                                    CameraController.instance.CloseTarget(character, delay);
                                    //int layerNone = (int)LayerType.None;
                                    //SwitchCharactersLayer(layerNone, false);
                                    //CameraController.instance.ResetMainCamera(backTime, () =>
                                    //{
                                    //    SwitchCharactersLayer((int)LayerType.Fight, true);
                                    //});
                                    CameraController.instance.ResetMainCamera(backTime, null);
                                }
                            }
                            break;
                        case AnimType.Run:
                            break;

                    }
                }
#if UNITY_EDITOR
            }
#else
            }
#endif
             * */
            if (FightController.instance.fightStatus == FightStatus.Normal && skillInfo.skillData.cameraLength > 0)
            {
                CameraController.instance.CloseTarget(character, skillInfo.skillData.cameraTime);
                CameraController.instance.ResetMainCamera(skillInfo.skillData.cameraTime + skillInfo.skillData.cameraLength);
            }

            #endregion
            if (skillInfo.animationData.animType != AnimType.Run)
                AnimatorUtil.Play(character.anim, Animator.StringToHash(AnimatorUtil.BASE_LAYER + skillInfo.animationData.animName), 0, 0f);
            switch (skillInfo.animationData.animType)
            {
                case AnimType.Root:
                case AnimType.Trace:
                    PlayCasterEffect(layer);
                    break;
            }

            #region 远程技能特效
            if (skillInfo.animationData.animType == AnimType.Root)
            {
                #region aoe
                //aoe 与 单体互斥
                if (skillInfo.skillData.aoeFlyEffects.Length > 0)
                {
                    #region aeo fly  只有表现，没有效果
                    List<float> timelineKeys = skillInfo.skillData.timeline.GetKeys();
                    for (int j = 0, count = timelineList.Count; j < count; j++)
                    {
                        Dictionary<uint, List<KeyValuePair<uint, uint>>> mechanicsDic = timelineList[j];
                        List<uint> mechanicsKeys = mechanicsDic.GetKeys();
                        uint effectId = 0;
                        if (skillInfo.skillData.aoeFlyEffects.Length > j)
                            effectId = skillInfo.skillData.aoeFlyEffects[j];
                        for (int k = 0, kCount = mechanicsKeys.Count; k < kCount; k++)
                        {
                            uint mechanicsId = mechanicsKeys[k];
                            mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsId);
                            List<KeyValuePair<uint, uint>> tids;
                            if (mechanicsData.mechanicsType == MechanicsType.Reborn)
                            {
                                tids = CharacterUtil.FindDeadTargets(mechanicsData.rangeType, mechanicsData.targetType, isPlayer);
                                if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                                    tids.AddRange(CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer));
                            }
                            else
                            {
                                if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                                    tids = CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer);
                                else
                                    tids = mechanicsDic[mechanicsId];
                            }
                            float delay = 0f;
                            if (GameSetting.instance.effectable)
                            {
                                EffectInfo effectInfo = new EffectInfo(effectId);
                                if (effectInfo.effectData != null)
                                {
                                    effectInfo.character = character;
                                    effectInfo.target = character;
                                    effectInfo.skillInfo = skillInfo;
                                    effectInfo.delay = timelineKeys[j] + effectInfo.effectData.delay;
                                    switch (effectInfo.effectData.effectType)
                                    {
                                        case EffectType.Trace:
                                        case EffectType.CurveTrace://感觉跟直线轨迹是一样的
                                            uint positionId = GetTargetAreaPositionId(mechanicsData, tids, isPlayer);
                                            PositionData positionData = PositionData.GetPostionDataById(positionId);
                                            effectInfo.rotateAngles = skillInfo.rotateAngles; //FightController.instance.Rotate(character.transform, positionData.position);
                                            Vector3 offset = effectInfo.effectData.offset;
                                            if (!isPlayer)
                                                offset.x *= -1;
                                            effectInfo.pos = PositionData.GetPostionDataById(character.positionId).position + offset;
                                            float positionRow = PositionData.GetEnemyPositionLevels(character.positionId, positionId);
                                            effectInfo.time = effectInfo.effectData.moveTime * (positionRow * GameConfig.timePercent + 1f);
                                            Vector3 endOffset = effectInfo.effectData.endOffset;
                                            if (!isPlayer)
                                                endOffset.x *= -1;
                                            effectInfo.endPos = positionData.position + endOffset;
                                            delay = effectInfo.effectData.delay + effectInfo.time;
                                            EffectController.instance.PlayEffect(effectInfo, layer);
                                            break;
                                    }
                                }
                            }
                            PlayAOEEffects(delay, layer);//aoe飞行特效只计算特效延迟和飞行时间，不加时间点，aoe特效会计入时间点
                        }
                    }
                }
                else if (skillInfo.skillData.aoeEffects.Length > 0)
                {
                    #endregion
                    PlayAOEEffects(0f, layer);
                }

                #endregion
                #region fly effects
                else
                {
                    bool isLastTarget = false;
                    List<float> timelineKeys = skillInfo.skillData.timeline.GetKeys();
                    Dictionary<CharacterEntity, int> judgeTypeDic = new Dictionary<CharacterEntity, int>();
                    for (int j = 0, count = timelineList.Count; j < count; j++)
                    {
                        Dictionary<uint, List<KeyValuePair<uint, uint>>> mechanicsDic = timelineList[j];
                        List<uint> mechanicsKeys = mechanicsDic.GetKeys();
                        List<Triple<float, float, float>> mechanicsValues = skillInfo.skillData.mechanicsValues[j];
                        for (int k = 0, kCount = mechanicsKeys.Count; k < kCount; k++)
                        {
                            uint mechanicsId = mechanicsKeys[k];
                            mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsId);
                            List<KeyValuePair<uint, uint>> tids;
                            if (mechanicsData.mechanicsType == MechanicsType.Reborn)//获取重生目标
                            {
                                tids = CharacterUtil.FindDeadTargets(mechanicsData.rangeType, mechanicsData.targetType, isPlayer);
                                if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                                    tids.AddRange(CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer));
                            }
                            else
                            {
                                if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                                    tids = CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer);
                                else
                                    tids = mechanicsDic[mechanicsId];
                            }
                            Triple<float, float, float> mechanicsValue = mechanicsValues[k];
                            uint effectId = 0;
                            if (skillInfo.skillData.flyEffectIds.Count > j)
                                effectId = skillInfo.skillData.flyEffectIds[j];//所有目标都发射伤害特效
                            for (int m = 0, mCount = tids.Count; m < mCount; m++)
                            {
                                KeyValuePair<uint, uint> tid = tids[m];
                                if (m == mCount - 1 && k == kCount - 1 && j == count - 1)//最后一个移动特效
                                    isLastTarget = true;
                                else
                                    isLastTarget = false;
                                CharacterEntity target1 = CharacterUtil.FindTarget(mechanicsData.mechanicsType, mechanicsData.targetType, isPlayer, tid.Key);
                                CharacterEntity target2 = CharacterUtil.FindTarget(mechanicsData.mechanicsType, mechanicsData.targetType, isPlayer, tid.Value);
                                if (target1 == null) continue;
                                float delay = timelineKeys[j];

                                if (GameSetting.instance.effectable)
                                {
                                    EffectInfo effectInfo = new EffectInfo(effectId);
                                    if (effectInfo.effectData != null)
                                    {
                                        effectInfo.character = character;
                                        effectInfo.skillInfo = skillInfo;
                                        effectInfo.delay = delay + effectInfo.effectData.delay;
                                        effectInfo.isLastTarget = isLastTarget;
                                        effectInfo.target = target1;
                                        effectInfo.mechanicsData = mechanicsData;
                                        effectInfo.mechanicsValue = mechanicsValue;
                                        switch (effectInfo.effectData.effectType)
                                        {
                                            case EffectType.Root:
                                                //{
                                                //if (isPlayer != target1.isPlayer)//当目标为友方是不旋转
                                                //{
                                                //    PositionData positionData = PositionData.GetPostionDataById(target1.positionId);
                                                //    FightController.instance.Rotate(character.transform, positionData.position);
                                                //}
                                                effectInfo.rotateAngles = skillInfo.rotateAngles;
                                                //}
                                                break;
                                            case EffectType.LockTarget://锁定目标技能特效
                                                //{
                                                //    if (isPlayer != target1.isPlayer)
                                                //    {
                                                //        PositionData positionData = PositionData.GetPostionDataById(target1.positionId);
                                                //        FightController.instance.Rotate(character.transform, positionData.position);
                                                //    }
                                                //}
                                                effectInfo.rotateAngles = skillInfo.rotateAngles;
                                                break;
                                            case EffectType.Trace:
                                            case EffectType.CurveTrace://感觉跟直线轨迹是一样的
                                                {
                                                    PositionData positionData = PositionData.GetPostionDataById(target1.positionId);
                                                    //effectInfo.rotateAngles = FightController.instance.Rotate(character.transform, positionData.position);
                                                    effectInfo.rotateAngles = skillInfo.rotateAngles;
                                                    Vector3 offset = effectInfo.effectData.offset;
                                                    if (!isPlayer)
                                                        offset.x *= -1;
                                                    effectInfo.pos = PositionData.GetPostionDataById(character.positionId).position + offset;
                                                    float positionRow = PositionData.GetEnemyPositionLevels(character.positionId, target1.positionId);
                                                    effectInfo.time = effectInfo.effectData.moveTime * (positionRow * GameConfig.timePercent + 1f);
                                                    Vector3 endOffset = effectInfo.effectData.endOffset;
                                                    if (!isPlayer)
                                                        endOffset.x *= -1;
                                                    effectInfo.endPos = positionData.position + endOffset + new Vector3(0, target1.height * 0.6f, 0);
                                                }
                                                break;
                                        }
                                        delay += effectInfo.effectData.delay + effectInfo.time;
                                        EffectController.instance.PlayEffect(effectInfo, layer);
                                    }
                                }
                                if (FightController.instance.fightStatus == FightStatus.Normal)//矫正因暂停导致时间线推迟
                                    delay -= skillInfo.skillData.pauseTime;
                                int judgeType = 0;
                                if (!judgeTypeDic.ContainsKey(target1))
                                {
                                    judgeType = JudgeUtil.GetJudgeResult(character, target1, target2, (int)skillInfo.skillData.skillId);
                                    judgeTypeDic[target1] = judgeType;
                                    if (judgeType == 0)
                                        EffectController.instance.RemoveEffectByPrefabName(EffectController.SPEED_LINE);
                                }
                                else
                                    judgeType = judgeTypeDic[target1];
                                if (isPlayer)
                                    MechanicsController.instance.PlayerMechanics(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, delay, judgeType, isLastTarget);
                                else
                                    MechanicsController.instance.EnemyMechanics(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, delay, judgeType, isLastTarget);
                            }
                        }
                    }
                    judgeTypeDic.Clear();
                    judgeTypeDic = null;
                }
                #endregion
            }
            for (int i = 0, count = skillInfo.skillData.audioIds.Count; i < count; i++)
            {
                AudioData audioData = AudioData.GetAudioDataById(skillInfo.skillData.audioIds[i]);
                if (audioData != null)
                    AudioController.instance.PlayAudio(audioData.audioName, !audioData.accelerate, skillInfo.skillData.audioDelay);
            }

            Hero.Model.HeroData heroData = Hero.Model.HeroData.GetHeroDataByID((int)character.characterInfo.baseId);
            AudioData attackAudioData = AudioData.GetAudioDataById(heroData.audioAttack);
            if (attackAudioData != null)
                AudioController.instance.PlayAudio(attackAudioData.audioName, !attackAudioData.accelerate);
            #endregion
            finish = true;
        }

        private void SwitchCharactersLayer(int layer, bool showHPBarView)
        {
            if (character.isPlayer)
            {
                PlayerController.instance.ShowHPBarViews(showHPBarView);
                PlayerController.instance.SwitchHeros(layer, character as HeroEntity);
            }
            else
            {
                EnemyController.instance.SwitchEnemies(layer, character as EnemyEntity);
                EnemyController.instance.ShowHPBarViews(showHPBarView);
            }
        }

        #region 施法特效(释放者特效，不附带任何效果，仅做表现)
        public void PlayCasterEffect(int layer)
        {
            if (!GameSetting.instance.effectable) return;
            #region black screen
            if (skillInfo.animationData.blackScreen > 0)
            {
                EffectInfo effectInfo = new EffectInfo(skillInfo.animationData.blackScreen);
                if (effectInfo.effectData != null)
                {
                    effectInfo.skillInfo = skillInfo;
                    effectInfo.target = character;
                    effectInfo.delay = skillInfo.skillData.blackTime;
                    effectInfo.length = skillInfo.skillData.blackLength;

                    //switch (skillInfo.animationData.animType)
                    //{
                    //    case AnimType.Root:
                    //        effectInfo.length = skillInfo.skillData.timeline.First().Key;// skillInfo.animationData.length - skillInfo.skillData.pauseTime;
                    //        break;
                    //    case AnimType.Trace:
                    //        effectInfo.length = skillInfo.animationData.moveTime + 0.3f;
                    //        break;
                    //}
                    EffectController.instance.PlayEffect(effectInfo, (int)Logic.Enums.LayerType.Scene);
                }
            }
            if (skillInfo.animationData.bgEffect > 0)
            {
                EffectInfo effectInfoBgEffect = new EffectInfo(skillInfo.animationData.bgEffect);
                if (effectInfoBgEffect.effectData != null)
                {
                    effectInfoBgEffect.skillInfo = skillInfo;
                    effectInfoBgEffect.target = character;
                    effectInfoBgEffect.delay = skillInfo.skillData.blackTime;
                    effectInfoBgEffect.length = skillInfo.skillData.blackLength;
                    EffectController.instance.PlayEffect(effectInfoBgEffect, (int)Logic.Enums.LayerType.Scene);
                }
            }
            #endregion
            #region full screen
            if (skillInfo.animationData.fullScreen > 0 && isPlayer)
            {
                EffectInfo effectInfo = new EffectInfo(skillInfo.animationData.fullScreen);
                if (effectInfo.effectData != null)
                {
                    effectInfo.delay = skillInfo.skillData.fullScreenTime;// +effectInfo.effectData.delay;
                    effectInfo.target = character;
                    Vector3 pos = PositionData.GetPos((int)FormationPosition.Player_Position_2);
                    effectInfo.pos = pos;// effectInfo.effectData.offset + new Vector3(0, 0, pos.z);
                    effectInfo.length = skillInfo.skillData.fullScreenLength;
                    //effectInfo.length = skillInfo.animationData.closeupOverTime;
                    EffectController.instance.PlayEffect(effectInfo, layer);
                }
            }
            #endregion
            MechanicsData mechanicsData = MechanicsData.GetMechanicsDataById(timelineList.First().First().Key);//第一个作用效果
            for (int i = 0, count = skillInfo.skillData.effectIds.Length; i < count; i++)
            {
                uint effectId = skillInfo.skillData.effectIds[i];
                EffectInfo effectInfo = new EffectInfo(effectId);
                if (effectInfo.effectData == null) continue;
                effectInfo.skillInfo = skillInfo;
                effectInfo.character = character;
                switch (effectInfo.effectData.effectType)
                {
                    case EffectType.Root:
                        {
                            effectInfo.delay = skillInfo.effectDelay + effectInfo.effectData.delay;
                            Vector3 offset = effectInfo.effectData.offset;
                            if (!isPlayer)
                                offset.x *= -1;
                            effectInfo.pos = PositionData.GetPostionDataById(character.positionId).position + offset;
                            effectInfo.target = character;
                        }
                        break;
                    case EffectType.LockPart:
                        effectInfo.delay = skillInfo.effectDelay + effectInfo.effectData.delay;
                        effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, character.transform);
                        effectInfo.target = character;
                        break;
                    case EffectType.LockTarget:
                        if (effectInfo.effectData.isRotate)
                            effectInfo.rotateAngles = skillInfo.rotateAngles;
                        effectInfo.delay = skillInfo.effectDelay + effectInfo.effectData.delay;
                        effectInfo.target = character;
                        break;
                    case EffectType.TargetArea:
                        uint positionId = GetTargetAreaPositionId(mechanicsData, timelineList.First().First().Value, isPlayer);
                        effectInfo.pos = PositionData.GetPos(positionId);
                        effectInfo.target = character;
                        effectInfo.delay = effectInfo.effectData.delay;
                        break;
                    case EffectType.MoveTargetPos:
                        {
                            float moveCost = (skillInfo.animationData.hitTime - skillInfo.animationData.moveTime);
                            float moveDelay = moveCost * positionRow * GameConfig.timePercent;
                            effectInfo.delay = skillInfo.effectDelay + effectInfo.effectData.delay + moveDelay;
                            effectInfo.character = character;
                            effectInfo.target = character;
                            Vector3 offset = effectInfo.effectData.offset;
                            if (!isPlayer)
                                offset.x *= -1;
                            effectInfo.pos = endPos + offset;
                        }
                        break;
                    case EffectType.FullScreen:
                        if (!isPlayer) continue;
                        effectInfo.delay = skillInfo.effectDelay + effectInfo.effectData.delay;
                        effectInfo.target = character;
                        Vector3 pos = PositionData.GetPos((int)FormationPosition.Player_Position_2);
                        effectInfo.pos = effectInfo.effectData.offset + new Vector3(0, 0, pos.z);
                        break;
                    case EffectType.BlackScreen:
                    case EffectType.ShakeScreen:
                        effectInfo.delay = effectInfo.effectData.delay;
                        break;
                }
                EffectController.instance.PlayEffect(effectInfo, layer);
            }
        }
        #endregion

        private void PlayAOEEffects(float lastDelay, int layer)
        {
            #region aoe
            MechanicsData mechanicsData = null;
            List<float> timelineKeys = skillInfo.skillData.timeline.GetKeys();
            Dictionary<CharacterEntity, int> judgeTypeDic = new Dictionary<CharacterEntity, int>();
            for (int j = 0, count = timelineList.Count; j < count; j++)
            {
                Dictionary<uint, List<KeyValuePair<uint, uint>>> mechanicsDic = timelineList[j];
                List<uint> mechanicsKeys = mechanicsDic.GetKeys();
                List<Triple<float, float, float>> mechanicsValues = skillInfo.skillData.mechanicsValues[j];
                uint effectId = 0;
                if (skillInfo.skillData.aoeEffects.Length > j)
                    effectId = skillInfo.skillData.aoeEffects[j];
                for (int k = 0, kCount = mechanicsKeys.Count; k < kCount; k++)
                {
                    uint mechanicsId = mechanicsKeys[k];
                    mechanicsData = MechanicsData.GetMechanicsDataById(mechanicsId);
                    List<KeyValuePair<uint, uint>> tids;
                    if (mechanicsData.mechanicsType == MechanicsType.Reborn)
                    {
                        tids = CharacterUtil.FindDeadTargets(mechanicsData.rangeType, mechanicsData.targetType, isPlayer);
                        if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                            tids.AddRange(CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer));
                    }
                    else
                    {
                        if (mechanicsData.rangeType == RangeType.All || mechanicsData.rangeType == RangeType.AllAbsolutely)
                            tids = CharacterUtil.FindAllTargets(mechanicsData.targetType, isPlayer);
                        else
                            tids = mechanicsDic[mechanicsId];
                    }
                    Triple<float, float, float> mechanicsValue = mechanicsValues[k];
                    float delay = timelineKeys[j] + lastDelay;
                    if (GameSetting.instance.effectable)
                    {
                        EffectInfo effectInfo = new EffectInfo(effectId);
                        if (effectInfo.effectData != null)
                        {
                            effectInfo.character = character;
                            effectInfo.target = character;
                            effectInfo.skillInfo = skillInfo;
                            effectInfo.delay = delay + effectInfo.effectData.delay;
                            effectInfo.mechanicsData = mechanicsData;
                            effectInfo.mechanicsValue = mechanicsValue;

                            switch (effectInfo.effectData.effectType)
                            {
                                case EffectType.TargetArea://根据范围信息取位置   
                                    uint positionId = GetTargetAreaPositionId(mechanicsData, tids, isPlayer);
                                    effectInfo.pos = PositionData.GetPos(positionId);
                                    delay += effectInfo.effectData.delay;
                                    break;
                            }
                            EffectController.instance.PlayEffect(effectInfo, layer);
                        }
                    }
                    bool isLastTarget = false;
                    if (FightController.instance.fightStatus == FightStatus.Normal)//矫正因暂停导致时间线推迟
                        delay -= skillInfo.skillData.pauseTime;
                    for (int i = 0, iCount = tids.Count; i < iCount; i++)
                    {
                        if (i == iCount - 1)
                            isLastTarget = true;
                        CharacterEntity target1 = CharacterUtil.FindTarget(mechanicsData.mechanicsType, mechanicsData.targetType, isPlayer, tids[i].Key);
                        CharacterEntity target2 = CharacterUtil.FindTarget(mechanicsData.mechanicsType, mechanicsData.targetType, isPlayer, tids[i].Value);
                        if (!target1) continue;
                        int judgeType = 0;
                        if (!judgeTypeDic.ContainsKey(target1))
                        {
                            judgeType = JudgeUtil.GetJudgeResult(character, target1, target2, (int)skillInfo.skillData.skillId);
                            judgeTypeDic[target1] = judgeType;
                            if (judgeType == 0)
                                EffectController.instance.RemoveEffectByPrefabName(EffectController.SPEED_LINE);
                        }
                        else
                            judgeType = judgeTypeDic[target1];
                        if (isPlayer)
                            MechanicsController.instance.PlayerMechanics(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, delay, judgeType, isLastTarget);
                        else
                            MechanicsController.instance.EnemyMechanics(skillInfo, mechanicsData, character, target1, target2, mechanicsValue, delay, judgeType, isLastTarget);
                    }
                }
            }
            judgeTypeDic.Clear();
            judgeTypeDic = null;
            #endregion
        }

        private uint GetTargetAreaPositionId(MechanicsData mechanicsData, List<KeyValuePair<uint, uint>> tids, bool isPlayer)
        {
            uint positionId = 0;
            switch (mechanicsData.rangeType)
            {
                case RangeType.CurrentSingle:
                case RangeType.CurrentRow:
                case RangeType.CurrentColumn:
                case RangeType.All:
                case RangeType.CurrentAndBehindFirst:
                case RangeType.CurrentAndNearCross:
                case RangeType.CurrentBehindFirst:
                case RangeType.CurrentBehindSecond:
                case RangeType.CurrentIntervalOne:
                case RangeType.CurrentAndRandomTwo:
                case RangeType.Weakness:
                case RangeType.LowestHP:
                case RangeType.RandomSingle:
                case RangeType.CurrentAndBehindTowColum:
                    CharacterEntity firstTarget = CharacterUtil.FindTarget(mechanicsData.mechanicsType, mechanicsData.targetType, isPlayer, tids.First().Key);
                    positionId = firstTarget.positionId;
                    break;
                case RangeType.RandomN:
                case RangeType.Cross:
                case RangeType.SecondColum:
                case RangeType.ExceptMidpoint:
                case RangeType.Midpoint:
                case RangeType.AllAbsolutely:
                case  RangeType.BehindTowColum:
                    switch (mechanicsData.targetType)
                    {
                        case TargetType.Ally:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Player_Position_5;
                            else
                                positionId = (uint)FormationPosition.Enemy_Position_5;
                            break;
                        case TargetType.Enemy:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Enemy_Position_5;
                            else
                                positionId = (uint)FormationPosition.Player_Position_5;
                            break;
                    }
                    break;
                case RangeType.FirstColum:
                case RangeType.SecondRow:
                    switch (mechanicsData.targetType)
                    {
                        case TargetType.Ally:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Player_Position_2;
                            else
                                positionId = (uint)FormationPosition.Enemy_Position_2;
                            break;
                        case TargetType.Enemy:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Enemy_Position_2;
                            else
                                positionId = (uint)FormationPosition.Player_Position_2;
                            break;
                    }
                    break;
                case RangeType.ThirdColum:
                    switch (mechanicsData.targetType)
                    {
                        case TargetType.Ally:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Player_Position_7;
                            else
                                positionId = (uint)FormationPosition.Enemy_Position_7;
                            break;
                        case TargetType.Enemy:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Enemy_Position_7;
                            else
                                positionId = (uint)FormationPosition.Player_Position_7;
                            break;
                    }
                    break;
                case RangeType.FirstRow:
                case RangeType.LeadingDiagonal:
                    switch (mechanicsData.targetType)
                    {
                        case TargetType.Ally:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Player_Position_1;
                            else
                                positionId = (uint)FormationPosition.Enemy_Position_1;
                            break;
                        case TargetType.Enemy:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Enemy_Position_1;
                            else
                                positionId = (uint)FormationPosition.Player_Position_1;
                            break;
                    }
                    break;
                case RangeType.ThirdRow:
                case RangeType.SecondaryDiagonal:
                    switch (mechanicsData.targetType)
                    {
                        case TargetType.Ally:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Player_Position_3;
                            else
                                positionId = (uint)FormationPosition.Enemy_Position_3;
                            break;
                        case TargetType.Enemy:
                            if (isPlayer)
                                positionId = (uint)FormationPosition.Enemy_Position_3;
                            else
                                positionId = (uint)FormationPosition.Player_Position_3;
                            break;
                    }
                    break;
            }
            return positionId;
        }
    }
}