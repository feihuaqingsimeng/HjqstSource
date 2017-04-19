using UnityEngine;
using UnityEngine.UI;
using Common.Localization;
using System.Collections.Generic;
using Logic.Player.Model;
using Common.ResMgr;

namespace Logic.UI.WorldBoss.View
{
    public class WorldBossDamageRankingItem : MonoBehaviour
    {
        #region UI components
        public Image top3RankingImage;

        private List<Sprite> _top3RankingSprites;
        public List<Sprite> Top3RankingSprites
        {
            get
            {
                if (_top3RankingSprites == null)
                {
                    _top3RankingSprites = new List<Sprite>();
                    _top3RankingSprites.Add(ResMgr.instance.LoadSprite("sprite/main_ui/icon_1st"));
                    _top3RankingSprites.Add(ResMgr.instance.LoadSprite("sprite/main_ui/icon_2st"));
                    _top3RankingSprites.Add(ResMgr.instance.LoadSprite("sprite/main_ui/icon_3st"));
                }
                return _top3RankingSprites;
            }
        }
        public Text rankingNOText;
        public Logic.UI.CommonHeroIcon.View.CommonHeroIcon commonHeroIcon;
        public Text playerNameText;
        public Text totalDamageText;
        public Text totalHurtPercentText;
        #endregion UI components

        public void SetInfo(Logic.Protocol.Model.WorldBossHurtRankProto worldBossHurtRankProto)
        {
            if (worldBossHurtRankProto.rankNo <= 3)
            {
				top3RankingImage.SetSprite(Top3RankingSprites[worldBossHurtRankProto.rankNo - 1]);
                top3RankingImage.SetNativeSize();
                //				top3RankingImage.transform.localScale = new Vector3(0.6f, 0.6f, 1);
                top3RankingImage.gameObject.SetActive(true);
                rankingNOText.gameObject.SetActive(false);
            }
            else
            {
                top3RankingImage.gameObject.SetActive(false);
                rankingNOText.text = worldBossHurtRankProto.rankNo.ToString();
                rankingNOText.gameObject.SetActive(true);
            }
            PlayerInfo mockPlayerInfo = new PlayerInfo(0, (uint)worldBossHurtRankProto.playerNo, (uint)worldBossHurtRankProto.hairCutId, (uint)worldBossHurtRankProto.hairColorId, (uint)worldBossHurtRankProto.faceId, worldBossHurtRankProto.skinId, worldBossHurtRankProto.roleName);
            mockPlayerInfo.level = worldBossHurtRankProto.lv;
            commonHeroIcon.SetPlayerInfo(mockPlayerInfo);
            commonHeroIcon.SetHeroHeadIcon(UIUtil.ParseHeadIcon(worldBossHurtRankProto.headNo));
            playerNameText.text = worldBossHurtRankProto.roleName;
            totalDamageText.text = string.Format(Localization.Get("ui.world_boss_view.total_hurt"), worldBossHurtRankProto.totalHurt);
            string totalHurtPercentValueString = string.Format("{0:F2}", (float)(worldBossHurtRankProto.hurtPercent) / 100);
            totalHurtPercentText.text = string.Format(Localization.Get("ui.world.boss_view.total_hurt_percent"), totalHurtPercentValueString);
        }
    }
}