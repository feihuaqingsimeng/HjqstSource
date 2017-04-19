using UnityEngine;
using System.Collections;
using Logic.Fight.Controller;
using Common.Animators;
namespace Logic.Action
{
    public class MoveAction : AIAction
    {
        public int stateNameHash;
        public Vector3 endPos;
        public float moveTime;
        public override void Execute()
        {
            if (!character) return;
            if (!character.anim) return;
            AnimatorUtil.CrossFade(character.anim, stateNameHash, 0.05f);
            character.anim.speed = 1.2f;
            FightController.instance.Move(character.transform, endPos, moveTime, () =>
            {
                character.anim.speed = 1f;
                AnimatorUtil.SetTrigger(character.anim, AnimatorUtil.ENDRUN);
                FinishAction();
                finish = true;
            });
        }
    }
}