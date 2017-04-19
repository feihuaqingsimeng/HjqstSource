using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.ResMgr;
using Common.Localization;
using Logic.Player.Model;
using Logic.UI.CreateRole.Controller;
using Logic.UI.CreateRole.Model;

namespace Logic.UI.CreateRole.View
{
    public class SelectRoleView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/select_role/select_role_view";

        private int _currentSelectRoleIndex = 0;

        #region UI components
        public Text selectRoleTipsText;
        public Text professionDescriptionText;
        public Text attackText;
        public Text hpText;
        public Text speedText;
        public Slider attackSlider;
        public Slider hpSlider;
        public Slider speedSlider;
        public Text professionNameText;
        public Transform rolesRoot;
        public List<GameObject> rolePositionList;
        public List<RoleView> roleViewList;

        public Text backText;
        public Text nextText;
        #endregion

        public void Awake()
        {
            Init();
            RefreshRoleList();
        }

        private void Init()
        {
            selectRoleTipsText.text = Localization.Get("ui.select_role_view.select_your_role");
            attackText.text = Localization.Get("ui.select_role_view.attack");
            hpText.text = Localization.Get("ui.select_role_view.hp");
            speedText.text = Localization.Get("ui.select_role_view.speed");
            backText.text = Localization.Get("ui.select_role_view.back");
            nextText.text = Localization.Get("ui.select_role_view.next");

            List<PlayerData> basicPlayerDataList = PlayerData.GetBasicPlayerDataList();
            int basicPlayerDataCount = basicPlayerDataList.Count;
            PlayerData basicPlayerData = null;
            for (int basicPlayerDataIndex = 0; basicPlayerDataIndex < basicPlayerDataCount; basicPlayerDataIndex++)
            {
                basicPlayerData = basicPlayerDataList[basicPlayerDataIndex];
                GameObject roleViewGO = GameObject.Instantiate(ResMgr.instance.Load<GameObject>(ResPath.GetFigureImagePath(basicPlayerData.figureImage)) as GameObject);
                roleViewGO.transform.SetParent(rolesRoot, false);
                RoleView roleView = roleViewGO.GetComponent<RoleView>();
                roleView.Init(basicPlayerData);
                roleViewList.Add(roleView);
            }
        }

        private void RefreshRoleList()
        {
            int previousRoleIndex = _currentSelectRoleIndex == 0 ? roleViewList.Count - 1 : _currentSelectRoleIndex - 1;
            int nextRoleIndex = (_currentSelectRoleIndex + 1) % roleViewList.Count;

            RoleView currentSelectRoleView = roleViewList[_currentSelectRoleIndex];
            RoleView previousSelectRoleView = roleViewList[previousRoleIndex];
            RoleView nextSelectRoleView = roleViewList[nextRoleIndex];

            currentSelectRoleView.transform.position = rolePositionList[0].transform.position;
            currentSelectRoleView.transform.SetAsLastSibling();
            currentSelectRoleView.SetAsSelect();
            previousSelectRoleView.transform.position = rolePositionList[1].transform.position;
            previousSelectRoleView.SetAsUnselect();
            nextSelectRoleView.transform.position = rolePositionList[2].transform.position;
            nextSelectRoleView.SetAsUnselect();

            professionDescriptionText.text = Localization.Get(currentSelectRoleView.PlayerData.heroData.description);
            attackSlider.value = currentSelectRoleView.PlayerData.offence * 1.0f / 10;
            hpSlider.value = currentSelectRoleView.PlayerData.defence * 1.0f / 10;
			speedSlider.value = currentSelectRoleView.PlayerData.heroData.speed * 1.0f / 10;
			professionNameText.text = Localization.Get(currentSelectRoleView.PlayerData.heroData.name);
        }

        #region ui event handlers
        public void ClickPreviousRoleHandler()
        {
            _currentSelectRoleIndex = _currentSelectRoleIndex == 0 ? roleViewList.Count - 1 : _currentSelectRoleIndex - 1;
            RefreshRoleList();
        }

        public void ClickNextRoleHandler()
        {
            _currentSelectRoleIndex = (_currentSelectRoleIndex + 1) % roleViewList.Count;
            RefreshRoleList();
        }

        public void ClickBackHandler()
        {
			Logic.UI.Login.View.LoginView.Open();
            UIMgr.instance.Close(PREFAB_PATH);
        }

        public void ClickNextHandler()
        {
            RoleView currentSelectRoleView = roleViewList[_currentSelectRoleIndex];
            CreateRoleController.instance.SelectRole(currentSelectRoleView.PlayerData.Id);
            UIMgr.instance.Open(CreateRoleView.PREFAB_PATH);           
            UIMgr.instance.Close(PREFAB_PATH);
        }
        #endregion
    }
}
