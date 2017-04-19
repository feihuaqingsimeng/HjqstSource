using UnityEngine;
using System.Collections;
using Logic.Hero.Model;
using Logic.Protocol.Model;

namespace Logic.Fight.Model
{
    public class FightHeroInfo
    {
        public HeroInfo heroInfo;
		public HeroFightProtoData  pveHeroProtoData;

		public FightHeroInfo()
		{
			
		}
		public FightHeroInfo(HeroInfo heroInfo,HeroFightProtoData  pveHeroProtoData)
		{
			this.heroInfo = heroInfo;
			this.pveHeroProtoData = pveHeroProtoData;
		}
    }
}
