using UnityEngine;
using System.Collections;
using Logic.Player.Model;
using Logic.Protocol.Model;

namespace Logic.Fight.Model
{
    public class FightPlayerInfo
    {
        public PlayerInfo playerInfo;
		public PlayerFightProtoData pvePlayerProtoData;

		public FightPlayerInfo()
		{

		}
		public FightPlayerInfo(PlayerInfo playerInfo,PlayerFightProtoData pvePlayerData)
		{
			this.playerInfo = playerInfo;
			this.pvePlayerProtoData = pvePlayerData;
		}
    }
}
