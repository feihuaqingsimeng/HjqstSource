using System;
using UnityEngine;
using UnityEngine.UI;
using Common.ResMgr;
using Logic.Enums;
using Logic.Role.Model;
using Logic.Hero.Model;
using Logic.Player.Model;

namespace Logic.UI.SkillHead.View
{
    public class SkillHeadView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/skill_head/skill_head_view";

        public GameObject core;
        public RectTransform headRoot;
        public Image headImage;

        public static SkillHeadView Open()
        {
//            SkillHeadView skillHeadView = UIMgr.instance.Open<SkillHeadView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
//            skillHeadView.core.SetActive(false);
//            return skillHeadView;
			return null;
        }

        public static void SetRoleInfo(RoleInfo roleInfo, SkillHeadViewShowType skillHeadViewShowType)
        {
//            SkillHeadView skillHeadView = UIMgr.instance.Get<SkillHeadView>(PREFAB_PATH);
//            if (skillHeadView == null)
//                skillHeadView = SkillHeadView.Open();
//            if (roleInfo == null)
//            {
//                skillHeadView.core.SetActive(false);
//                return;
//            }
//            Sprite skillHeadSprite = ResMgr.instance.Load<Sprite>(ResPath.GetRoleSkillHeadIconPath(roleInfo.heroData.headIcons[roleInfo.advanceLevel - 1]));
//            if (skillHeadSprite == null)
//            {
//                skillHeadView.core.SetActive(false);
//                return;
//            }
//
//            skillHeadView.headImage.SetSprite(skillHeadSprite);
//            skillHeadView.core.SetActive(true);
//            LeanTween.cancel(skillHeadView.gameObject);
//            if (skillHeadViewShowType == SkillHeadViewShowType.Left)
//                skillHeadView.MoveHeadRootFromLeft();
//            else if (skillHeadViewShowType == SkillHeadViewShowType.Right)
//                skillHeadView.MoveHeadRootFromRight();
        }

        private void OnHeadRootMove(Vector2 anchordPosition)
        {
            headRoot.anchoredPosition = anchordPosition;
        }

        private void MoveHeadRootFromLeft()
        {
            headRoot.localScale = Vector3.one;
            headRoot.anchorMin = new Vector2(0, 0.5f);
            headRoot.anchorMax = new Vector2(0, 0.5f);
            headRoot.pivot = new Vector2(0, 0.5f);
            headRoot.anchoredPosition = new Vector2(-headRoot.rect.size.x, headRoot.anchoredPosition.y);
            Vector2 idealAnchoredPosition = new Vector2(0, headRoot.anchoredPosition.y);

            LTDescr ltDescr = LeanTween.value(gameObject, headRoot.anchoredPosition, idealAnchoredPosition, 0.2f);
            ltDescr.setOnUpdate((Action<Vector2>) OnHeadRootMove);
            ltDescr.setIgnoreTimeScale(true);

            LTDescr delayedCallDescr = LeanTween.delayedCall(1.5f, HideCore);
            delayedCallDescr.setIgnoreTimeScale(true);
        }

        private void MoveHeadRootFromRight()
        {
            headRoot.localScale = new Vector3(-1, 1, 1);
            headRoot.anchorMin = new Vector2(1, 0.5f);
            headRoot.anchorMax = new Vector2(1, 0.5f);
            headRoot.pivot = new Vector2(0, 0.5f);
            headRoot.anchoredPosition = new Vector2(headRoot.rect.size.x, headRoot.anchoredPosition.y);
            Vector2 idealAnchoredPosition = new Vector2(0, headRoot.anchoredPosition.y);

            LTDescr ltDescr = LeanTween.value(gameObject, headRoot.anchoredPosition, idealAnchoredPosition, 0.2f);
            ltDescr.setOnUpdate((Action<Vector2>) OnHeadRootMove);
            ltDescr.setIgnoreTimeScale(true);

            LTDescr delayedCallDescr = LeanTween.delayedCall(1.5f, HideCore);
            delayedCallDescr.setIgnoreTimeScale(true);
        }

        private void HideCore()
        {
            core.gameObject.SetActive(false);
        }
    }
}
