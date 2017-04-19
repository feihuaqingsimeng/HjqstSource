using UnityEngine;
using System.Collections;
using Logic.Player.Model;
using Logic.Protocol.Model;
using System.Collections.Generic;
using Logic.Hero.Model;
using Logic.Role.Model;

namespace Logic.UI.Pvp.Model
{
    public class PvpFighterInfo
    {
        public int power;//战力
        public int rank;//排名
        public int id;
        public PlayerInfo playerInfo;
        public List<TeamHeroProtoData> heroTeamDataList;
        public TeamPlayerProtoData playerDataTeam;
        public string headIcon;
        public string name;
        public PvpFighterInfo(RankArenaOpponentProtoData data)
        {
            PlayerInfo info = new PlayerInfo((uint)data.team.player.id, (uint)data.playerNo, (uint)data.hairCutId, (uint)data.hairColorId, (uint)data.faceId, data.skinId, data.roleName);
            info.level = data.lv;
            playerInfo = info;
            power = data.fightingPower;
            rank = data.rankNo;
            id = data.id;
            name = data.roleName;
            playerDataTeam = data.team.player;
            heroTeamDataList = new List<TeamHeroProtoData>(data.team.heros);
        }

        public List<HeroInfo> GetHeroInfoList()
        {
            List<HeroInfo> infoList = new List<HeroInfo>();
            for (int i = 0, count = heroTeamDataList.Count; i < count; i++)
            {
                TeamHeroProtoData data = heroTeamDataList[i];
                HeroInfo info = new HeroInfo((uint)data.id, data.heroNo, data.breakLayer, data.aggrLv, data.star, data.lv);
                infoList.Add(info);
            }
            return infoList;

        }
		public SortedDictionary<int,RoleInfo> GetRoleInfoDicByPos()
		{
			SortedDictionary<int,RoleInfo> roleDic = new SortedDictionary<int, RoleInfo>();
			if (playerDataTeam != null)
				roleDic.Add(playerDataTeam.posIndex,playerInfo);
			for (int i = 0, count = heroTeamDataList.Count; i < count; i++)
			{
				TeamHeroProtoData data = heroTeamDataList[i];
				HeroInfo info = new HeroInfo((uint)data.id, data.heroNo, data.breakLayer, data.aggrLv, data.star, data.lv);
				roleDic.Add(data.posIndex,info);
			}
			return roleDic;
		}
        public HeroInfo GetHeroInfo(int instanceId)
        {
            for (int i = 0, count = heroTeamDataList.Count; i < count; i++)
            {

                TeamHeroProtoData data = heroTeamDataList[i];
                if (data.id == instanceId)
                {
                    HeroInfo info = new HeroInfo((uint)data.id, data.heroNo, data.breakLayer, data.aggrLv, data.star, data.lv);
                    return info;
                }
            }
            return null;
        }
        public override string ToString()
        {
            return string.Format("[PvpFighterInfo]id:{0},modelId:{1},hairCutId:{2},hairColorId:{3},faceId:{4},skinIndex:{5},level:{6},rank:{7},name:{8}", id,
                                  playerInfo.modelDataId, playerInfo.hairCutIndex, playerInfo.hairColorIndex, playerInfo.faceIndex, playerInfo.skinIndex, playerInfo.level, rank, name);
        }
    }
}

