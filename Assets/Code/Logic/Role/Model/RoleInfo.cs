using UnityEngine;
using System.Collections;
using Logic.Enums;
using Logic.Hero.Model;
using Logic.Game.Model;

namespace Logic.Role.Model
{
	public class RoleInfo 
	{
		public uint instanceID;

		public int level = 1;
		public int exp;
		public int advanceLevel = 0;
		public int strengthenLevel = 0;
		public int strengthenExp = 0;
		public int breakthroughLevel = 1;

		public int weaponID = 0;
		public int armorID = 0;
		public int accessoryID = 0;

		public HeroData heroData;

		public virtual int modelDataId
		{
			get
			{
				return 0;
			}
		}


		public virtual int TotalLevelUpExp
		{
			get
			{
				return HeroExpData.GetHeroExpDataByLv(level-1).exp_total+exp;
			}
			
		}
		public virtual int MaxLevel
		{
			get
			{

				return GameProxy.instance.AccountLevel;
			}
		}
		public virtual bool IsMaxLevel
		{
			get
			{
				return level == MaxLevel;
			}
		}
		public virtual int MaxStrengthenLevel
		{
			get
			{
				return HeroStrengthenNeedData.MaxLevel();
			}
		}
		public virtual int MaxAdvanceLevel
		{
			get
			{
				return 0;
			}
		}
		public virtual bool IsMaxAdvanceLevel
		{
			get
			{
				return advanceLevel == MaxAdvanceLevel;
			}
		}
		public virtual bool IsMaxStrengthenLevel
		{
			get
			{
				return strengthenLevel == MaxStrengthenLevel;
			}
		}
		public virtual string HeadIcon
		{
			get
			{
				if (advanceLevel > heroData.headIcons.Length)
				{
					Debugger.LogError(string.Format("advanceLevel：{0} 超过headIcons的长度了 heroDataid:{1}",advanceLevel,heroData.id));
					return "";
				}
				return Common.ResMgr.ResPath.GetCharacterHeadIconPath(heroData.headIcons[advanceLevel-1]);
			}
		}
		public string ModelName
		{
			get
			{
				if (advanceLevel > heroData.headIcons.Length)
				{
					Debugger.LogError(string.Format("advanceLevel：{0} 超过headIcons的长度了 heroDataid:{1}",advanceLevel,heroData.id));
					return heroData.modelNames[0];
				}
				return heroData.modelNames[advanceLevel - 1];
			}
		}
		public float Power
		{
			get{
				return RoleUtil.CalcRolePower(this);
			}
		}
		public RoleStrengthenStage RoleStrengthenStage
		{
			get
			{
				RoleStrengthenStage roleStrengthenStage = RoleStrengthenStage.White;
				int correspondingHeroStrengthenNeedDataID = strengthenLevel - 1;
				HeroStrengthenNeedData heroStrengthenNeedData = HeroStrengthenNeedData.GetHeroStrengthenNeedDataByID(correspondingHeroStrengthenNeedDataID);
				if (heroStrengthenNeedData != null)
				{
					roleStrengthenStage = heroStrengthenNeedData.roleStrengthenStage;
				}
				return roleStrengthenStage;
			}
		}

		public int StrengthenAddShowValue
		{
			get
			{
				return RoleUtil.GetStrengthenAddShowValue(strengthenLevel);
			}
		}
	}
}
