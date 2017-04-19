using UnityEngine;
using System.Collections;
using Logic.Character;
using Common.Animators;
namespace Logic.Model.View
{
    public class ModelBehaviour : MonoBehaviour
    {
        public float defaultSpeed = 20;
        private bool _canRotate = false;
        public int stateNameHash = AnimatorUtil.VICOTRY_ID;

        private CharacterEntity _chatacterEntity;
        public CharacterEntity CharacterEntity
        {
            get
            {
                if (_chatacterEntity == null)
                {
                    _chatacterEntity = GetComponent<CharacterEntity>();
                }
                return _chatacterEntity;
            }
        }

        void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                defaultSpeed = 100;
            }
        }

        public bool canRotate
        {
            set { _canRotate = value; }
        }
        public virtual void ClickBehavior()
        {
            Action.Controller.ActionController.instance.PlayerAnimAction(this.CharacterEntity, stateNameHash);
            if (CharacterEntity is PlayerEntity)
            {
                PlayerEntity playerEntity = CharacterEntity as PlayerEntity;
                Action.Controller.ActionController.instance.PlayerAnimAction(playerEntity.petEntity, stateNameHash);
            }
        }

        public virtual void Rotate(float fac)
        {
            if (_canRotate)
                this.transform.Rotate(Vector3.up, -defaultSpeed * fac * Time.deltaTime);
        }

    }
}
