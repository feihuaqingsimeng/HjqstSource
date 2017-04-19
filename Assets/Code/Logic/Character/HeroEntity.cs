using UnityEngine;
using Logic.UI.SkillBar.View;
using PathologicalGames;
namespace Logic.Character
{
    public class HeroEntity : CharacterEntity
    {
        [HideInInspector]
        public SkillItemBoxView skillItemBoxView;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            isPlayer = true;
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
