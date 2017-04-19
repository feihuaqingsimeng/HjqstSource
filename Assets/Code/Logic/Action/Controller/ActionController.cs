using UnityEngine;
using System.Collections;
using Logic.Character;
using Common.Animators;
namespace Logic.Action.Controller
{
    public class ActionController : SingletonMono<ActionController>
    {
        void Awake()
        {
            instance = this;
        }

        public void PlayerAnimAction(CharacterEntity character, int stateNameHash)
        {
            if (character)
            {
                AnimAction animAction = new AnimAction();
                animAction.character = character;
                animAction.stateNameHash = stateNameHash;
                animAction.Execute();
            }
        }

        public void SetAnimBoolean(CharacterEntity character, string argName, bool value)
        {
            AnimatorUtil.SetBool(character.anim, argName, value);
        }

        public void PlayerAnimAction(CharacterEntity character, int stateNameHash, float normalizedTime)
        {
            if (character)
            {
                AnimAction animAction = new AnimAction();
                animAction.character = character;
                animAction.stateNameHash = stateNameHash;
                animAction.isCrossFade = false;
                animAction.normalizedTime = normalizedTime;
                animAction.Execute();
            }
        }

        public void MoveTarget(CharacterEntity character, Vector3 endPos, float moveTime)
        {
            if (character)
            {
                MoveAction moveAction = new MoveAction();
                moveAction.character = character;
                moveAction.endPos = endPos;
                moveAction.moveTime = moveTime;
                moveAction.stateNameHash = Common.Animators.AnimatorUtil.RUN_ID;
                moveAction.Execute();
            }
        }
    }
}