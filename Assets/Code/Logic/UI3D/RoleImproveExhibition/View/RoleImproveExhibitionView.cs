using UnityEngine;
using Logic.Role.Model;
using Common.Util;
using Logic.Player.Model;
using Logic.Character;
using Logic.Hero.Model;
using Logic.Shaders;
using Common.Animators;

namespace Logic.UI3D.RoleImproveExhibition.View
{
    public class RoleImproveExhibitionView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui_3d/role_improve_exhibition/role_improve_exhibition_view";

        public enum ExhibitionType
        {
            Breakthrough = 1,
            Strengthen = 2,
            Advance = 3,
        }

        public delegate void OnCloseUpEndDelegate();
        public OnCloseUpEndDelegate onCloseUpEndDelegate;

        public Transform roleModelRoot;
        private CharacterEntity _oldCharacterEnyity;
        private CharacterEntity _newCharacterEntity;

        public void SetRoleInfo(RoleInfo roleInfo, ExhibitionType exhibitionType)
        {
            RoleInfo oldRoleInfo = roleInfo;
            RoleInfo newRoleInfo = roleInfo;
            if (exhibitionType == ExhibitionType.Advance)
            {
                if (roleInfo is PlayerInfo)
                {
                    oldRoleInfo = (roleInfo as PlayerInfo).GetPlayerInfoCopy();
                    oldRoleInfo.advanceLevel -= 1;
                }
                else if (roleInfo is HeroInfo)
                {
                    oldRoleInfo = (roleInfo as HeroInfo).GetHeroInfoCopy();
                    oldRoleInfo.advanceLevel -= 1;
                }
            }
            DespawnCharacter();
            TransformUtil.ClearChildren(roleModelRoot, true);
            if (roleInfo is PlayerInfo)
            {
                _oldCharacterEnyity = PlayerEntity.CreatePlayerEntityAs3DUIElement(oldRoleInfo as PlayerInfo, roleModelRoot, false, false);
                _newCharacterEntity = PlayerEntity.CreatePlayerEntityAs3DUIElement(newRoleInfo as PlayerInfo, roleModelRoot, false, false);
            }
            else if (roleInfo is HeroInfo)
            {
                _oldCharacterEnyity = HeroEntity.CreateHeroEntityAs3DUIElement(oldRoleInfo as HeroInfo, roleModelRoot, false, false);
                _newCharacterEntity = HeroEntity.CreateHeroEntityAs3DUIElement(newRoleInfo as HeroInfo, roleModelRoot, false, false);
            }
            ShadersUtil.SetShaderKeyword(_oldCharacterEnyity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetMainColor(_oldCharacterEnyity, ShadersUtil.MAIN_COLOR);
            ShadersUtil.SetShaderKeyword(_newCharacterEntity, ShadersUtil.RIMLIGHT_OFF, ShadersUtil.RIMLIGHT_ON);
            ShadersUtil.SetMainColor(_newCharacterEntity, ShadersUtil.MAIN_COLOR);

            _oldCharacterEnyity.gameObject.SetActive(true);
            _newCharacterEntity.gameObject.SetActive(false);
        }

        private void DespawnCharacter()
        {
            if (_oldCharacterEnyity)
                Pool.Controller.PoolController.instance.Despawn(_oldCharacterEnyity.name, _oldCharacterEnyity);
            _oldCharacterEnyity = null;
            if (_newCharacterEntity)
            {
                _newCharacterEntity.gameObject.SetActive(true);
                Pool.Controller.PoolController.instance.Despawn(_newCharacterEntity.name, _newCharacterEntity);
            }
            _newCharacterEntity = null;
        }

        void OnDestroy()
        {
            DespawnCharacter();
        }

        public void OnShouldReplaceRoleModel()
        {
            _oldCharacterEnyity.gameObject.SetActive(false);
            _newCharacterEntity.gameObject.SetActive(true);
            Action.Controller.ActionController.instance.PlayerAnimAction(_newCharacterEntity, AnimatorUtil.VICOTRY_ID);
        }

        public void OnCloseUpEnd()
        {
            if (onCloseUpEndDelegate != null)
                onCloseUpEndDelegate();
        }

        public void OnAnimationEnd()
        {
            Logic.Model.View.ModelRotateAndAnim modelRotateAndAnim = _newCharacterEntity.GetComponent<Logic.Model.View.ModelRotateAndAnim>();
            if (modelRotateAndAnim != null)
            {
                modelRotateAndAnim.canClick = true;
                modelRotateAndAnim.canDrag = true;
            }
        }
    }
}