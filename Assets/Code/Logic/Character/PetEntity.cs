using UnityEngine;
using System.Collections;
namespace Logic.Character
{
    public class PetEntity : CharacterEntity
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        public override Enums.Status status
        {
            get
            {
                return _status;
            }
            protected set
            {

            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}