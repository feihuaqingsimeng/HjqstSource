using Logic.Character;
using System;
namespace Logic.Action
{
    public class AIAction : IAIAction
    {
        public CharacterEntity character;
        public bool breakable = false;
        public bool finish = false;
        public System.Action OnFinishAction;
        public AIAction() { }

        public virtual void Execute() { }

        public void FinishAction()
        {
            if (OnFinishAction != null)
                OnFinishAction();
            OnFinishAction = null;
        }
    }
}

