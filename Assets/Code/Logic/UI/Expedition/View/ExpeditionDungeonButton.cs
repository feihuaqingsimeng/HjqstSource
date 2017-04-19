using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Logic.UI.Expedition.Model;
using Logic.Enums;
using Common.ResMgr;
using Logic.UI.Tips.View;
using Logic.UI.Expedition.Controller;
using Logic.Game.Model;
using Common.Util;
using Logic.Player.Model;
using Logic.Character;
using Common.Animators;
using Common.Localization;

namespace Logic.UI.Expedition.View
{
    public class ExpeditionDungeonButton : MonoBehaviour
    {
        #region ui component
        public Text normalDungeonNameText;
        public Image imageNormalDungeonBg;
        public Image imageTreasureBox;
        public GameObject normalRoot;
        public GameObject normalStarRoot;
        public Transform playerModelRoot;
        public Transform playerModelBoxRoot;
        #endregion

        public ExpeditionDungeonInfo expeditionDungeonInfo;
        private CharacterEntity _characterEntity;

        public void SetDungeonInfo(ExpeditionDungeonInfo info)
        {
            expeditionDungeonInfo = info;
            TransformUtil.ClearChildren(playerModelRoot, true);
			TransformUtil.ClearChildren(playerModelBoxRoot, true);
            Refresh();
        }

        private void Refresh()
        {
            bool isFinish = expeditionDungeonInfo.isFinished;
            bool isUnlock = expeditionDungeonInfo.isUnlocked;
            bool isGetReward = expeditionDungeonInfo.isGetReward;
            string path = "sprite/main_ui/";
            if (expeditionDungeonInfo.data.type == (int)ExpeditionDungeonType.Expedition_Normal)
            {
                imageTreasureBox.transform.parent.gameObject.SetActive(false);
                normalRoot.SetActive(true);
                if (isUnlock)
                {
					imageNormalDungeonBg.SetSprite(ResMgr.instance.Load<Sprite>(path + "icon_bg_checkpoint_02_01"));
                }
                else
                {
					imageNormalDungeonBg.SetSprite(ResMgr.instance.Load<Sprite>(path + "icon_bg_checkpoint_02_02"));
                }
                //star
                normalStarRoot.SetActive(isFinish);
                normalDungeonNameText.gameObject.SetActive(isUnlock);
                normalDungeonNameText.text = string.Empty;
            }
            else
            {
				imageTreasureBox.transform.parent.gameObject.SetActive(true);
                normalRoot.SetActive(false);
                string notOpenPath = path + "icon_box_3_1";
                string openPath = path + "icon_box_3_2";
				imageTreasureBox.SetSprite(ResMgr.instance.Load<Sprite>(isGetReward ? openPath : notOpenPath));
                imageTreasureBox.SetGray(!isUnlock);
                imageTreasureBox.SetNativeSize();
                if (expeditionDungeonInfo.data.type == (int)ExpeditionDungeonType.Expedition_SmallTreasureBox)
                {
                    imageTreasureBox.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                }
                else
                {
                    imageTreasureBox.transform.localScale = Vector3.one;
                }
            }

        }

        public void CreatePlayer()
        {
			Transform parent = null;
			if (expeditionDungeonInfo.data.type == (int)ExpeditionDungeonType.Expedition_Normal)
			{
				parent = playerModelRoot;
			}else
			{
				parent = playerModelBoxRoot;
			}
            PlayerInfo playerInfo = GameProxy.instance.PlayerInfo;
			_characterEntity = CharacterEntity.CreatePlayerEntityAsUIElement(playerInfo, parent, false, false);
            AnimatorUtil.SetBool(_characterEntity.anim, AnimatorUtil.RUN, true);
        }

        private void DespawnCharacter()
        {
            if (_characterEntity)
                Pool.Controller.PoolController.instance.Despawn(_characterEntity.name, _characterEntity);
            _characterEntity = null;
        }

        void OnDestroy()
        {
            DespawnCharacter();
        }

        public void OnClickDungeonButtonHandler()
        {

            if (!expeditionDungeonInfo.isUnlocked)
            {
                if (expeditionDungeonInfo.data.type != (int)ExpeditionDungeonType.Expedition_Normal)
                {
                    ExpeditionData data = ExpeditionData.GetExpeditionDataByID(expeditionDungeonInfo.id);
                    CommonRewardTipsView.Open(data.rewardList);
                }

                return;
            }
            ExpeditionProxy.instance.selectExpeditionDungeonInfo = expeditionDungeonInfo;
            int type = expeditionDungeonInfo.data.type;
            if (type == (int)ExpeditionDungeonType.Expedition_Normal)
            {

                if (expeditionDungeonInfo.isFinished)
                {
                    CommonAutoDestroyTipsView.Open(Localization.Get("ui.expedition_view.conquerOther"));
                    return;
                }
            }
            else
            {
                if (expeditionDungeonInfo.isFinished)
                {
                    ExpeditionData data = ExpeditionData.GetExpeditionDataByID(expeditionDungeonInfo.id);
                    if (!expeditionDungeonInfo.isGetReward)
                    {
                        CommonRewardTipsView.Open(data.rewardList, true, ClickRewardBtnHandler);
                    }
                    else
                    {
                        CommonRewardTipsView.Open(data.rewardList);
                    }
                    return;
                }
            }

            //ExpeditionFormationView.Open(true);
            ExpeditionEmbattleView.Open(true);
        }
        private void ClickRewardBtnHandler()
        {
            ExpeditionController.instance.CLIENT2LOBBY_Expedition_TreasureReward_REQ(expeditionDungeonInfo.id);
        }
    }

}
