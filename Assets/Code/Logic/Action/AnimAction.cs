using UnityEngine;
using System.Collections;
using Common.Animators;
namespace Logic.Action
{
    public class AnimAction : AIAction
    {
        public int stateNameHash;
        public bool isCrossFade = true;
        public float crossFadeTime = 0.3f;
        public float normalizedTime = 0f;
        public override void Execute()
        {
            //Debugger.LogError(animName);
            if (isCrossFade)
                AnimatorUtil.CrossFade(character.anim, stateNameHash, crossFadeTime);
            else
                AnimatorUtil.Play(character.anim, stateNameHash, 0, normalizedTime);
            //actioner.anim.SetTrigger(animName);
            finish = true;
        }
    }
}