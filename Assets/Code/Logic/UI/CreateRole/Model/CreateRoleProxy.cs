using UnityEngine;
using System.Collections;

namespace Logic.UI.CreateRole.Model
{
    public class CreateRoleProxy : SingletonMono<CreateRoleProxy>
    {
        public const int HAIR_COLOR_ITEM_PER_GROUP_COUNT = 4;

        public uint selectedPlayerID;
        public uint selectedHairCutIndex;
        public uint selectedHairColorIndex;
        private int _selectedSkinIndex;
        public int selectedSkinIndex
        {
            get
            {
                return _selectedSkinIndex;
            }
            set
            {
                Logic.Player.Model.PlayerData playerData = Logic.Player.Model.PlayerData.GetPlayerData(CreateRoleProxy.instance.selectedPlayerID);
                Logic.Avatar.Model.AvatarData avatarData = Avatar.Model.AvatarData.GetAvatarData(playerData.avatarID);
                int length = avatarData.skinPaths.Length;
                if (value < 0)
                    _selectedSkinIndex = length - 1;
                else if (value >= length)
                    _selectedSkinIndex = 0;
                else
                    _selectedSkinIndex = value;
            }
        }
        public uint HairColorRealIndex
        {
            get
            {
                return selectedHairCutIndex * HAIR_COLOR_ITEM_PER_GROUP_COUNT + selectedHairColorIndex;
            }
        }
        public uint selectedFaceIndex;

        public delegate void OnHairCutChangedHandler(uint selectedHairCutIndex, uint hairColorRealIndex);
        public delegate void OnHairColorChangedHandler(uint selectedHairCutIndex, uint selectedHairColorIndex);
        public delegate void OnFaceChangedHandler(uint selectedFaceIndex);
        public delegate void OnSkinChangedHandler(int selectedSkinIndex);

        public OnHairCutChangedHandler onHairCutChangedHandler;
        public OnHairColorChangedHandler onHairColorChangedHandler;
        public OnFaceChangedHandler onFaceChangedHandler;
        public OnSkinChangedHandler onSkinChangedHandler;

        void Awake()
        {
            instance = this;
        }
    }
}
