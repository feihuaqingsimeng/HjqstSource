using Logic.Enums;
using Logic.Net.Controller;
using PathologicalGames;
using UnityEngine;
namespace Logic.Character
{
    public class EnemyEntity : CharacterEntity
    {
        //[HideInInspector]
        //public HPBarView hpBarView;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }
        
        public override bool canOrderSkill1
        {
            get
            {
                if (characterInfo.skillInfo1 == null || isDead) return false;
                return _canOrderSkill1 && skill1CD >= characterInfo.skillInfo1.skillData.CD * Const.Model.ConstData.GetConstData().aiSkillRound && !controled && !Silence;
            }
            set
            {
                _canOrderSkill1 = value;
            }
        }

        public override bool canOrderSkill2
        {
            get
            {
                if (characterInfo.skillInfo2 == null || isDead) return false;
				return _canOrderSkill2 && skill2CD >= characterInfo.skillInfo2.skillData.CD * Const.Model.ConstData.GetConstData().aiSkillRound && !controled && !Silence;
            }
            set
            {
                _canOrderSkill2 = value;
            }
        }

        public override bool canPlaySkill1
        {
            get
            {
                if (characterInfo.skillInfo1 == null || isDead) return false;
                if (!canPlayAnimator || Swimmy) return false;
#if UNITY_EDITOR
                if (VirtualServerController.instance.fightEidtor)
                    return status == Status.Idle && isInPlace;
#endif
				return skill1CD >= characterInfo.skillInfo1.skillData.CD * Const.Model.ConstData.GetConstData().aiSkillRound && (status == Status.Idle || status == Status.BootSkill) && isInPlace && !controled && !Silence;
            }
        }

        public override bool canPlaySkill2
        {
            get
            {
                if (characterInfo.skillInfo2 == null || isDead) return false;
                if (!canPlayAnimator || Swimmy) return false;
#if UNITY_EDITOR
                if (VirtualServerController.instance.fightEidtor)
                    return status == Status.Idle && isInPlace;
#endif
				return skill2CD >= characterInfo.skillInfo2.skillData.CD * Const.Model.ConstData.GetConstData().aiSkillRound && (status == Status.Idle || status == Status.BootSkill) && isInPlace && !controled && !Silence;
            }
        }

        public void OnHPChangedHandler(int hp)
        {
            HP = hp;
            if (HP <= 0)
                isDead = true;
            if (hpBarView)
                hpBarView.UpdateHPValue(true);
        }

        public void OnForceDeadHandler()
        {
            isDead = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}