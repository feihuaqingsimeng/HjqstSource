using PathologicalGames;
using UnityEngine;
namespace Logic.Character
{
    public class EnemyPlayerEntity : EnemyEntity
    {
        [HideInInspector]
        public PetEntity petEntity;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            isRole = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (petEntity != null)
                petEntity.gameObject.SetActive(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (petEntity != null)
                petEntity.gameObject.SetActive(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}