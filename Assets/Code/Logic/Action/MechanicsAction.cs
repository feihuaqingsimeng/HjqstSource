using UnityEngine;
using System.Collections;
using Logic.Skill.Model;
using Logic.Effect.Model;
using Logic.Enums;
using Logic.Effect.Controller;
using Logic.Character;
using Logic.Position.Model;
using Common.Animators;
using Common.Util;
using Logic.Fight.Controller;
using Logic.Audio.Controller;
using System.Collections.Generic;
using Logic.Audio.Model;
using Logic.Character.Controller;

namespace Logic.Action
{
    public class MechanicsAction : AIAction
    {
        public MechanicsData mechanicsData;
        public SkillInfo skillInfo;
        public CharacterEntity mechanicsMaker;
        public MechanicsType mechanicsType = MechanicsType.None;
        public override void Execute()
        {
            if (!character || mechanicsData == null || skillInfo == null) return;

            int layer = (int)LayerType.Fight;
            int stateNameHash = 0;
            AnimatorPlayType animatorPlayType = AnimatorPlayType.None;
            if (mechanicsType == MechanicsType.None)
                mechanicsType = mechanicsData.mechanicsType;
            switch (mechanicsType)
            {
                //攻击
                case MechanicsType.Damage:
                case MechanicsType.DrainDamage:
                case MechanicsType.IgnoreDefenseDamage:
                case MechanicsType.ImmediatePercentDamage:
                case MechanicsType.SwimmyExtraDamage:
                case MechanicsType.LandificationExtraDamage:
                case MechanicsType.BleedExtraDamage:
                case MechanicsType.FrozenExtraDamage:
                case MechanicsType.PoisoningExtraDamage:
                case MechanicsType.TagExtraDamage:
                case MechanicsType.IgniteExtraDamage:
                    if ((character.status == Status.BootSkill && breakable) || character.status != Status.BootSkill)//非引导技能，或者引导技能可打断
                    {
                        animatorPlayType = AnimatorPlayType.Play;
                        switch (FightController.instance.fightType)
                        {
                            case FightType.PVE:
                            case FightType.Arena:
                            case FightType.DailyPVE:
                            case FightType.Expedition:
                            case FightType.WorldTree:
                            case FightType.WorldBoss:
                            case FightType.FirstFight:
                            case FightType.SkillDisplay:
                            case FightType.MineFight:
#if UNITY_EDITOR
                            case FightType.Imitate:
#endif
                                if (Logic.Game.GameSetting.instance.closeupCameraable)
                                {
                                    if (FightController.instance.fightStatus == FightStatus.Normal)
                                    {
                                        if (skillInfo.animationData.closeup)
                                        {
                                            if (mechanicsMaker.isPlayer || (!mechanicsMaker.isPlayer && Logic.Character.Controller.EnemyController.instance.isBoss(mechanicsMaker.characterInfo.instanceID)))//只有玩家和boss有特写镜头
                                            {
                                                layer = (int)LayerType.Closeup;
                                            }
                                        }
                                    }
                                }
                                break;
                            case FightType.PVP:
                            case FightType.FriendFight:
                            case FightType.ConsortiaFight:
                                break;
                        }
                        switch (skillInfo.skillData.skillType)
                        {
                            case SkillType.Hit:
                                if (Fight.Controller.FightController.instance.fightStatus == FightStatus.Normal)
                                    stateNameHash = AnimatorUtil.GETHIT_1_ID;
                                break;
                            case SkillType.Skill:
                            case SkillType.Aeon:
                                switch (Fight.Controller.FightController.instance.fightStatus)
                                {
                                    case FightStatus.FloatComboing:
                                        character.anim.speed = Game.GameSetting.instance.speed;
                                        stateNameHash = AnimatorUtil.FLOATGETHIT_ID;
                                        Cameras.Controller.CameraController.instance.Shake();
                                        break;
                                    case FightStatus.TumbleComboing:
                                        character.anim.speed = Game.GameSetting.instance.speed;
                                        stateNameHash = AnimatorUtil.TUMBLEGETHIT_ID;
                                        Cameras.Controller.CameraController.instance.Shake();
                                        break;
                                    case FightStatus.Normal:
                                        stateNameHash = AnimatorUtil.GETHIT_2_ID;
                                        break;
                                }
                                break;
                        }
                        character.SetStatus(Status.GetHit);
                    }
                    break;
                case MechanicsType.Float:
                    animatorPlayType = AnimatorPlayType.Play;
                    if (AnimatorUtil.GetBool(character.anim, AnimatorUtil.FLOATING) || AnimatorUtil.GetBool(character.anim, AnimatorUtil.TUMBLE))//忽略再次连击
                    {
                        switch (Fight.Controller.FightController.instance.fightStatus)
                        {
                            case FightStatus.FloatComboing:
                                character.anim.speed = Game.GameSetting.instance.speed;
                                stateNameHash = AnimatorUtil.FLOATGETHIT_ID;
                                Cameras.Controller.CameraController.instance.Shake();
                                break;
                            case FightStatus.TumbleComboing:
                                character.anim.speed = Game.GameSetting.instance.speed;
                                stateNameHash = AnimatorUtil.TUMBLEGETHIT_ID;
                                Cameras.Controller.CameraController.instance.Shake();
                                break;
                            case FightStatus.Normal:
                                stateNameHash = AnimatorUtil.GETHIT_2_ID;
                                break;
                        }
                    }
                    else
                    {
                        if (character.floatable)
                        {
                            if (skillInfo.skillData.skillType == SkillType.Aeon)
                            {
                                stateNameHash = AnimatorUtil.FLOATGETHIT_ID;
                            }
                            else
                            {
                                stateNameHash = AnimatorUtil.FLOATSTART_ID;
                                if (mechanicsMaker.isPlayer)
                                {
                                    if (PlayerController.instance.HasFloatHeros())
                                    {
                                        layer = (int)LayerType.FightCombo;
                                        Fight.Controller.FightController.instance.SetComboCharacter(mechanicsMaker, character);
                                        AnimatorUtil.SetBool(character.anim, AnimatorUtil.FLOATING, true);
                                        Fight.Controller.FightController.instance.fightStatus = FightStatus.FloatWaiting;
                                    }
                                }
                                else
                                {
                                    if (EnemyController.instance.HasFloatEnemies())
                                    {
                                        layer = (int)LayerType.FightCombo;
                                        Fight.Controller.FightController.instance.SetComboCharacter(mechanicsMaker, character);
                                        AnimatorUtil.SetBool(character.anim, AnimatorUtil.FLOATING, true);
                                        Fight.Controller.FightController.instance.fightStatus = FightStatus.FloatWaiting;
                                    }
                                }
                            }
                        }
                        else
                        {
                            stateNameHash = AnimatorUtil.GETHIT_2_ID;
                        }
                    }
                    character.SetStatus(Status.GetHit);
                    break;
                case MechanicsType.Tumble:
                    animatorPlayType = AnimatorPlayType.Play;
                    if (AnimatorUtil.GetBool(character.anim, AnimatorUtil.TUMBLE) || AnimatorUtil.GetBool(character.anim, AnimatorUtil.FLOATING))//忽略再次连击
                    {
                        switch (Fight.Controller.FightController.instance.fightStatus)
                        {
                            case FightStatus.FloatComboing:
                                character.anim.speed = Game.GameSetting.instance.speed;
                                stateNameHash = AnimatorUtil.FLOATGETHIT_ID;
                                Cameras.Controller.CameraController.instance.Shake();
                                break;
                            case FightStatus.TumbleComboing:
                                character.anim.speed = Game.GameSetting.instance.speed;
                                stateNameHash = AnimatorUtil.TUMBLEGETHIT_ID;
                                Cameras.Controller.CameraController.instance.Shake();
                                break;
                            case FightStatus.Normal:
                                stateNameHash = AnimatorUtil.GETHIT_2_ID;
                                break;
                        }
                    }
                    else
                    {
                        if (character.floatable)
                        {
                            if (skillInfo.skillData.skillType == SkillType.Aeon)
                            {
                                stateNameHash = AnimatorUtil.TUMBLEGETHIT_ID;
                            }
                            else
                            {
                                stateNameHash = AnimatorUtil.TUMBLESTART_ID;
                                if (mechanicsMaker.isPlayer)
                                {
                                    if (PlayerController.instance.HasTumbleHeros())
                                    {
                                        layer = (int)LayerType.FightCombo;
                                        Fight.Controller.FightController.instance.SetComboCharacter(mechanicsMaker, character);
                                        AnimatorUtil.SetBool(character.anim, AnimatorUtil.TUMBLE, true);
                                        Fight.Controller.FightController.instance.fightStatus = FightStatus.TumbleWaiting;
                                    }
                                }
                                else
                                {
                                    if (EnemyController.instance.HasTumbleEnemies())
                                    {
                                        layer = (int)LayerType.FightCombo;
                                        Fight.Controller.FightController.instance.SetComboCharacter(mechanicsMaker, character);
                                        AnimatorUtil.SetBool(character.anim, AnimatorUtil.TUMBLE, true);
                                        Fight.Controller.FightController.instance.fightStatus = FightStatus.TumbleWaiting;
                                    }
                                }
                            }
                        }
                        else
                        {
                            stateNameHash = AnimatorUtil.GETHIT_2_ID;
                        }
                    }
                    character.SetStatus(Status.GetHit);
                    break;
            }

            if (stateNameHash != 0)
            {
                switch (animatorPlayType)
                {
                    case AnimatorPlayType.Play:
                        //if (Fight.Controller.FightController.instance.fightStatus == FightStatus.Normal)
                        //{
                        if (character.canPlayAnimator && character.isInPlace)
                            AnimatorUtil.Play(character.anim, stateNameHash, 0, 0f);
                        //}
                        //else
                        //    AnimatorUtil.Play(character.anim, Animator.StringToHash(AnimatorUtil.BASE_LAYER + animName), 0, 0f);
                        break;
                    case AnimatorPlayType.Trigger:
                        //if (character.canPlayAnimator && character.isInPlace)
                        //    AnimatorUtil.SetTrigger(character.anim, stateNameHash);
                        break;
                }
            }
            if (mechanicsData.audioType != 0)
            {
                List<AudioData> audios = AudioData.GetAuidoDatasByType(mechanicsData.audioType);
                if (audios.Count > 0)
                {
                    int index = Random.Range(0, audios.Count);
                    AudioData audioData = audios[index];
                    AudioController.instance.PlayAudio(audioData.audioName, !audioData.accelerate, mechanicsData.audioDelay);
                }
            }
            Hero.Model.HeroData heroData = Hero.Model.HeroData.GetHeroDataByID((int)character.characterInfo.baseId);
            AudioData attackAudioData = AudioData.GetAudioDataById(heroData.audioUnderAttack);
            if (attackAudioData != null)
                AudioController.instance.PlayAudio(attackAudioData.audioName, !attackAudioData.accelerate);
            if (Logic.Game.GameSetting.instance.effectable)
                PlayEffect(layer);
            finish = true;
        }

        private void PlayEffect(int layer)
        {
            for (int i = 0, count = mechanicsData.effectIds.Length; i < count; i++)
            {
                EffectInfo effectInfo = new EffectInfo(mechanicsData.effectIds[i]);
                if (effectInfo.effectData == null) continue;
                effectInfo.skillInfo = skillInfo;
                effectInfo.character = character;
                //Debugger.Log(effectInfo.effectData.effectType);
                switch (effectInfo.effectData.effectType)
                {
                    case EffectType.Root:
                        effectInfo.pos = PositionData.GetPostionDataById(character.positionId).position + effectInfo.effectData.offset;
                        effectInfo.target = character;
                        break;
                    case EffectType.LockTarget:
                        effectInfo.target = character;
                        break;
                    case EffectType.ChangeColor:
                        effectInfo.target = character;
                        break;
                    case EffectType.LockPart:
                        effectInfo.lockTrans = TransformUtil.Find(effectInfo.effectData.partName, character.transform);
                        effectInfo.target = character;
                        break;
                }
                effectInfo.delay = effectInfo.effectData.delay;
                EffectController.instance.PlayEffect(effectInfo, layer);
            }
        }
    }

    public enum AnimatorPlayType
    {
        None = 0,
        Play = 1,
        Trigger = 2,
    }
}
