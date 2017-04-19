using UnityEngine;
using System;
using System.Collections.Generic;
using Logic.UI.CreateRole.Model;
using Logic.Protocol.Model;

namespace Logic.UI.CreateRole.Controller
{
    public class CreateRoleController : SingletonMono<CreateRoleController>
    {
		public bool isCreatingNewRole = false;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            Observers.Facade.Instance.RegisterObserver(((int)MSG.CreateRoleResp).ToString(), LOBBY2CLIENT_CREATE_ROLE_SUCCESS_Handler);
        }

        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                Observers.Facade.Instance.RemoveObserver(((int)MSG.CreateRoleResp).ToString(), LOBBY2CLIENT_CREATE_ROLE_SUCCESS_Handler);
            }
        }

        public void SelectRole(uint roleID)
        {
            CreateRoleProxy.instance.selectedPlayerID = roleID;
        }

        public void SelectHairCut(uint hairCutIndex)
        {
            CreateRoleProxy.instance.selectedHairCutIndex = hairCutIndex;
            CreateRoleProxy.instance.onHairCutChangedHandler(CreateRoleProxy.instance.selectedHairCutIndex, CreateRoleProxy.instance.HairColorRealIndex);
        }

        public void SelectHairColor(uint hairColorIndex)
        {
            CreateRoleProxy.instance.selectedHairColorIndex = hairColorIndex;
            CreateRoleProxy.instance.onHairColorChangedHandler(CreateRoleProxy.instance.selectedHairCutIndex, CreateRoleProxy.instance.HairColorRealIndex);
        }

        public void SelectFace(uint faceIndex)
        {
            CreateRoleProxy.instance.selectedFaceIndex = faceIndex;
            CreateRoleProxy.instance.onFaceChangedHandler(CreateRoleProxy.instance.selectedFaceIndex);
        }

        public void SelectSkin(int skinIndex)
        {
            CreateRoleProxy.instance.selectedSkinIndex = skinIndex;
            CreateRoleProxy.instance.onSkinChangedHandler(CreateRoleProxy.instance.selectedSkinIndex);
        }

        public void CreateRole(string name, uint playerID, uint hairCutIndex, uint hairColorIndex, uint faceIndex, int skinId)
        {
			isCreatingNewRole = true;

            CreateRoleReq createRoleReq = new CreateRoleReq();
            createRoleReq.account = Logic.UI.Login.Model.LoginProxy.instance.cachedAccount;
            createRoleReq.roleName = name;
            createRoleReq.playerId = (int)playerID;
            createRoleReq.hairCutId = (int)hairCutIndex;
            createRoleReq.hairColorId = (int)hairColorIndex;
            createRoleReq.faceId = (int)faceIndex;
            createRoleReq.skinId = skinId;
            createRoleReq.partnerId = Logic.UI.Login.Model.LoginProxy.instance.cachedPlatformId;
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(createRoleReq);

            /********** Legacy code, I'll remove them later **********/
            //			Game.Model.GameProxy.instance.PlayerInfo = new Logic.Player.Model.PlayerInfo(Game.Model.GameProxy.instance.PlayerInstanceID, playerID, hairCutIndex, hairColorIndex, faceIndex, name);
            //			UIMgr.instance.Open(UI.Main.View.MainView.PREFAB_PATH);
            /********** Legacy code, I'll remove them later **********/
        }

        public void onCreateAvatarResult(Byte retcode, object info, Dictionary<UInt64, Dictionary<string, object>> avatars)
        {
            if (retcode == 0)
            {
                Debugger.Log("CreateRoleController::onCreateAvatarResult----->Success");
                RequestAvatarList();
            }
            else
            {
                Debugger.Log("CreateRoleController::onCreateAvatarResult----->Failed");
            }
        }

        public void RequestAvatarList()
        {

        }

        private bool LOBBY2CLIENT_CREATE_ROLE_SUCCESS_Handler(Observers.Interfaces.INotification note)
        {
            Debugger.Log("Congratulations!!!!!Create role success!");
            UI.UIMgr.instance.Close(EUISortingLayer.MainUI);
            Logic.Game.Controller.GameController.instance.SendExternalData(Logic.Enums.ExtraDataType.CreateRole);
            UI.Main.View.MainView.Open();
            return true;
        }
    }
}
